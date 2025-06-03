using UnityEngine;
using System.Collections.Generic;
using Meta.XR;

namespace VRMoL.Core
{
    public class AudioController : MonoBehaviour
    {
        [SerializeField] private AudioSource ambientAudioSource;
        [SerializeField] private AudioSource spatialAudioSource;
        [SerializeField] private float spatialAudioRadius = 1.0f;

        private bool isSpatialAudioEnabled = false;
        private Transform playerHead;

        private void Start()
        {
            // プレイヤーの頭の位置を取得
            var player = FindObjectOfType<OVRCameraRig>();
            if (player != null)
            {
                playerHead = player.centerEyeAnchor;
            }

            // 初期設定
            SetSpatialAudio(false);
        }

        public void SetSpatialAudio(bool enabled)
        {
            isSpatialAudioEnabled = enabled;
            
            if (ambientAudioSource != null)
            {
                ambientAudioSource.spatialBlend = enabled ? 1.0f : 0.0f;
            }

            if (spatialAudioSource != null)
            {
                spatialAudioSource.spatialBlend = enabled ? 1.0f : 0.0f;
            }
        }

        public void SetAmbientSound(AudioClip clip)
        {
            if (ambientAudioSource != null && clip != null)
            {
                ambientAudioSource.clip = clip;
                ambientAudioSource.Play();
            }
        }

        public void SetSpatialSound(AudioClip clip, Vector3 position)
        {
            if (spatialAudioSource != null && clip != null)
            {
                spatialAudioSource.clip = clip;
                spatialAudioSource.transform.position = position;
                spatialAudioSource.Play();
            }
        }

        private void Update()
        {
            if (isSpatialAudioEnabled && playerHead != null && spatialAudioSource != null)
            {
                // プレイヤーの頭の周りに音源を配置
                Vector3 randomOffset = Random.insideUnitSphere * spatialAudioRadius;
                spatialAudioSource.transform.position = playerHead.position + randomOffset;
            }
        }
    }
} 