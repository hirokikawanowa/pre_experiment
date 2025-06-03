using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Meta.XR;

namespace VRMoL.Core
{
    public class LocationManager : MonoBehaviour
    {
        [SerializeField] private string initialLocationLabel = "Location_01";
        [SerializeField] private Transform playerSpawnPoint;

        private GameObject currentLocation;
        private int currentLocationIndex = 0;
        private const int TOTAL_LOCATIONS = 10;

        public void LoadInitialLocation()
        {
            LoadLocation(initialLocationLabel);
        }

        public void LoadNextLocation()
        {
            if (currentLocationIndex < TOTAL_LOCATIONS)
            {
                currentLocationIndex++;
                string locationLabel = $"Location_{currentLocationIndex:D2}";
                LoadLocation(locationLabel);
            }
        }

        public void LoadPreviousLocation()
        {
            if (currentLocationIndex > 1)
            {
                currentLocationIndex--;
                string locationLabel = $"Location_{currentLocationIndex:D2}";
                LoadLocation(locationLabel);
            }
        }

        private async void LoadLocation(string locationLabel)
        {
            if (currentLocation != null)
            {
                Destroy(currentLocation);
            }

            var handle = Addressables.LoadSceneAsync(locationLabel);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // SceneInstanceからルートオブジェクトを取得
                currentLocation = handle.Result.Scene.GetRootGameObjects()[0];
                
                if (playerSpawnPoint != null)
                {
                    // プレイヤーの位置をリセット
                    var player = FindObjectOfType<OVRCameraRig>();
                    if (player != null)
                    {
                        player.transform.position = playerSpawnPoint.position;
                        player.transform.rotation = playerSpawnPoint.rotation;
                    }
                }
            }
            else
            {
                Debug.LogError($"Failed to load location: {locationLabel}");
            }
        }

        public int GetCurrentLocationIndex()
        {
            return currentLocationIndex;
        }

        public int GetTotalLocations()
        {
            return TOTAL_LOCATIONS;
        }
    }
} 