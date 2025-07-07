using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using System.Collections;

namespace VRMoL.UI
{
    public class LogExporter : MonoBehaviour
    {
        [SerializeField] private Logger logger;

        // Google Apps Script（GAS）送信用メソッド
        public IEnumerator SendLogToGAS(string logText)
        {
            string url = "https://script.google.com/macros/s/AKfycbxcIofgUNjD1SiKHB097pNadSV235L7onQq4zHZTdSNLQyXIT6Jt8eQUtveOXUjcGPY/exec"; // GASのWebアプリURLに置換
            var json = "{\"log\":\"" + EscapeForJson(logText) + "\"}";

            Debug.Log($"[DEBUG-GAS] 送信開始: {logText.Length}文字");
            UnityWebRequest www = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
                Debug.Log("[DEBUG-GAS] GAS送信成功" );
            else
                Debug.LogError("[DEBUG-GAS] GAS送信失敗: " + www.error);
        }

        // JSON用にエスケープ（改行やダブルクォート対策）
        private string EscapeForJson(string s)
        {
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "");
        }
    }
}