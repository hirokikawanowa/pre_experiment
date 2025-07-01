using UnityEngine;
using System.Collections.Generic;
using Meta.XR;

namespace VRMoL.Core
{
    [System.Serializable]
    public class AudioSettings
    {
        public bool isSpatial = true;
        public float spatialBlend = 1.0f;
        public float minDistance = 1.0f;
        public float maxDistance = 10.0f;
        public AnimationCurve falloffCurve;
    }

    public class AudioController : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float spatialAudioRadius = 1.0f;
        public AudioSource[] locationAudioSources;
        public AudioSettings audioSettings;
        public int currentLocation = 0;

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
            // シーン開始時は音声を再生しない（PlayLocationAudio(0)は呼ばない）
        }

        public void SetSpatialAudio(bool enabled)
        {
            isSpatialAudioEnabled = enabled;
            
            if (audioSource != null)
            {
                audioSource.spatialBlend = enabled ? 1.0f : 0.0f;
            }

            audioSettings.isSpatial = enabled;
            audioSettings.spatialBlend = enabled ? 1.0f : 0.0f;
            foreach (var src in locationAudioSources)
            {
                src.spatialBlend = audioSettings.spatialBlend;
            }

            // Loggerで音声モード切替を記録
            var logger = FindObjectOfType<VRMoL.UI.Logger>();
            var gm = FindObjectOfType<VRMoL.Core.GameManager>();
            if (logger != null && gm != null)
            {
                int round = gm.GetCurrentRound();
                int locIdx = gm.GetCurrentLocationIndex();
                string location = $"Location{locIdx + 1}";
                logger.LogAudioModeChange(round, location, enabled);
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

        public void PlayLocationAudio(int locationIndex)
        {
            // まず全てStop
            foreach (var src in locationAudioSources)
            {
                if (src.isPlaying) src.Stop();
                src.time = 0f;
            }
            // 指定indexだけPlay
            if (locationIndex >= 0 && locationIndex < locationAudioSources.Length)
            {
                locationAudioSources[locationIndex].spatialBlend = audioSettings.spatialBlend;
                locationAudioSources[locationIndex].Play();
            }
            currentLocation = locationIndex;
        }

        // 追加: 全ロケーションの環境音を停止
        public void StopAllLocationAudio()
        {
            foreach (var src in locationAudioSources)
            {
                if (src.isPlaying) src.Stop();
                src.time = 0f;
            }
        }
    }
} 