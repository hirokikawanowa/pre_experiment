using UnityEngine;
// using UnityEngine.InputSystem; // Input System を使うために必要
using System.Collections; // コルーチンを使うために必要
using System.Collections.Generic;

namespace VRMoL.Core
{
    // XR OriginにこのコンポーネントとCharacterControllerがアタッチされていることを要求する
    [RequireComponent(typeof(CharacterController))]
    public class LocationWarpManager : MonoBehaviour
    {
        /*
        [Header("入力設定")]
        [SerializeField]
        // インスペクターから "WarpNext" アクションを直接設定します
        private InputActionReference warpActionReference;
        */

        [Header("テレポート設定")]
        [SerializeField]
        // インスペクターからテレポート先となるTransformのリストを設定します
        private Transform[] teleportPoints;

        // XR Originのコンポーネントを保持
        private CharacterController characterController;

        // --- ゲーム状態管理 ---
        public enum GameState { WaitingToStart, Round1, BreakTime, Round2, Finished }
        public GameState CurrentState { get; set; } = GameState.WaitingToStart;

        // --- 訪問順と状態の管理 ---
        private int visitCount = 0;   // 1始まりにする
        private readonly List<int> round1Order = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private readonly List<int> round2Order = new List<int> { 3, 7, 1, 10, 2, 5, 8, 4, 9, 6 };

        // --- 外部コンポーネントへの参照 ---
        // UIのタイマーをリセットするために使用
        [SerializeField] private VRMoL.UI.ProgressMenuUI progressMenuUI;
        [SerializeField] private VRMoL.Core.AudioController audioController;

        // 追加: 1周目の空間/非空間オーディオ設定
        private bool isRound1Spatial = true;

        public int CurrentLocationIndex { get; private set; } = 0;

        // --- ロケーション入退室ログ用 ---
        private float lastEnterTime = 0f;
        private int lastLocationIndex = -1;

        private void Awake()
        {
            // 必要なコンポーネントを自動で取得
            characterController = GetComponent<CharacterController>();

            /*
            // 念のため、インスペクターでアクションが設定されているか確認します
            if (warpActionReference == null)
            {
                Debug.LogError("ワープ用の入力アクション(Warp Action Reference)が設定されていません！", this);
                return;
            }
            */

            if (teleportPoints.Length == 0)
            {
                Debug.LogWarning("テレポートポイント(Teleport Points)が1つも設定されていません！", this);
                return;
            }
        }

        /*
        private void OnEnable()
        {
            // このコンポーネントが有効になった時に、入力アクションを監視し始めます
            if (warpActionReference != null)
            {
                // ボタンが押された瞬間に HandleWarpAction メソッドが呼ばれるように予約します
                warpActionReference.action.performed += HandleWarpAction;
                warpActionReference.action.Enable();
            }
        }

        private void OnDisable()
        {
            // このコンポーネントが無効になった時に、監視を解除します
            if (warpActionReference != null)
            {
                warpActionReference.action.performed -= HandleWarpAction;
                warpActionReference.action.Disable();
            }
        }
        */

        /*
        // Input Actionから呼び出されるプライベートメソッド
        private void HandleWarpAction(InputAction.CallbackContext context)
        {
            // WarpToNextLocation();
        }
        */

