using UnityEngine;

public class AudioMarker : MonoBehaviour
{
    [SerializeField] private int wordIndex;
    [SerializeField] private bool isWordA = true;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                LocationProfile profile = GameManager.Instance.GetCurrentProfile();
                if (profile != null)
                {
                    AudioClip clip = isWordA ? profile.wordClipA : profile.wordClipB;
                    GameManager.Instance.PlayWord(clip);
                    GameManager.Instance.GetComponent<Logger>().LogEvent("WordPlayed", 
                        GameManager.Instance.GetCurrentLocationIndex(), 
                        wordIndex);
                }
            }
        }
    }
} 