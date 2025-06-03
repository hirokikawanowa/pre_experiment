using UnityEngine;
using UnityEditor;

namespace VRMoL.Template
{
    public class JapaneseStudyPrefabCreator : MonoBehaviour
    {
        [Header("Room Settings")]
        public float roomWidth = 5f;
        public float roomLength = 5f;
        public float roomHeight = 3f;

        [Header("Materials")]
        public Material wallMaterial;
        public Material floorMaterial;
        public Material ceilingMaterial;

        [Header("Japanese Study Assets")]
        public GameObject tatamiPrefab;
        public GameObject shojiPrefab;
        public GameObject writingDeskPrefab;
        public GameObject scrollPrefab;
        public AudioClip rainSound;
        public AudioClip windChimeSound;
        public AudioClip gardenWaterSound;

        [Header("Prefab Settings")]
        public string prefabName = "JapaneseStudy";
        public string prefabPath = "Assets/VRMoL/Templates/BaseRoom";

        public void CreateJapaneseStudyPrefab()
        {
            // 基本の部屋を生成
            BaseRoomPrefabCreator baseCreator = gameObject.AddComponent<BaseRoomPrefabCreator>();
            baseCreator.roomWidth = roomWidth;
            baseCreator.roomLength = roomLength;
            baseCreator.roomHeight = roomHeight;
            baseCreator.wallMaterial = wallMaterial;
            baseCreator.floorMaterial = floorMaterial;
            baseCreator.ceilingMaterial = ceilingMaterial;
            baseCreator.prefabName = prefabName;
            baseCreator.prefabPath = prefabPath;

            // 基本の部屋を生成
            baseCreator.CreateBaseRoomPrefab();

            // 和風書斎のテンプレートを追加
            GameObject room = new GameObject(prefabName);
            JapaneseStudyTemplate template = room.AddComponent<JapaneseStudyTemplate>();
            template.tatamiPrefab = tatamiPrefab;
            template.shojiPrefab = shojiPrefab;
            template.writingDeskPrefab = writingDeskPrefab;
            template.scrollPrefab = scrollPrefab;
            template.rainSound = rainSound;
            template.windChimeSound = windChimeSound;
            template.gardenWaterSound = gardenWaterSound;

            // Prefabとして保存
            #if UNITY_EDITOR
            string fullPath = $"{prefabPath}/{prefabName}.prefab";
            PrefabUtility.SaveAsPrefabAsset(room, fullPath);
            DestroyImmediate(room);
            #endif
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(JapaneseStudyPrefabCreator))]
    public class JapaneseStudyPrefabCreatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            JapaneseStudyPrefabCreator creator = (JapaneseStudyPrefabCreator)target;
            if (GUILayout.Button("Create Japanese Study Prefab"))
            {
                creator.CreateJapaneseStudyPrefab();
            }
        }
    }
    #endif
} 