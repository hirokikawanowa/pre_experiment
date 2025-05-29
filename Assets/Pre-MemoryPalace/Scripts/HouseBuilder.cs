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
    
    public void BuildHouse()
    {
        // コンテナの作成
        houseContainer = new GameObject("House");
        roomContainer = new GameObject("RoomContainer");
        roomContainer.transform.SetParent(houseContainer.transform);
        
        // 1階の作成
        CreateFloor(0);
        // 2階の作成
        CreateFloor(1);
        
        // 階段の配置
        PlaceStairs();
    }
    
    private void CreateFloor(int floorIndex)
    {
        float floorY = floorIndex * roomHeight;
        
        // 廊下の作成
        CreateCorridor(floorY);
        
        // 各部屋の作成
        for (int i = 0; i < roomsPerFloor; i++)
        {
            float roomX = i * (roomWidth + corridorWidth);
            CreateRoom(roomX, floorY, i + (floorIndex * roomsPerFloor));
        }
    }
    
    private void CreateCorridor(float floorY)
    {
        GameObject corridor = new GameObject($"Corridor_Floor_{floorY}");
        corridor.transform.SetParent(houseContainer.transform);
        corridor.transform.position = new Vector3(0, floorY, 0);
        
        // 廊下の床
        GameObject corridorFloor = Instantiate(floorPrefab, corridor.transform);
        corridorFloor.transform.localScale = new Vector3(roomsPerFloor * (roomWidth + corridorWidth), 0.1f, corridorWidth);
        corridorFloor.transform.localPosition = new Vector3(0, -0.05f, 0);
    }
    
    private void CreateRoom(float x, float y, int roomIndex)
    {
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
    }
    
    private void CreateWalls(GameObject room)
    {
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
    }
    
    private void PlaceDoor(GameObject room)
    {
        GameObject door = Instantiate(doorPrefab, room.transform);
        door.transform.localPosition = new Vector3(0, 0, -roomDepth/2);
        door.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
    
    private void PlaceStairs()
    {
        GameObject stairs = Instantiate(stairsPrefab, houseContainer.transform);
        stairs.transform.position = new Vector3(-roomWidth/2, 0, -roomDepth/2);
    }
}