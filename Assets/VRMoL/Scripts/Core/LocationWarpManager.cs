using UnityEngine;

namespace VRMoL.Core
{
    public class LocationWarpManager : MonoBehaviour
    {
        [SerializeField] private Transform[] warpPoints; // WarpPoint_1〜10をセット
        [SerializeField] private GameObject xrOrigin;    // OVRCameraRigなど

        private int currentLocationIndex = 0;

        public void WarpToLocation(int index)
        {
            if (index < 0 || index >= warpPoints.Length) return;
            xrOrigin.transform.position = warpPoints[index].position;
            xrOrigin.transform.rotation = warpPoints[index].rotation;
            currentLocationIndex = index;
        }

        public int GetCurrentLocationIndex() => currentLocationIndex + 1;
        public int GetTotalLocations() => warpPoints.Length;
    }
} 