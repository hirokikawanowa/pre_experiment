using UnityEngine;

namespace VRMoL.Template
{
    public class JapaneseStudyTemplate : BaseRoomTemplate
    {
        [Header("Japanese Study Specific")]
        public GameObject tatamiPrefab;
        public GameObject shojiPrefab;
        public GameObject writingDeskPrefab;
        public GameObject scrollPrefab;
        public AudioClip rainSound;
        public AudioClip windChimeSound;
        public AudioClip gardenWaterSound;

        public override void InitializeRoom(GameObject room)
        {
            base.InitializeRoom(room);

            // 畳の配置
            if (tatamiPrefab != null)
            {
                // 畳を6枚配置（2x3）
                float tatamiWidth = 0.91f;  // 標準的な畳のサイズ
                float tatamiLength = 1.82f;
                
                for (int x = 0; x < 2; x++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        Vector3 position = new Vector3(
                            (x - 0.5f) * tatamiWidth,
                            0,
                            (z - 1) * tatamiLength
                        );
                        GameObject tatami = Instantiate(tatamiPrefab, position, Quaternion.identity);
                        tatami.transform.SetParent(room.transform);
                    }
                }
            }

            // 障子の配置
            if (shojiPrefab != null)
            {
                // 前の壁に障子を配置
                GameObject shoji = Instantiate(shojiPrefab, new Vector3(0, 1.5f, 2.4f), Quaternion.identity);
                shoji.transform.SetParent(room.transform);
            }

            // 文机の配置
            if (writingDeskPrefab != null)
            {
                GameObject desk = Instantiate(writingDeskPrefab, new Vector3(0, 0, 1.5f), Quaternion.identity);
                desk.transform.SetParent(room.transform);
            }

            // 掛け軸の配置
            if (scrollPrefab != null)
            {
                GameObject scroll = Instantiate(scrollPrefab, new Vector3(0, 1.8f, -2.3f), Quaternion.identity);
                scroll.transform.SetParent(room.transform);
            }

            // 環境音の設定
            AudioSource audioSource = room.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                // 雨音をメインの環境音として設定
                if (rainSound != null)
                {
                    audioSource.clip = rainSound;
                    audioSource.loop = true;
                    audioSource.Play();
                }

                // 風鈴と庭の水音は別のAudioSourceで再生
                if (windChimeSound != null)
                {
                    AudioSource windChimeSource = room.AddComponent<AudioSource>();
                    windChimeSource.clip = windChimeSound;
                    windChimeSource.loop = true;
                    windChimeSource.volume = 0.3f;
                    windChimeSource.Play();
                }

                if (gardenWaterSound != null)
                {
                    AudioSource waterSource = room.AddComponent<AudioSource>();
                    waterSource.clip = gardenWaterSound;
                    waterSource.loop = true;
                    waterSource.volume = 0.2f;
                    waterSource.Play();
                }
            }
        }
    }
} 