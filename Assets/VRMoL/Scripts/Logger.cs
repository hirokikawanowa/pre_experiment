using UnityEngine;
using System.IO;
using System;

public class Logger : MonoBehaviour
{
    private string logPath;
    private StreamWriter writer;

    void Start()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        logPath = Path.Combine(Application.persistentDataPath, $"VRMoL_Log_{timestamp}.csv");
        writer = new StreamWriter(logPath, true);
        writer.WriteLine("Timestamp,EventType,LocationIndex,WordIndex,HeadPosition,HeadRotation");
    }

    public void LogEvent(string eventType, int locationIndex = -1, int wordIndex = -1)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        Vector3 headPos = Camera.main.transform.position;
        Quaternion headRot = Camera.main.transform.rotation;
        
        writer.WriteLine($"{timestamp},{eventType},{locationIndex},{wordIndex}," +
                        $"{headPos.x},{headPos.y},{headPos.z}," +
                        $"{headRot.x},{headRot.y},{headRot.z},{headRot.w}");
        writer.Flush();
    }

    void OnApplicationQuit()
    {
        if (writer != null)
        {
            writer.Close();
            writer = null;
        }
    }
} 