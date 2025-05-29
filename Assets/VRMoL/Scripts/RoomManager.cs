using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }
    
    [SerializeField] private RoomProfile[] roomProfiles;
    [SerializeField] private GameObject roomContainer;
    [SerializeField] private AudioSource ambientAudio;
    [SerializeField] private AudioSource wordAudio;
    
    private Dictionary<int, GameObject> activeRooms = new Dictionary<int, GameObject>();
    private int currentRoomIndex = -1;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        InitializeRooms();
    }
    
    private void InitializeRooms()
    {
        foreach (var profile in roomProfiles)
        {
            CreateRoom(profile);
        }
    }
    
    private void CreateRoom(RoomProfile profile)
    {
        GameObject room = new GameObject(profile.roomName);
        room.transform.SetParent(roomContainer.transform);
        room.transform.localPosition = profile.roomPosition;
        room.transform.localRotation = Quaternion.Euler(profile.roomRotation);
        
        // 部屋のオブジェクトを配置
        foreach (var obj in profile.roomObjects)
        {
            if (obj != null)
            {
                Instantiate(obj, room.transform);
            }
        }
        
        // 装飾を配置
        foreach (var decoration in profile.roomDecorations)
        {
            if (decoration != null)
            {
                Instantiate(decoration, room.transform);
            }
        }
        
        // 音声マーカーを配置
        CreateAudioMarkers(room, profile);
        
        activeRooms.Add(activeRooms.Count, room);
    }
    
    private void CreateAudioMarkers(GameObject room, RoomProfile profile)
    {
        // Word A マーカー
        GameObject markerA = new GameObject("WordMarker_A");
        markerA.transform.SetParent(room.transform);
        markerA.transform.localPosition = new Vector3(-1, 1, 0);
        var audioMarkerA = markerA.AddComponent<AudioMarker>();
        audioMarkerA.Setup(profile.wordClipA, true);
        
        // Word B マーカー
        GameObject markerB = new GameObject("WordMarker_B");
        markerB.transform.SetParent(room.transform);
        markerB.transform.localPosition = new Vector3(1, 1, 0);
        var audioMarkerB = markerB.AddComponent<AudioMarker>();
        audioMarkerB.Setup(profile.wordClipB, false);
    }
    
    public void EnterRoom(int roomIndex)
    {
        if (roomIndex >= 0 && roomIndex < activeRooms.Count)
        {
            currentRoomIndex = roomIndex;
            RoomProfile profile = roomProfiles[roomIndex];
            
            // アンビエント音声を設定
            ambientAudio.clip = profile.ambientClip;
            ambientAudio.Play();
            
            // ログに記録
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GetComponent<Logger>().LogEvent("RoomEntered", roomIndex);
            }
        }
    }

    public void PlayWord(AudioClip clip, bool isWordA)
    {
        if (wordAudio != null)
        {
            wordAudio.clip = clip;
            wordAudio.Play();
            
            // ログに記録
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GetComponent<Logger>().LogEvent("WordPlayed", 
                    currentRoomIndex, 
                    isWordA ? 0 : 1);
            }
        }
    }
} 