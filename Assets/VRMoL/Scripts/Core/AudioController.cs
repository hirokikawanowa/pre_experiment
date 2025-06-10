using UnityEngine;
using System.Collections.Generic;
using Meta.XR;

namespace VRMoL.Core
{
    public class AudioController : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
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
            
            if (audioSource != null)
            {
                audioSource.spatialBlend = enabled ? 1.0f : 0.0f;
            }
        }

        public void SetSound(AudioClip clip, Vector3? position = null)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.clip = clip;
                if (position.HasValue)
                {
                    audioSource.transform.position = position.Value;
                }
                audioSource.Play();
            }
        }

        private void Update()
        {
            if (isSpatialAudioEnabled && playerHead != null && audioSource != null)
            {
                // プレイヤーの頭の周りに音源を配置
                Vector3 randomOffset = Random.insideUnitSphere * spatialAudioRadius;
                audioSource.transform.position = playerHead.position + randomOffset;
            }
        }
    }
} 