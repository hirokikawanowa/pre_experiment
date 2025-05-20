using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public bool conditionSpatial = false;  // toggled per session
    private LocationProfile[] profiles;
    private int currentLocationIndex = 0;

    [SerializeField] private AudioSource ambient;
    [SerializeField] private AudioSource words;

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

    void Start()
    {
        profiles = Resources.LoadAll<LocationProfile>("Profiles");
        StartCoroutine(LoadLocationScene(currentLocationIndex));
    }

    IEnumerator LoadLocationScene(int i)
    {
        yield return Addressables.LoadSceneAsync($"Location_{i}", LoadSceneMode.Single);
        SetupAudio(profiles[i]);
    }

    void SetupAudio(LocationProfile p)
    {
        ambient.clip = p.ambientClip;
        ambient.loop = true;
        ambient.spatialBlend = 1;  // always spatial
        ambient.Play();
    }

    public void PlayWord(AudioClip clip)
    {
        words.clip = clip;
        words.spatialize = conditionSpatial;
        words.spatialBlend = conditionSpatial ? 1 : 0;
        words.Play();
    }

    public LocationProfile GetCurrentProfile()
    {
        if (profiles != null && currentLocationIndex < profiles.Length)
        {
            return profiles[currentLocationIndex];
        }
        return null;
    }

    public int GetCurrentLocationIndex()
    {
        return currentLocationIndex;
    }
} 