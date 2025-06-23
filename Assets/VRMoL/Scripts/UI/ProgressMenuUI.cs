using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRMoL.Core;

namespace VRMoL.UI
{
    public class ProgressMenuUI : MonoBehaviour
    {
        [Header("UI参照")]
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private Button nextButton;

        [Header("機能コンポーネント")]
        // 自動で探すのではなく、インスペクターから直接設定するように変更
        [SerializeField] private LocationWarpManager warpManager;

        [Header("進捗設定")]
        private float totalTimeSeconds = 600f; // 10分

        [Header("ユーザー参照")]
        [SerializeField] private Transform xrOrigin; // XR OriginやMain Camera
        [SerializeField] private RectTransform menuPanelTransform; // メニューUIのRectTransform

        private float timeRemaining;
        private bool isVisible = false;

        private void Start()
        {
            // warpManager = FindFirstObjectByType<LocationWarpManager>(); // この行を削除
            
            // warpManagerが設定されているか確認
            if (warpManager == null)
            {
                Debug.LogError("LocationWarpManagerがインスペクターから設定されていません！", this);
                return; // 設定されていなければ、ここで処理を中断
            }

            timeRemaining = totalTimeSeconds;

            if (nextButton != null)
            {
                nextButton.onClick.AddListener(OnNextButtonClicked);
            }
            HideMenu();
        }

        private void Update()
        {
            if (!isVisible) return;
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0) timeRemaining = 0;
            UpdateDisplay();
        }

        public void ShowMenu()
        {
            // ユーザーの左側・前方にメニューを表示
            if (xrOrigin != null && menuPanelTransform != null)
            {
                Vector3 left = -xrOrigin.right;
                Vector3 forward = xrOrigin.forward;
                Vector3 basePos = xrOrigin.position + left * 0.5f + forward * 0.5f + Vector3.up * 1.2f;
                menuPanelTransform.position = basePos;

                // メニューがユーザーの方を向くように回転を計算します。
                Vector3 lookDirection = menuPanelTransform.position - xrOrigin.position;
                lookDirection.y = 0;
                menuPanelTransform.rotation = Quaternion.LookRotation(lookDirection.normalized);
            }

            isVisible = true;
            if (menuPanel != null) menuPanel.SetActive(true);
            UpdateDisplay();
        }

        public void HideMenu()
        {
            isVisible = false;
            if (menuPanel != null) menuPanel.SetActive(false);
        }

        public void ToggleMenu()
        {
            if (isVisible) HideMenu();
            else ShowMenu();
        }

        private void UpdateDisplay()
        {
            if (titleText != null)
                titleText.text = "Progress";

            if (progressText != null && warpManager != null)
            {
                int currentLocationNumber = warpManager.GetCurrentPointIndex();
                int totalLocations = warpManager.GetTotalPoints();
                progressText.text = $"Location: {currentLocationNumber}/{totalLocations}";
            }
            
            if (timeText != null)
                timeText.text = $"Time Remaining: {FormatTime(timeRemaining)}";

            // Nextボタンを押せるかどうかを制御
            // テレポートポイントが1つ以上あれば、常にボタンを押せるようにします
            if (nextButton != null && warpManager != null)
            {
                nextButton.interactable = warpManager.GetTotalPoints() > 0;
            }
        }

        private string FormatTime(float seconds)
        {
            int min = Mathf.FloorToInt(seconds / 60f);
            int sec = Mathf.FloorToInt(seconds % 60f);
            return $"{min:D2}:{sec:D2}";
        }

        private void OnNextButtonClicked()
        {
            if (warpManager == null)
            {
                Debug.LogError("ワープマネージャーが見つかりません！");
                return;
            }
            
            warpManager.TeleportToNext();
        }
    }
} 