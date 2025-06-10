using UnityEngine;
using VRMoL.Core;

namespace VRMoL.Template
{
    public class LocationCustomizer : MonoBehaviour
    {
        [SerializeField] private LocationTemplate template;
        [SerializeField] private Transform wallsContainer;
        [SerializeField] private Transform floorContainer;
        [SerializeField] private Transform ceilingContainer;
        [SerializeField] private Transform propsContainer;
        [SerializeField] private Transform lightsContainer;

        private AudioController audioController;

        private void Start()
        {
            audioController = FindObjectOfType<AudioController>();
            ApplyTemplate();
        }

        public void ApplyTemplate()
        {
            if (template == null) return;

            // マテリアルの適用
            ApplyMaterials();

            // 小物の配置
            PlaceProps();

            // 照明の設定
            SetupLights();

            // 環境音の設定
            SetupAmbientSound();
        }

        private void ApplyMaterials()
        {
            if (wallsContainer != null && template.wallMaterial != null)
            {
                foreach (Renderer renderer in wallsContainer.GetComponentsInChildren<Renderer>())
                {
                    renderer.material = template.wallMaterial;
                }
            }

            if (floorContainer != null && template.floorMaterial != null)
            {
                foreach (Renderer renderer in floorContainer.GetComponentsInChildren<Renderer>())
                {
                    renderer.material = template.floorMaterial;
                }
            }

            if (ceilingContainer != null && template.ceilingMaterial != null)
            {
                foreach (Renderer renderer in ceilingContainer.GetComponentsInChildren<Renderer>())
                {
                    renderer.material = template.ceilingMaterial;
                }
            }
        }

        private void PlaceProps()
        {
            if (propsContainer == null) return;

            // 既存の小物を削除
            foreach (Transform child in propsContainer)
            {
                Destroy(child.gameObject);
            }

            // 新しい小物を配置
            foreach (var propPlacement in template.props)
            {
                if (propPlacement.propPrefab != null)
                {
                    GameObject prop = Instantiate(propPlacement.propPrefab, propsContainer);
                    prop.transform.localPosition = propPlacement.position;
                    prop.transform.localRotation = propPlacement.rotation;
                    prop.transform.localScale = propPlacement.scale;
                }
            }
        }

        private void SetupLights()
        {
            if (lightsContainer == null) return;

            // 既存の照明を削除
            foreach (Transform child in lightsContainer)
            {
                Destroy(child.gameObject);
            }

            // 新しい照明を配置
            foreach (var lightSettings in template.lights)
            {
                GameObject lightObj = new GameObject($"Light_{lightSettings.type}");
                lightObj.transform.SetParent(lightsContainer);
                lightObj.transform.localPosition = lightSettings.position;
                lightObj.transform.localRotation = lightSettings.rotation;

                Light light = lightObj.AddComponent<Light>();
                light.type = lightSettings.type;
                light.color = lightSettings.color;
                light.intensity = lightSettings.intensity;
                light.range = lightSettings.range;

                if (lightSettings.type == LightType.Spot)
                {
                    light.spotAngle = lightSettings.spotAngle;
                }
            }

            // 環境光の設定
            RenderSettings.ambientLight = template.ambientLightColor;
            RenderSettings.ambientIntensity = template.ambientIntensity;
        }

        private void SetupAmbientSound()
        {
            if (audioController != null && template.ambientSound != null)
            {
                audioController.SetSound(template.ambientSound);
            }
        }
    }
} 