using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif
#if UNITY_EDITOR
using Newtonsoft.Json;
#endif

namespace VRMoL.UI
{
    public class Logger : MonoBehaviour
    {
        [SerializeField] private string logFileName = "experiment_data.csv";
        [SerializeField] private bool logToFile = true;
        [SerializeField] private bool logToConsole = true;

        private string logFilePath;
        private StreamWriter logWriter;
        private bool isInitialized = false;
        private List<string> logBuffer = new List<string>();
        private string lastLogMessage = null;

        private void Awake()
        {
            StartCoroutine(InitializeWithPermissions());
        }

        private IEnumerator InitializeWithPermissions()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            // Android 13以降や11-12のストレージ制限は考慮不要
            // 権限リクエストも不要
#endif
            yield return null;
            InitializeLogger();
        }

        private void InitializeLogger()
        {
            if (!isInitialized)
            {
                if (logToFile)
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string fileName = $"{timestamp}_{logFileName}";
                    logFilePath = Path.Combine(Application.persistentDataPath, fileName);
                    Debug.Log($"Primary log path: {logFilePath}");
                    try
                    {
                        string directory = Path.GetDirectoryName(logFilePath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        logWriter = new StreamWriter(logFilePath, true);
                        logWriter.WriteLine("Timestamp,EventType,Location,Data");
                        logWriter.Flush();
                        foreach (var bufferedLog in logBuffer)
                        {
                            logWriter.WriteLine(bufferedLog);
                        }
                        logBuffer.Clear();
                        logWriter.Flush();
                        Debug.Log("Logger initialized successfully");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to initialize logger: {e.Message}");
                        logToFile = false;
                    }
                }
                isInitialized = true;
            }
        }

        public void LogEvent(string eventType, string location, string data)
        {
            if (eventType == "AudioModeChange" || eventType == "CardPlacement")
            {
                location = "---";
            }
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logMessage = $"{timestamp},{eventType},{location},{data}";
            if (logMessage == lastLogMessage) return;
            lastLogMessage = logMessage;
            if (logToConsole)
            {
                Debug.Log(logMessage);
            }
            if (logToFile)
            {
                if (logWriter != null)
                {
                    try
                    {
                        logWriter.WriteLine(logMessage);
                        logWriter.Flush();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to write to log file: {e.Message}");
                        logBuffer.Add(logMessage);
                    }
                }
                else
                {
                    logBuffer.Add(logMessage);
                }
            }
        }

        public void LogCardPlacement(string location, string cardId, Vector3 position, Quaternion rotation)
        {
            string data = $"CardID:{cardId},Position:{position},Rotation:{rotation}";
            LogEvent("CardPlacement", location, data);
        }

        public void LogLocationChange(string fromLocation, string toLocation)
        {
            string data = $"From:{fromLocation},To:{toLocation}";
            LogEvent("LocationChange", toLocation, data);
        }

        public void LogAudioModeChange(int roundNumber, string location, bool isSpatialAudio)
        {
            string data = $"Round={roundNumber},SpatialAudio={isSpatialAudio}";
            LogEvent("AudioModeChange", location, data);
        }

        public void LogTaskStart(int roundNumber)
        {
            string data = $"Round={roundNumber}";
            LogEvent("TaskStart", "-", data);
        }

        public void LogTaskEnd(int roundNumber, float duration)
        {
            string data = $"Round={roundNumber},Duration={duration:F2}";
            LogEvent("TaskEnd", "-", data);
        }

        public void LogLocationEnter(int roundNumber, string location)
        {
            string data = $"Round={roundNumber}";
            LogEvent("LocationEnter", location, data);
        }

        public void LogLocationExit(int roundNumber, string location, float stayTime)
        {
            string data = $"Round={roundNumber},StayTime={stayTime:F2}";
            LogEvent("LocationExit", location, data);
        }

        public void LogCardPlacement(int roundNumber, string location, string cardId, int wordIndex, int orderInLocation, Vector3 position, Quaternion rotation)
        {
            string data = $"Round={roundNumber},CardID={cardId},WordIndex={wordIndex},OrderInLocation={orderInLocation},Position=({position.x:F2},{position.y:F2},{position.z:F2}),Rotation=({rotation.x:F2},{rotation.y:F2},{rotation.z:F2},{rotation.w:F2})";
            LogEvent("CardPlacement", location, data);
            Debug.Log($"[DEBUG-CardPlacement] ロケーション:{location} 順番:{orderInLocation} カードID:{cardId} 単語Index:{wordIndex} Round:{roundNumber} 位置:({position.x:F2},{position.y:F2},{position.z:F2})");
        }

        private void OnDestroy()
        {
            CloseLogger();
        }

        private void OnApplicationQuit()
        {
            CloseLogger();
            // LogExporterを探してGoogleフォーム送信を実行
            var logExporter = FindObjectOfType<LogExporter>();
            if (logExporter != null)
            {
                logExporter.StartCoroutine(logExporter.SendLogToGAS(GetAllLogText()));
            }
            else
            {
                Debug.LogWarning("[Logger] LogExporterが見つかりませんでした。Googleフォーム送信はスキップされます。");
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                if (logWriter != null)
                {
                    logWriter.Flush();
                }
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            // 何もしない
        }

        private void CloseLogger()
        {
            if (logWriter != null)
            {
                logWriter.Flush();
                logWriter.Close();
                logWriter = null;
            }
        }

        public string GetLogFilePath()
        {
            return logFilePath;
        }

        public bool IsLogFileExists()
        {
            return !string.IsNullOrEmpty(logFilePath) && File.Exists(logFilePath);
        }

        public string GetAllLogText()
        {
            if (!string.IsNullOrEmpty(logFilePath) && File.Exists(logFilePath))
            {
                return File.ReadAllText(logFilePath);
            }
            return string.Empty;
        }

        public string GetLogJsonArray()
        {
            if (!string.IsNullOrEmpty(logFilePath) && File.Exists(logFilePath))
            {
                var lines = File.ReadAllLines(logFilePath);
                if (lines.Length < 2) return "[]"; // データなし
                var headers = lines[0].Split(',');
                var result = new List<Dictionary<string, string>>();
                foreach (var line in lines.Skip(1))
                {
                    var values = line.Split(',');
                    if (values.Length == headers.Length)
                    {
                        var dict = new Dictionary<string, string>();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dict[headers[i]] = values[i];
                        }
                        result.Add(dict);
                    }
                }
#if UNITY_EDITOR
                return JsonConvert.SerializeObject(result, Formatting.Indented);
#else
                return UnityEngine.JsonUtility.ToJson(new Wrapper<Dictionary<string, string>> { Items = result });
#endif
            }
            return "[]";
        }

        [Serializable]
        private class Wrapper<T>
        {
            public List<T> Items;
        }
    }
} 