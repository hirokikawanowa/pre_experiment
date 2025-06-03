using UnityEngine;

namespace VRMoL.Template
{
    public class RoomGenerator : MonoBehaviour
    {
        [Header("Room Generation Settings")]
        public float wallHeight = 3f;
        public float wallThickness = 0.2f;
        public Material defaultWallMaterial;
        public Material defaultFloorMaterial;
        public Material defaultCeilingMaterial;

        public GameObject GenerateRoom(float width, float length, float height)
        {
            // 部屋のルートオブジェクトを作成
            GameObject room = new GameObject("Room");
            room.transform.position = Vector3.zero;

            // 床を生成
            GameObject floor = CreateFloor(width, length);
            floor.transform.SetParent(room.transform);

            // 壁を生成
            GameObject walls = CreateWalls(width, length, height);
            walls.transform.SetParent(room.transform);

            // 天井を生成
            GameObject ceiling = CreateCeiling(width, length);
            ceiling.transform.SetParent(room.transform);

            return room;
        }

        private GameObject CreateFloor(float width, float length)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.transform.localScale = new Vector3(width, wallThickness, length);
            floor.transform.position = new Vector3(0, -wallThickness/2, 0);

            if (defaultFloorMaterial != null)
            {
                floor.GetComponent<Renderer>().material = defaultFloorMaterial;
            }

            return floor;
        }

        private GameObject CreateWalls(float width, float length, float height)
        {
            GameObject walls = new GameObject("Walls");

            // 4面の壁を生成
            CreateWall(walls, new Vector3(0, height/2, length/2), new Vector3(width, height, wallThickness)); // 前の壁
            CreateWall(walls, new Vector3(0, height/2, -length/2), new Vector3(width, height, wallThickness)); // 後ろの壁
            CreateWall(walls, new Vector3(width/2, height/2, 0), new Vector3(wallThickness, height, length)); // 右の壁
            CreateWall(walls, new Vector3(-width/2, height/2, 0), new Vector3(wallThickness, height, length)); // 左の壁

            return walls;
        }

        private void CreateWall(GameObject parent, Vector3 position, Vector3 scale)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "Wall";
            wall.transform.SetParent(parent.transform);
            wall.transform.localPosition = position;
            wall.transform.localScale = scale;

            if (defaultWallMaterial != null)
            {
                wall.GetComponent<Renderer>().material = defaultWallMaterial;
            }
        }

        private GameObject CreateCeiling(float width, float length)
        {
            GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ceiling.name = "Ceiling";
            ceiling.transform.localScale = new Vector3(width, wallThickness, length);
            ceiling.transform.position = new Vector3(0, wallHeight + wallThickness/2, 0);

            if (defaultCeilingMaterial != null)
            {
                ceiling.GetComponent<Renderer>().material = defaultCeilingMaterial;
            }

            return ceiling;
        }
    }
} 