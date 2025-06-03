using UnityEngine;
using System.Collections.Generic;

namespace VRMoL.Template
{
    [CreateAssetMenu(menuName = "VRMoL/LocationTemplate")]
    public class LocationTemplate : ScriptableObject
    {
        [Header("基本情報")]
        public string locationName;
        public string locationDescription;

        [Header("マテリアル設定")]
        public Material wallMaterial;
        public Material floorMaterial;
        public Material ceilingMaterial;

        [Header("小物配置")]
        public List<PropPlacement> props = new List<PropPlacement>();

        [Header("オーディオ設定")]
        public AudioClip ambientSound;
        [Range(0f, 1f)]
        public float ambientVolume = 0.5f;

        [Header("照明設定")]
        public Color ambientLightColor = Color.white;
        [Range(0f, 8f)]
        public float ambientIntensity = 1.0f;
        public List<LightSettings> lights = new List<LightSettings>();
    }

    [System.Serializable]
    public class PropPlacement
    {
        public GameObject propPrefab;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale = Vector3.one;
    }

    [System.Serializable]
    public class LightSettings
    {
        public LightType type = LightType.Point;
        public Color color = Color.white;
        [Range(0f, 8f)]
        public float intensity = 1.0f;
        public Vector3 position;
        public Quaternion rotation;
        [Range(0f, 179f)]
        public float spotAngle = 30f;
        public float range = 10f;
    }
} 