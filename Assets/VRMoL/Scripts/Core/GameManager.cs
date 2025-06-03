using UnityEngine;
using System.Collections.Generic;

namespace VRMoL.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private LocationManager locationManager;
        [SerializeField] private AudioController audioController;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            if (locationManager == null)
            {
                locationManager = FindObjectOfType<LocationManager>();
            }

            if (audioController == null)
            {
                audioController = FindObjectOfType<AudioController>();
            }

            // 初期ロケーションの読み込み
            locationManager?.LoadInitialLocation();
        }

        public void SwitchAudioMode(bool isSpatialAudio)
        {
            audioController?.SetSpatialAudio(isSpatialAudio);
        }

        public void SaveExperimentData()
        {
            // TODO: 実験データの保存処理を実装
        }
    }
} 