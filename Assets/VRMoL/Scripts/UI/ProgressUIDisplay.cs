using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRMoL.Core;

namespace VRMoL.UI
{
    public class ProgressUIDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject progressPanel;
        [SerializeField] private TextMeshProUGUI locationText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Image progressBar;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button previousButton;
        [SerializeField] private Button closeButton;

        private LocationManager locationManager;
        private bool isVisible = false;

        private void Start()
        {
            locationManager = FindObjectOfType<LocationManager>();
            if (locationManager == null)
            {
                Debug.LogError("LocationManager not found!");
                return;
            }

            // ボタンのイベントリスナーを設定
            if (nextButton != null)
            {
                nextButton.onClick.AddListener(OnNextButtonClicked);
            }

            if (previousButton != null)
            {
                previousButton.onClick.AddListener(OnPreviousButtonClicked);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseButtonClicked);
            }

            // 初期状態では非表示
            if (progressPanel != null)
            {
                progressPanel.SetActive(false);
            }
        }

        public void ToggleProgressUI()
        {
            isVisible = !isVisible;
            if (progressPanel != null)
            {
                progressPanel.SetActive(isVisible);
            }

            if (isVisible)
            {
                UpdateProgressDisplay();
            }
        }

        private void UpdateProgressDisplay()
        {
            if (locationManager == null) return;

            int currentLocation = locationManager.GetCurrentLocationIndex();
            int totalLocations = locationManager.GetTotalLocations();

            // ロケーション情報の更新
            if (locationText != null)
            {
                locationText.text = $"現在のロケーション: {currentLocation} / {totalLocations}";
            }

            // 進捗バーの更新
            if (progressBar != null)
            {
                float progress = (float)currentLocation / totalLocations;
                progressBar.fillAmount = progress;
            }

            // 進捗テキストの更新
            if (progressText != null)
            {
                progressText.text = $"進捗: {Mathf.RoundToInt(progressBar.fillAmount * 100)}%";
            }

            // ボタンの有効/無効状態の更新
            if (nextButton != null)
            {
                nextButton.interactable = currentLocation < totalLocations;
            }

            if (previousButton != null)
            {
                previousButton.interactable = currentLocation > 1;
            }
        }

        private void OnNextButtonClicked()
        {
            locationManager.LoadNextLocation();
            UpdateProgressDisplay();
        }

        private void OnPreviousButtonClicked()
        {
            locationManager.LoadPreviousLocation();
            UpdateProgressDisplay();
        }

        private void OnCloseButtonClicked()
        {
            ToggleProgressUI();
        }
    }
} 