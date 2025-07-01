using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using VRMoL.Core;

namespace VRMoL.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("カード・単語管理")]
        [SerializeField] private WordCardList wordCardList;
        [SerializeField] private CardSpawner[] cardSpawners;
        [SerializeField] private LocationWarpManager warpManager;
        [SerializeField] private AudioController audioController;

        private List<WordCardData> round1Words;
        private List<WordCardData> round2Words;
        private int currentRound = 1; // 1:1周目, 2:2周目
        private int currentLocationIndex = 0; // 0〜9
        private const int LOCATIONS = 10;
        private const int CARDS_PER_LOCATION = 2;

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
            Debug.Log($"[GameManager] Start: round1Words={round1Words.Count}, round2Words={round2Words.Count}");

            // Loggerで実験開始を記録
            var logger = FindObjectOfType<VRMoL.UI.Logger>();
            if (logger != null)
            {
                logger.LogTaskStart(currentRound);
            }

            // 強制的にRound1の最初のロケーションにカードを出す
            if (warpManager != null)
            {
                warpManager.CurrentState = LocationWarpManager.GameState.Round1;
                warpManager.SetVisitCount(1);
                SpawnCardsForCurrentLocation();
            }
        }

        private void InitializeGame()
        {
            // 1周目・2周目の単語リストをランダム抽選
            var shuffled = wordCardList.cards.OrderBy(x => Random.value).ToList();
            round1Words = shuffled.Take(LOCATIONS * CARDS_PER_LOCATION).ToList();
            round2Words = shuffled.Skip(LOCATIONS * CARDS_PER_LOCATION).Take(LOCATIONS * CARDS_PER_LOCATION).ToList();
            currentRound = 1;
            currentLocationIndex = 0;
            // 追加: 初期ロケーションの環境音再生（削除）
            // if (audioController != null)
            // {
            //     audioController.PlayLocationAudio(currentLocationIndex);
            // }
        }

        public void OnLocationChanged(int locationIndex)
        {
            currentLocationIndex = locationIndex;
            SpawnCardsForCurrentLocation();
        }

        public void OnRoundChanged(int round)
        {
            currentRound = round;
            currentLocationIndex = 0;
            SpawnCardsForCurrentLocation();
            // 追加: ラウンド変更時も環境音を最初のロケーションに
            if (audioController != null)
            {
                audioController.PlayLocationAudio(currentLocationIndex);
            }
        }

        public void SpawnCardsForCurrentLocation()
        {
            Debug.Log($"[GM] SpawnCardsForCurrentLocation called: currentLocationIndex={currentLocationIndex}, currentRound={currentRound}");
            // すべてのCardSpawnerをクリア
            foreach (var spawner in cardSpawners)
            {
                if (spawner != null) spawner.ClearAllCards();
            }
            if (currentLocationIndex >= 0 && currentLocationIndex < cardSpawners.Length)
            {
                List<WordCardData> wordList = (currentRound == 1) ? round1Words : round2Words;
                var cardsForLocation = wordList.Skip(currentLocationIndex * CARDS_PER_LOCATION).Take(CARDS_PER_LOCATION).ToArray();
                Debug.Log($"[GM] SpawnCards: locIdx={currentLocationIndex}, round={currentRound}, cards={string.Join(",", cardsForLocation.Select(c => c.word))}");
                cardSpawners[currentLocationIndex].SpawnCards(cardsForLocation);
            }
            else
            {
                Debug.Log($"[GM] カード生成スキップ: currentLocationIndex={currentLocationIndex}, cardSpawners.Length={cardSpawners.Length}");
            }
        }

        // LocationWarpManagerやUIから呼び出す用
        public void NextLocation()
        {
            // LocationWarpManagerのOnButtonPressedで進行するので、ここではカード生成のみ
            SpawnCardsForCurrentLocation();
            // 追加: ロケーション進行時の環境音切り替え
            if (audioController != null)
            {
                int idx = GetCurrentLocationIndex();
                audioController.PlayLocationAudio(idx);
            }
        }

        public int GetCurrentRound()
        {
            if (warpManager == null) return 1;
            var state = warpManager.CurrentState;
            if (state == LocationWarpManager.GameState.Round2) return 2;
            return 1;
        }

        public int GetCurrentLocationIndex()
        {
            if (warpManager == null) return 0;
            var state = warpManager.CurrentState;
            int visitCount = warpManager.GetCurrentVisitCount();
            List<int> order = null;
            if (state == LocationWarpManager.GameState.Round1)
            {
                order = typeof(LocationWarpManager).GetField("round1Order", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(warpManager) as List<int>;
            }
            else if (state == LocationWarpManager.GameState.Round2)
            {
                order = typeof(LocationWarpManager).GetField("round2Order", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(warpManager) as List<int>;
            }
            if (order != null && visitCount > 0 && visitCount <= order.Count)
            {
                return order[visitCount - 1] - 1;
            }
            return 0;
        }

        public void SwitchAudioMode(bool isSpatialAudio)
        {
            // Implementation needed
        }

        public void SaveExperimentData()
        {
            // Loggerで実験終了を記録
            var logger = FindObjectOfType<VRMoL.UI.Logger>();
            if (logger != null)
            {
                float duration = Time.timeSinceLevelLoad; // 実験開始からの経過時間
                logger.LogTaskEnd(currentRound, duration);
            }
            // TODO: 実験データの保存処理を実装
        }

        public void ClearAllCardsAllLocations()
        {
            if (cardSpawners == null) return;
            foreach (var spawner in cardSpawners)
            {
                if (spawner != null) spawner.ClearAllCards();
            }
        }

        public void ForceSpawnCardsDebug()
        {
            Debug.Log("[GM] ForceSpawnCardsDebug called");
            SpawnCardsForCurrentLocation();
        }
    }
} 