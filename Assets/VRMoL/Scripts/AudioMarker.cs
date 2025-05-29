using UnityEngine;

public class AudioMarker : MonoBehaviour
{
    private AudioClip wordClip;
    private bool isWordA;
    
    public void Setup(AudioClip clip, bool isA)
    {
        wordClip = clip;
        isWordA = isA;
        
        // コライダーの設定
        BoxCollider collider = gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(1, 1, 1);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.PlayWord(wordClip, isWordA);
            }
        }
    }
} 