using UnityEngine;

public class HouseBuilder : MonoBehaviour
{
    [Header("House Structure")]
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject stairsPrefab;
    
    [Header("Room Settings")]
    [SerializeField] private float roomWidth = 5f;
    [SerializeField] private float roomHeight = 3f;
    [SerializeField] private float roomDepth = 5f;
    [SerializeField] private float corridorWidth = 2f;
    
    [Header("House Layout")]
    [SerializeField] private int roomsPerFloor = 5;
    [SerializeField] private int floors = 2;
    
    private GameObject houseContainer;
    private GameObject roomContainer;
    
    private void Start()
    {
        Debug.Log("HouseBuilder: Start called");
        if (floorPrefab == null) Debug.LogError("Floor Prefab is not set!");
        if (wallPrefab == null) Debug.LogError("Wall Prefab is not set!");
        if (doorPrefab == null) Debug.LogError("Door Prefab is not set!");
        if (stairsPrefab == null) Debug.LogError("Stairs Prefab is not set!");
        
        BuildHouse();
    }
    
    public void BuildHouse()
    {
        Debug.Log("HouseBuilder: BuildHouse started");
        
        // コンテナの作成
        houseContainer = new GameObject("House");
        roomContainer = new GameObject("RoomContainer");
        roomContainer.transform.SetParent(houseContainer.transform);
        Debug.Log("HouseBuilder: Containers created");
        
        // 1階の作成
        Debug.Log("HouseBuilder: Creating first floor");
        CreateFloor(0);
        // 2階の作成
        Debug.Log("HouseBuilder: Creating second floor");
        CreateFloor(1);
        
        // 階段の配置
        Debug.Log("HouseBuilder: Placing stairs");
        PlaceStairs();
        
        Debug.Log("HouseBuilder: BuildHouse completed");
    }
    
    private void CreateFloor(int floorIndex)
    {
        Debug.Log($"HouseBuilder: Creating floor {floorIndex}");
        float floorY = floorIndex * roomHeight;
        
        // 廊下の作成
        CreateCorridor(floorY);
        
        // 各部屋の作成
        for (int i = 0; i < roomsPerFloor; i++)
        {
            float roomX = i * (roomWidth + corridorWidth);
            CreateRoom(roomX, floorY, i + (floorIndex * roomsPerFloor));
        }
        Debug.Log($"HouseBuilder: Floor {floorIndex} completed");
    }
    
    private void CreateCorridor(float floorY)
    {
        Debug.Log($"HouseBuilder: Creating corridor at Y={floorY}");
        GameObject corridor = new GameObject($"Corridor_Floor_{floorY}");
        corridor.transform.SetParent(houseContainer.transform);
        corridor.transform.position = new Vector3(0, floorY, 0);
        
        // 廊下の床
        GameObject corridorFloor = Instantiate(floorPrefab, corridor.transform);
        corridorFloor.transform.localScale = new Vector3(roomsPerFloor * (roomWidth + corridorWidth), 0.1f, corridorWidth);
        corridorFloor.transform.localPosition = new Vector3(0, -0.05f, 0);
        Debug.Log($"HouseBuilder: Corridor created at Y={floorY}");
    }
    
    private void CreateRoom(float x, float y, int roomIndex)
    {
        Debug.Log($"HouseBuilder: Creating room {roomIndex} at position ({x}, {y})");
        GameObject room = new GameObject($"Room_{roomIndex}");
        room.transform.SetParent(roomContainer.transform);
        room.transform.position = new Vector3(x, y, 0);
        
        // 部屋の床
        GameObject floor = Instantiate(floorPrefab, room.transform);
        floor.transform.localScale = new Vector3(roomWidth, 0.1f, roomDepth);
        floor.transform.localPosition = new Vector3(0, -0.05f, 0);
        
        // 部屋の壁
        CreateWalls(room);
        
        // ドアの配置
        PlaceDoor(room);
        Debug.Log($"HouseBuilder: Room {roomIndex} created");
    }
    
    private void CreateWalls(GameObject room)
    {
        Debug.Log($"HouseBuilder: Creating walls for room {room.name}");
        // 左壁
        GameObject leftWall = Instantiate(wallPrefab, room.transform);
        leftWall.transform.localScale = new Vector3(0.1f, roomHeight, roomDepth);
        leftWall.transform.localPosition = new Vector3(-roomWidth/2, roomHeight/2, 0);
        
        // 右壁
        GameObject rightWall = Instantiate(wallPrefab, room.transform);
        rightWall.transform.localScale = new Vector3(0.1f, roomHeight, roomDepth);
        rightWall.transform.localPosition = new Vector3(roomWidth/2, roomHeight/2, 0);
        
        // 奥の壁
        GameObject backWall = Instantiate(wallPrefab, room.transform);
        backWall.transform.localScale = new Vector3(roomWidth, roomHeight, 0.1f);
        backWall.transform.localPosition = new Vector3(0, roomHeight/2, -roomDepth/2);
        Debug.Log($"HouseBuilder: Walls created for room {room.name}");
    }
    
    private void PlaceDoor(GameObject room)
    {
        Debug.Log($"HouseBuilder: Placing door for room {room.name}");
        GameObject door = Instantiate(doorPrefab, room.transform);
        door.transform.localPosition = new Vector3(0, 0, -roomDepth/2);
        door.transform.localRotation = Quaternion.Euler(0, 0, 0);
        Debug.Log($"HouseBuilder: Door placed for room {room.name}");
    }
    
    private void PlaceStairs()
    {
        Debug.Log("HouseBuilder: Placing stairs");
        GameObject stairs = Instantiate(stairsPrefab, houseContainer.transform);
        stairs.transform.position = new Vector3(-roomWidth/2, 0, -roomDepth/2);
        Debug.Log("HouseBuilder: Stairs placed");
    }
}