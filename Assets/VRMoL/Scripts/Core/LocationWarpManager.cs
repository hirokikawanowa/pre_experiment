using UnityEngine;
// using UnityEngine.InputSystem; // Input System を使うために必要
using System.Collections; // コルーチンを使うために必要

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

        // 現在どのテレポートポイントにいるかを示すインデックス
        private int currentIndex = 0;

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
            WarpToNextLocation();
        }
        */

        // UIボタンなどから呼び出される公開メソッド
        public void TeleportToNext()
        {
            // テレポートポイントが1つも設定されていない場合は、警告を出して処理を中断します
            if (teleportPoints.Length == 0)
            {
                Debug.LogWarning("テレポートポイント(Teleport Points)が1つも設定されていません！", this);
                return;
            }

            // 次の場所のインデックスを計算します。最後の場所なら最初に戻ります。
            currentIndex = (currentIndex + 1) % teleportPoints.Length;
            
            // 安全なテレポート処理を開始します
            StartCoroutine(WarpSafely(teleportPoints[currentIndex]));
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

            Debug.Log($"テレポート成功: {currentIndex + 1}番目のポイント ({destination.name}) へ移動しました。", this);
        }

        // --- 外部から情報を取得するためのオプショナルなメソッド ---
        public int GetCurrentPointIndex() => currentIndex + 1;
        public int GetTotalPoints() => teleportPoints.Length;
    }
} 