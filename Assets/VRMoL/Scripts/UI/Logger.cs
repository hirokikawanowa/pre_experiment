using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

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

        private void Awake()
        {
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

                    try
                    {
                        logWriter = new StreamWriter(logFilePath, true);
                        // CSVヘッダーの書き込み
                        logWriter.WriteLine("Timestamp,EventType,Location,Data");
                        logWriter.Flush();
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
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logMessage = $"{timestamp},{eventType},{location},{data}";

            if (logToConsole)
            {
                Debug.Log(logMessage);
            }

            if (logToFile && logWriter != null)
            {
                try
                {
                    logWriter.WriteLine(logMessage);
                    logWriter.Flush();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to write to log file: {e.Message}");
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

        public void LogAudioModeChange(string location, bool isSpatialAudio)
        {
            string data = $"SpatialAudio:{isSpatialAudio}";
            LogEvent("AudioModeChange", location, data);
        }

        private void OnDestroy()
        {
            if (logWriter != null)
            {
                try
                {
                    logWriter.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error closing log file: {e.Message}");
                }
            }
        }

        private void OnApplicationQuit()
        {
            if (logWriter != null)
            {
                try
                {
                    logWriter.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error closing log file: {e.Message}");
                }
            }
        }
    }
} 