        // UIボタンなどから呼び出される公開メソッド
        public void OnButtonPressed()
        {
            Debug.Log($"[LocationWarpManager] OnButtonPressed called. CurrentState: {CurrentState}, VisitCount: {visitCount}");

            // ProgressMenuUIの参照が切れていたら再取得
            if (progressMenuUI == null)
            {
                progressMenuUI = FindObjectOfType<VRMoL.UI.ProgressMenuUI>();
            }

            switch (CurrentState)
            {
                case GameState.WaitingToStart:
                    Debug.Log("[LocationWarpManager] Starting Round 1");
                    CurrentState = GameState.Round1;
                    visitCount = 0;

                    if (progressMenuUI != null)
                    {
                        if (!progressMenuUI.gameObject.activeSelf)
                            progressMenuUI.gameObject.SetActive(true);
                        if (progressMenuUI.transform.parent != null && !progressMenuUI.transform.parent.gameObject.activeSelf)
                            progressMenuUI.transform.parent.gameObject.SetActive(true);
                        progressMenuUI.ResetTimer();
                        progressMenuUI.StartTimer();
                        Debug.Log("[LocationWarpManager] Timer reset & started for Round 1");
                    }
                    else
                    {
                        Debug.LogError("[LocationWarpManager] progressMenuUI is null!");
                    }

                    // 追加: 1周目の空間/非空間をランダム決定し反映
                    isRound1Spatial = Random.value > 0.5f;
                    if (audioController != null)
                    {
                        audioController.SetSpatialAudio(isRound1Spatial);
                        audioController.PlayLocationAudio(0);
                    }

                    TeleportToCurrentLocation();

                    // 状態変更後にGameManagerに通知
                    if (VRMoL.Core.GameManager.Instance != null)
                    {
                        VRMoL.Core.GameManager.Instance.NextLocation();
                    }
                    break;

                case GameState.Round1:
                    if (visitCount < round1Order.Count)
                    {
                        TeleportToCurrentLocation();
                    }
                    else // 10/10でFinishボタンが押された
                    {
                        Debug.Log("[LocationWarpManager] Finishing Round 1, entering BreakTime");

                        if (progressMenuUI != null)
                        {
                            progressMenuUI.StopTimer();
                            Debug.Log("[LocationWarpManager] Timer stopped for BreakTime");
                        }

                        // 1周目終了時に全カードを即時削除
                        if (VRMoL.Core.GameManager.Instance != null)
                        {
                            VRMoL.Core.GameManager.Instance.ClearAllCardsAllLocations();
                        }

                        // 追加: 1周目終了時に全環境音を停止
                        if (audioController != null)
                        {
                            audioController.StopAllLocationAudio();
                        }

                        CurrentState = GameState.BreakTime;
                        visitCount = 0;
                    }
                    break;

                case GameState.BreakTime:
                    Debug.Log("[LocationWarpManager] Starting Round 2");
                    CurrentState = GameState.Round2;
                    visitCount = 0;

                    if (VRMoL.Core.GameManager.Instance != null)
                    {
                        VRMoL.Core.GameManager.Instance.OnRoundChanged(2);
                    }

                    if (progressMenuUI == null)
                    {
                        progressMenuUI = FindObjectOfType<VRMoL.UI.ProgressMenuUI>();
                        Debug.Log("[LocationWarpManager] progressMenuUI was null, reacquired: " + (progressMenuUI != null));
                    }
                    if (progressMenuUI != null)
                    {
                        if (!progressMenuUI.gameObject.activeSelf)
                            progressMenuUI.gameObject.SetActive(true);
                        if (progressMenuUI.transform.parent != null && !progressMenuUI.transform.parent.gameObject.activeSelf)
                            progressMenuUI.transform.parent.gameObject.SetActive(true);
                        progressMenuUI.ResetTimer();
                        progressMenuUI.StartTimer();
                        Debug.Log("[LocationWarpManager] Timer reset & started for Round 2");
                    }
                    else
                    {
                        Debug.LogError("[LocationWarpManager] progressMenuUI is null!");
                    }

                    // 追加: 2周目は1周目と逆の空間/非空間を反映
                    if (audioController != null)
                    {
                        audioController.SetSpatialAudio(!isRound1Spatial);
                    }

                    TeleportToCurrentLocation();
                    break;

                case GameState.Round2:
                    if (visitCount < round2Order.Count)
                    {
                        TeleportToCurrentLocation();
                    }
                    else // 10/10でFinishボタンが押された
                    {
                        Debug.Log("[LocationWarpManager] Finishing Round 2");

                        if (progressMenuUI != null)
                        {
                            progressMenuUI.StopTimer();
                            Debug.Log("[LocationWarpManager] Timer stopped for Finished state");
                        }

                        // 追加: 2周目終了時に全環境音を停止
                        if (audioController != null)
                        {
                            audioController.StopAllLocationAudio();
                        }

                        CurrentState = GameState.Finished;
                        visitCount = 0;
                    }
                    break;

                case GameState.Finished:
                    Debug.Log("[LocationWarpManager] Game is already finished");
                    // 何もしない
                    break;
            }
        }

