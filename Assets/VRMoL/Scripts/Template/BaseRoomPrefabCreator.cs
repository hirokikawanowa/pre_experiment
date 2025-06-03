using UnityEngine;
using UnityEditor;

namespace VRMoL.Template
{
    public class BaseRoomPrefabCreator : MonoBehaviour
    {
        [Header("Room Settings")]
        public float roomWidth = 5f;
        public float roomLength = 5f;
        public float roomHeight = 3f;

        [Header("Materials")]
        public Material wallMaterial;
        public Material floorMaterial;
        public Material ceilingMaterial;

        [Header("Prefab Settings")]
        public string prefabName = "BaseRoom";
        public string prefabPath = "Assets/VRMoL/Templates/BaseRoom";

        public void CreateBaseRoomPrefab()
        {
            // RoomGeneratorを取得
            RoomGenerator roomGenerator = GetComponent<RoomGenerator>();
            if (roomGenerator == null)
            {
                roomGenerator = gameObject.AddComponent<RoomGenerator>();
            }

            // マテリアルを設定
            roomGenerator.defaultWallMaterial = wallMaterial;
            roomGenerator.defaultFloorMaterial = floorMaterial;
            roomGenerator.defaultCeilingMaterial = ceilingMaterial;

            // 部屋を生成
            GameObject room = roomGenerator.GenerateRoom(roomWidth, roomLength, roomHeight);

            // BaseRoomTemplateを追加して初期化
            BaseRoomTemplate roomTemplate = room.AddComponent<BaseRoomTemplate>();
            roomTemplate.roomWidth = roomWidth;
            roomTemplate.roomLength = roomLength;
            roomTemplate.roomHeight = roomHeight;
            roomTemplate.wallMaterial = wallMaterial;
            roomTemplate.floorMaterial = floorMaterial;
            roomTemplate.ceilingMaterial = ceilingMaterial;

            // Prefabとして保存
            #if UNITY_EDITOR
            if (!System.IO.Directory.Exists(prefabPath))
            {
                System.IO.Directory.CreateDirectory(prefabPath);
            }

            string fullPath = $"{prefabPath}/{prefabName}.prefab";
            PrefabUtility.SaveAsPrefabAsset(room, fullPath);
            DestroyImmediate(room);
            #endif
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(BaseRoomPrefabCreator))]
    public class BaseRoomPrefabCreatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BaseRoomPrefabCreator creator = (BaseRoomPrefabCreator)target;
            if (GUILayout.Button("Create Base Room Prefab"))
            {
                creator.CreateBaseRoomPrefab();
            }
        }
    }
    #endif
} 