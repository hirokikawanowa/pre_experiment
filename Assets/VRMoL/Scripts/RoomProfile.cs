using UnityEngine;

[CreateAssetMenu(menuName="VRMoL/RoomProfile")]
public class RoomProfile : ScriptableObject 
{
    public string roomName;
    public string roomDescription;
    public AudioClip ambientClip;
    public AudioClip wordClipA;
    public AudioClip wordClipB;
    
    [Header("Room Position")]
    public Vector3 roomPosition;
    public Vector3 roomRotation;
    
    [Header("Room Objects")]
    public GameObject[] roomObjects;
    public GameObject[] roomDecorations;
} 