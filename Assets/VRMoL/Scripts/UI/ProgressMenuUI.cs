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
        [SerializeField] private Button closeButton;

        [Header("進捗設定")]
        [SerializeField] private int totalPlaceables = 5;
        [SerializeField] private float totalTimeSeconds = 900f; // 15分

        private int placedCount = 0;
        private float timeRemaining;
        private bool isVisible = false;
        private LocationWarpManager warpManager;

        private void Start()
        {
            warpManager = FindObjectOfType<LocationWarpManager>();
            timeRemaining = totalTimeSeconds;
            if (nextButton != null) nextButton.onClick.AddListener(OnNextButtonClicked);
            if (closeButton != null) closeButton.onClick.AddListener(ToggleMenu);
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

        public void SetPlacedCount(int count)
        {
            placedCount = count;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (titleText != null)
                titleText.text = "Progress Display";
            if (progressText != null)
                progressText.text = $"Placeables placed: {placedCount}/{totalPlaceables}";
            if (timeText != null)
                timeText.text = $"Time remaining: {FormatTime(timeRemaining)}";
            if (nextButton != null && warpManager != null)
                nextButton.interactable = warpManager.GetCurrentLocationIndex() < warpManager.GetTotalLocations();
        }

        private string FormatTime(float seconds)
        {
            int min = Mathf.FloorToInt(seconds / 60f);
            int sec = Mathf.FloorToInt(seconds % 60f);
            return $"{min:D2}:{sec:D2}";
        }

        private void OnNextButtonClicked()
        {
            if (warpManager == null) return;
            int nextIndex = warpManager.GetCurrentLocationIndex(); // 1-indexed
            if (nextIndex < warpManager.GetTotalLocations())
            {
                warpManager.WarpToLocation(nextIndex); // 0-indexed
                // 進捗や残り時間のリセットが必要ならここで行う
            }
            UpdateDisplay();
        }
    }
} 