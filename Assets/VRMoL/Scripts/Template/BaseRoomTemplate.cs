using UnityEngine;

namespace VRMoL.Template
{
    public class BaseRoomTemplate : MonoBehaviour
    {
        [Header("Room Settings")]
        public float roomWidth = 5f;
        public float roomLength = 5f;
        public float roomHeight = 3f;

        [Header("Materials")]
        public Material wallMaterial;
        public Material floorMaterial;
        public Material ceilingMaterial;

        [Header("Audio")]
        public AudioClip ambientSound;
        public float ambientVolume = 0.5f;

        private void Start()
        {
            InitializeRoom(gameObject);
        }

        public virtual void InitializeRoom(GameObject room)
        {
            // 部屋の基本構造を作成
            CreateWalls(room);
            CreateFloor(room);
            CreateCeiling(room);
            CreateDoor(room);
            CreateWindow(room);

            // 環境音の設定
            SetupAmbientSound(room);
        }

        protected virtual void CreateWalls(GameObject room)
        {
            // 壁の作成
            GameObject walls = new GameObject("Walls");
            walls.transform.SetParent(room.transform);

            // 前の壁
            CreateWall(walls, new Vector3(0, roomHeight/2, roomLength/2), new Vector3(roomWidth, roomHeight, 0.1f));
            // 後ろの壁
            CreateWall(walls, new Vector3(0, roomHeight/2, -roomLength/2), new Vector3(roomWidth, roomHeight, 0.1f));
            // 左の壁
            CreateWall(walls, new Vector3(-roomWidth/2, roomHeight/2, 0), new Vector3(0.1f, roomHeight, roomLength));
            // 右の壁
            CreateWall(walls, new Vector3(roomWidth/2, roomHeight/2, 0), new Vector3(0.1f, roomHeight, roomLength));
        }

        protected virtual void CreateWall(GameObject parent, Vector3 position, Vector3 scale)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "Wall";
            wall.transform.SetParent(parent.transform);
            wall.transform.localPosition = position;
            wall.transform.localScale = scale;

            // マテリアルの設定
            MeshRenderer renderer = wall.GetComponent<MeshRenderer>();
            if (renderer != null && wallMaterial != null)
            {
                renderer.material = wallMaterial;
            }
        }

        protected virtual void CreateFloor(GameObject room)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.transform.SetParent(room.transform);
            floor.transform.localPosition = new Vector3(0, 0, 0);
            floor.transform.localScale = new Vector3(roomWidth, 0.1f, roomLength);

            // マテリアルの設定
            MeshRenderer renderer = floor.GetComponent<MeshRenderer>();
            if (renderer != null && floorMaterial != null)
            {
                renderer.material = floorMaterial;
            }
        }

        protected virtual void CreateCeiling(GameObject room)
        {
            GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ceiling.name = "Ceiling";
            ceiling.transform.SetParent(room.transform);
            ceiling.transform.localPosition = new Vector3(0, roomHeight, 0);
            ceiling.transform.localScale = new Vector3(roomWidth, 0.1f, roomLength);

            // マテリアルの設定
            MeshRenderer renderer = ceiling.GetComponent<MeshRenderer>();
            if (renderer != null && ceilingMaterial != null)
            {
                renderer.material = ceilingMaterial;
            }
        }

        protected virtual void CreateDoor(GameObject room)
        {
            // ドアの作成（後で実装）
        }

        protected virtual void CreateWindow(GameObject room)
        {
            // 窓の作成（後で実装）
        }

        protected virtual void SetupAmbientSound(GameObject room)
        {
            if (ambientSound != null)
            {
                AudioSource audioSource = room.AddComponent<AudioSource>();
                audioSource.clip = ambientSound;
                audioSource.loop = true;
                audioSource.volume = ambientVolume;
                audioSource.Play();
            }
        }
    }
} 