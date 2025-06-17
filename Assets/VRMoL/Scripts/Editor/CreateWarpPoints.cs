using UnityEngine;
using UnityEditor;

public class CreateWarpPoints : MonoBehaviour
{
    [MenuItem("Tools/ワープポイントを自動生成")]
    static void CreateWarpPointsAtLocations()
    {
        for (int i = 1; i <= 10; i++)
        {
            string locationName = $"location{i}";
            GameObject location = GameObject.Find(locationName);
            if (location == null)
            {
                Debug.LogWarning($"{locationName} が見つかりません。");
                continue;
            }

            // すでにWarpPointがあればスキップ
            if (location.transform.Find($"WarpPoint_{i}") != null)
            {
                Debug.Log($"{locationName} にはすでにWarpPoint_{i}があります。");
                continue;
            }

            GameObject warpPoint = new GameObject($"WarpPoint_{i}");
            warpPoint.transform.parent = location.transform;
            warpPoint.transform.localPosition = Vector3.zero; // 中心に配置
            Debug.Log($"{locationName} に WarpPoint_{i} を作成しました。");
        }
        Debug.Log("ワープポイント自動生成 完了！");
    }
} 