using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRMoL.Core;
using System.Collections;

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
        [SerializeField] private LocationWarpManager warpManager;

        [Header("進捗設定")]
        [SerializeField] private float totalTimeSeconds = 600f; // 10分

        [Header("ユーザー参照")]
        [SerializeField] private Transform xrOrigin;
        [SerializeField] private RectTransform menuPanelTransform;

        [Header("デバッグ")]
        [SerializeField] private bool debugMode = false;
        [SerializeField] private bool useCoroutineTimer = true; // VR用にコルーチンベースのタイマーを使用

        private float timeRemaining;
        private bool isVisible = false;
        private bool isTimerRunning = false;
        private float lastUpdateTime = 0f;
        private Coroutine timerCoroutine;

        [SerializeField] public float defaultDuration = 600f;

        public bool IsTimerRunning => isTimerRunning;
        public float TimeRemaining => timeRemaining;

        private void Awake()
        {
            Debug.Log($"[PMUI] Awake id={GetInstanceID()}");
            StartCoroutine(InitializeAfterXRSetup());
            if (debugMode) 
            {
                Debug.Log($"[ProgressMenuUI] Awake called on {gameObject.name}");
                Debug.Log($"[ProgressMenuUI] Application.platform: {Application.platform}");
            }
        }

        private IEnumerator InitializeAfterXRSetup()
        {
            yield return new WaitForSeconds(0.5f);
            Initialize();
        }

        private void Initialize()
        {
            if (warpManager == null)
            {
                Debug.LogError("[ProgressMenuUI] LocationWarpManagerがインスペクターから設定されていません！", this);
                return;
            }

            timeRemaining = totalTimeSeconds;
            isTimerRunning = false;
            isVisible = false;

            if (nextButton != null)
            {
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(() => {
                    if (debugMode) Debug.Log("[ProgressMenuUI] Next button clicked in VR");
                    if (!isTimerRunning && warpManager.CurrentState == LocationWarpManager.GameState.WaitingToStart)
                    {
                        StartTimer();
                    }
                    warpManager.OnButtonPressed();
                });
            }
            if (menuPanel != null) menuPanel.SetActive(false);
            UpdateDisplay();
            if (debugMode)
            {
                Debug.Log($"[ProgressMenuUI] Initialize completed. Timer: {timeRemaining}s, isVisible: {isVisible}");
            }
        }

        private void OnEnable()
        {
            Debug.Log($"[PMUI] OnEnable id={GetInstanceID()}");
            if (isTimerRunning && useCoroutineTimer && timerCoroutine == null)
            {
                timerCoroutine = StartCoroutine(TimerCoroutine());
            }
        }

        private void OnDisable()
        {
            Debug.Log($"[PMUI] OnDisable id={GetInstanceID()}");
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
                timerCoroutine = null;
            }
        }

        private void Update()
        {
            if (useCoroutineTimer && isTimerRunning)
            {
                if (isVisible && menuPanel != null && menuPanel.activeInHierarchy)
                {
                    UpdateDisplay();
                }
                return;
            }
            if (!gameObject.activeInHierarchy)
            {
                if (debugMode) Debug.LogWarning("[ProgressMenuUI] GameObject is not active!");
                return;
            }
            if (isTimerRunning && timeRemaining > 0)
            {
                float deltaTime = Time.deltaTime;
                if (deltaTime > 0.1f) deltaTime = 0.1f;
                timeRemaining -= deltaTime;
                if (timeRemaining < 0) timeRemaining = 0;
                if (debugMode && Time.time - lastUpdateTime > 1f)
                {
                    lastUpdateTime = Time.time;
                    Debug.Log($"[ProgressMenuUI] Timer running (Update): {FormatTime(timeRemaining)}");
                }
                if (timeRemaining <= 0)
                {
                    OnTimerComplete();
                }
            }
            if (isVisible && menuPanel != null && menuPanel.activeInHierarchy)
            {
                UpdateDisplay();
            }
        }

        private IEnumerator TimerCoroutine()
        {
            Debug.Log("[ProgressMenuUI] TimerCoroutine started");
            while (isTimerRunning && timeRemaining > 0)
            {
                yield return new WaitForSeconds(0.1f);
                timeRemaining -= 0.1f;
                if (timeRemaining < 0) timeRemaining = 0;
                if (debugMode && Mathf.FloorToInt(timeRemaining) % 10 == 0 && 
                    Mathf.Abs(timeRemaining - Mathf.Floor(timeRemaining)) < 0.15f)
                {
                    Debug.Log($"[ProgressMenuUI] Timer running (Coroutine): {FormatTime(timeRemaining)}");
                }
                if (timeRemaining <= 0)
                {
                    OnTimerComplete();
                    break;
                }
            }
            timerCoroutine = null;
            Debug.Log("[ProgressMenuUI] TimerCoroutine ended");
        }

        public void ResetTimer()
        {
            timeRemaining = totalTimeSeconds;
            isTimerRunning = false;
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
                timerCoroutine = null;
            }
            if (debugMode) Debug.Log($"[ProgressMenuUI] Timer reset to {totalTimeSeconds}s");
        }

        public void StartTimer()
        {
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
                timerCoroutine = null;
            }
            isTimerRunning = true;
            timerCoroutine = StartCoroutine(TimerCoroutine());
            ShowMenu();
            if (debugMode)
            {
                Debug.Log($"[ProgressMenuUI] Timer started. Duration: {totalTimeSeconds}s, isRunning: {isTimerRunning}, useCoroutine: {useCoroutineTimer}");
            }
        }

        public void StopTimer() 
        {
            isTimerRunning = false;
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
                timerCoroutine = null;
                Debug.Log("[ProgressMenuUI] timerCoroutine stopped in StopTimer()");
            }
            if (debugMode) Debug.Log("[ProgressMenuUI] Timer stopped");
        }

        private void OnTimerComplete()
        {
            isTimerRunning = false;
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
                timerCoroutine = null;
            }
            if (debugMode) Debug.Log("[ProgressMenuUI] Timer completed!");
        }

        public void ShowMenu()
        {
            if (menuPanel != null)
            {
                menuPanel.SetActive(true);
                isVisible = true;
                PositionMenuForUser();
                UpdateDisplay();
                if (debugMode) Debug.Log($"[ProgressMenuUI] Menu shown. Panel active: {menuPanel.activeSelf}");
            }
            else
            {
                Debug.LogError("[ProgressMenuUI] menuPanel is null!");
            }
        }

        public void HideMenu()
        {
            isVisible = false;
            if (menuPanel != null)
            {
                menuPanel.SetActive(false);
                if (debugMode) Debug.Log("[ProgressMenuUI] Menu hidden");
            }
        }

        public void ToggleMenu()
        {
            if (isVisible) HideMenu();
            else ShowMenu();
        }

        private void PositionMenuForUser()
        {
            if (xrOrigin != null && menuPanelTransform != null)
            {
                Vector3 right = xrOrigin.right;
                Vector3 forward = xrOrigin.forward;
                Vector3 basePos = xrOrigin.position + right * 0.05f + forward * 0.7f + Vector3.up * 1.2f;
                menuPanelTransform.position = basePos;
                Vector3 lookDirection = menuPanelTransform.position - xrOrigin.position;
                lookDirection.y = 0;
                if (lookDirection != Vector3.zero)
                {
                    menuPanelTransform.rotation = Quaternion.LookRotation(lookDirection.normalized);
                }
            }
        }

        private void UpdateDisplay()
        {
            if (titleText != null)
                titleText.text = "Progress";
            if (timeText != null)
            {
                string timeString = $"Time Remaining: {FormatTime(timeRemaining)}";
                if (timeText.text != timeString)
                {
                    timeText.text = timeString;
                }
                if (timeRemaining < 60f)
                {
                    timeText.color = Color.red;
                }
                else if (timeRemaining < 180f)
                {
                    timeText.color = Color.yellow;
                }
                else
                {
                    timeText.color = Color.white;
                }
            }
            if (warpManager != null)
            {
                var state = warpManager.CurrentState;
                var buttonText = nextButton?.GetComponentInChildren<TextMeshProUGUI>();
                int visitCount = warpManager.GetCurrentVisitCount();
                int totalLocations = warpManager.GetTotalPoints();
                switch (state)
                {
                    case LocationWarpManager.GameState.WaitingToStart:
                        if (buttonText != null) buttonText.text = "Start";
                        if (progressText != null) progressText.text = "Ready to start Round 1.";
                        break;
                    case LocationWarpManager.GameState.Round1:
                    case LocationWarpManager.GameState.Round2:
                        if (buttonText != null)
                        {
                            if (visitCount == totalLocations)
                                buttonText.text = "Finish";
                            else
                                buttonText.text = "Next";
                        }
                        if (progressText != null) progressText.text = $"Location: {visitCount}/{totalLocations}";
                        break;
                    case LocationWarpManager.GameState.BreakTime:
                        if (buttonText != null) buttonText.text = "Start";
                        if (progressText != null) progressText.text = "Round 1 finished. Ready for Round 2.";
                        StopTimer();
                        break;
                    case LocationWarpManager.GameState.Finished:
                        if (buttonText != null) buttonText.text = "Finish";
                        if (progressText != null) progressText.text = "All rounds completed!";
                        if (nextButton != null) nextButton.interactable = false;
                        StopTimer();
                        break;
                }
            }
        }

        private string FormatTime(float seconds)
        {
            int min = Mathf.FloorToInt(seconds / 60f);
            int sec = Mathf.FloorToInt(seconds % 60f);
            return $"{min:D2}:{sec:D2}";
        }

        #if UNITY_EDITOR
        [ContextMenu("Test Start Timer")]
        private void TestStartTimer()
        {
            StartTimer();
        }

        [ContextMenu("Test Stop Timer")]
        private void TestStopTimer()
        {
            StopTimer();
        }

        [ContextMenu("Test Toggle Menu")]
        private void TestToggleMenu()
        {
            ToggleMenu();
        }
        
        [ContextMenu("Log Timer Status")]
        private void LogTimerStatus()
        {
            Debug.Log($"[ProgressMenuUI] Status - Running: {isTimerRunning}, Time: {FormatTime(timeRemaining)}, Visible: {isVisible}");
            Debug.Log($"[ProgressMenuUI] GameObjects - This: {gameObject.activeInHierarchy}, Panel: {menuPanel?.activeInHierarchy}");
        }
        #endif
    }
} 