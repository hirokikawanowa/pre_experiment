using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

namespace VRMoL.Interaction
{
    public class CardController : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
    {
        [SerializeField] private Sprite cardImage;
        [SerializeField] private AudioClip cardSound;
        [SerializeField] private float snapDistance = 0.1f;
        [SerializeField] private LayerMask placementLayer;

        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private bool isPlaced = false;
        private AudioSource audioSource;

        protected override void Awake()
        {
            base.Awake();
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            if (isPlaced)
            {
                isPlaced = false;
            }
            PlayCardSound();
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            TrySnapToPlacement();
        }

        private void TrySnapToPlacement()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, snapDistance, placementLayer);
            if (colliders.Length > 0)
            {
                // 最も近い配置ポイントを見つける
                Transform closestPoint = null;
                float closestDistance = float.MaxValue;

                foreach (var collider in colliders)
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPoint = collider.transform;
                    }
                }

                if (closestPoint != null)
                {
                    // カードを配置ポイントにスナップ
                    transform.position = closestPoint.position;
                    transform.rotation = closestPoint.rotation;
                    isPlaced = true;
                    PlayCardSound();
                }
            }
        }

        private void PlayCardSound()
        {
            if (audioSource != null && cardSound != null)
            {
                audioSource.PlayOneShot(cardSound);
            }
        }

        public void SetCardImage(Sprite image)
        {
            if (image != null)
            {
                cardImage = image;
                // カードの見た目を更新
                var renderer = GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    // マテリアルのテクスチャを更新
                    var material = renderer.material;
                    material.mainTexture = image.texture;
                }
            }
        }

        public bool IsPlaced()
        {
            return isPlaced;
        }

        public void ResetPosition()
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            isPlaced = false;
        }
    }
} 