        private void TeleportToCurrentLocation()
        {
            // ロケーション退室ログ（ワープ前）
            if (lastLocationIndex >= 0)
            {
                var logger = FindObjectOfType<VRMoL.UI.Logger>();
                var gm = VRMoL.Core.GameManager.Instance;
                if (logger != null && gm != null)
                {
                    int round = gm.GetCurrentRound();
                    string location = $"Location{lastLocationIndex + 1}";
                    float stayTime = Time.time - lastEnterTime;
                    logger.LogLocationExit(round, location, stayTime);
                }
            }

            // visitCountをワープ前にインクリメント
            visitCount++;

            // ワープ先ロケーションのインデックスを計算
            int locationIndex = -1;
            List<int> order = (CurrentState == GameState.Round1) ? round1Order : round2Order;
            if (visitCount > 0 && visitCount <= order.Count)
            {
                locationIndex = order[visitCount - 1] - 1; // 0-based index
            }
            CurrentLocationIndex = locationIndex;

            // ロケーション入室ログ（ワープ直後）
            if (locationIndex >= 0)
            {
                var logger = FindObjectOfType<VRMoL.UI.Logger>();
                var gm = VRMoL.Core.GameManager.Instance;
                if (logger != null && gm != null)
                {
                    int round = gm.GetCurrentRound();
                    string location = $"Location{locationIndex + 1}";
                    logger.LogLocationEnter(round, location);
                }
                lastEnterTime = Time.time;
                lastLocationIndex = locationIndex;
            }

            // ワープ処理
            if (locationIndex >= 0 && locationIndex < teleportPoints.Length)
            {
                StartCoroutine(WarpSafely(teleportPoints[locationIndex]));
            }

            // ワープ直後にGameManagerに通知
            if (VRMoL.Core.GameManager.Instance != null && locationIndex >= 0)
            {
                VRMoL.Core.GameManager.Instance.OnLocationChanged(locationIndex);
            }
        }

        // CharacterControllerを考慮した安全なテレポート処理
        private IEnumerator WarpSafely(Transform destination)
        {
            if (destination == null)
            {
                Debug.LogError("テレポート先(destination)が無効です。", this);
                yield break; // 処理を中断
            }

            // 1. テレポート前にCharacterControllerを無効化
            characterController.enabled = false;

            // 2. プレイヤーの位置と回転をテレポート先に合わせる
            //    (this.transform は、このスクリプトがアタッチされているオブジェクトのTransformを指します)
            this.transform.position = destination.position;
            this.transform.rotation = destination.rotation;

            // 3. 1フレーム待ってからCharacterControllerを再度有効化
            //    (これをしないと位置が正しく反映されないことがあるため)
            yield return null;
            characterController.enabled = true;

            // 修正: 正しいロケーションindexで音声切り替え
            if (audioController != null)
            {
                int locationIndex = GetCurrentLocationIndex();
                audioController.PlayLocationAudio(locationIndex);
            }

            Debug.Log($"テレポート成功: {visitCount}/{GetTotalPoints()} ({destination.name}) へ移動しました。 (State: {CurrentState})", this);
        }

        // --- 外部から情報を取得するためのオプショナルなメソッド ---
        public int GetCurrentVisitCount() => visitCount;
        public int GetTotalPoints()
        {
            if (CurrentState == GameState.Round1 || CurrentState == GameState.BreakTime)
                return round1Order.Count;
            if (CurrentState == GameState.Round2 || CurrentState == GameState.Finished)
                return round2Order.Count;
            return 0;
        }

        public int GetCurrentLocationIndex()
        {
            List<int> currentOrder = (CurrentState == GameState.Round1) ? round1Order : round2Order;
            int idx = visitCount - 1;
            if (idx < 0 || idx >= currentOrder.Count) return 0;
            // ロケーション番号（1始まり）→配列index（0始まり）に変換
            return currentOrder[idx] - 1;
        }

        // --- 追加: 現在のラウンド順を取得するpublicメソッド ---
        public List<int> GetCurrentRoundOrder()
        {
            return (CurrentState == GameState.Round1) ? round1Order : round2Order;
        }

        public void SetVisitCount(int count)
        {
            visitCount = count;
        }
    }
} 