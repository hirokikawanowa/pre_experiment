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

        public virtual void InitializeRoom(GameObject room)
        {
            // マテリアルの適用
            if (wallMaterial != null)
            {
                var walls = room.transform.Find("Walls");
                if (walls != null)
                {
                    var renderers = walls.GetComponentsInChildren<Renderer>();
                    foreach (var renderer in renderers)
                    {
                        renderer.material = wallMaterial;
                    }
                }
            }

            if (floorMaterial != null)
            {
                var floor = room.transform.Find("Floor");
                if (floor != null)
                {
                    var renderer = floor.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material = floorMaterial;
                    }
                }
            }

            if (ceilingMaterial != null)
            {
                var ceiling = room.transform.Find("Ceiling");
                if (ceiling != null)
                {
                    var renderer = ceiling.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material = ceilingMaterial;
                    }
                }
            }

            // 小物の配置
            var propsContainer = room.transform.Find("Props");
            if (propsContainer != null)
            {
                foreach (var prop in props)
                {
                    if (prop.propPrefab != null)
                    {
                        GameObject instance = Instantiate(prop.propPrefab, propsContainer);
                        instance.transform.localPosition = prop.position;
                        instance.transform.localRotation = prop.rotation;
                        instance.transform.localScale = prop.scale;
                    }
                }
            }

            // 環境音の設定
            if (ambientSound != null)
            {
                var audioSource = room.GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = room.AddComponent<AudioSource>();
                }
                audioSource.clip = ambientSound;
                audioSource.volume = ambientVolume;
                audioSource.loop = true;
                audioSource.Play();
            }

            // 照明の設定
            RenderSettings.ambientLight = ambientLightColor;
            RenderSettings.ambientIntensity = ambientIntensity;

            var lightsContainer = room.transform.Find("Lights");
            if (lightsContainer != null)
            {
                foreach (var lightSetting in lights)
                {
                    GameObject lightObj = new GameObject("Light");
                    lightObj.transform.SetParent(lightsContainer);
                    lightObj.transform.localPosition = lightSetting.position;
                    lightObj.transform.localRotation = lightSetting.rotation;

                    Light light = lightObj.AddComponent<Light>();
                    light.type = lightSetting.type;
                    light.color = lightSetting.color;
                    light.intensity = lightSetting.intensity;
                    light.range = lightSetting.range;
                    if (lightSetting.type == LightType.Spot)
                    {
                        light.spotAngle = lightSetting.spotAngle;
                    }
                }
            }
        }
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