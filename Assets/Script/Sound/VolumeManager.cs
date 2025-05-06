using UnityEngine;
using UnityEngine.SceneManagement;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance { get; private set; }

    [Header("Volume Settings")]
    public bool musicOn = true;

    [Header("Persistent Music Object (Optional)")]
    public string persistentMusicObjectName = "PersistentMusic";
    private AudioSource persistentMusicSource;

    private const string MusicOnKey = "MusicOn";
    private GameObject initialMusicObject;
    private AudioSource initialMusicSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Trova l'oggetto "Music" all'avvio
            initialMusicObject = GameObject.Find("Music");
            if (initialMusicObject != null)
            {
                initialMusicSource = initialMusicObject.GetComponent<AudioSource>();
                if (initialMusicSource == null)
                {
                    Debug.LogWarning("MusicManager Awake: GameObject 'Music' trovato ma non ha un componente AudioSource.");
                }
                // L'AudioSource "Music" dovrebbe essere disabilitato nell'Inspector
            }
            else
            {
                Debug.LogWarning("MusicManager Awake: GameObject 'Music' non trovato nella prima scena.");
            }

            musicOn = PlayerPrefs.GetInt(MusicOnKey, 1) == 1;
            Debug.Log("MusicManager Awake: musicOn caricato da PlayerPrefs: " + musicOn);

            FindPersistentMusicSource();
            UpdateAudioSources();

            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("MusicManager Awake: Iscritto a SceneManager.sceneLoaded.");
        }
        else
        {
            Debug.Log("MusicManager Awake: Trovata un'altra istanza, distrutta.");
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt(MusicOnKey, musicOn ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("MusicManager OnDestroy: Salvato musicOn in PlayerPrefs: " + musicOn);

        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("MusicManager OnDestroy: Disiscritto da SceneManager.sceneLoaded.");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("MusicManager OnSceneLoaded: Scena caricata: " + scene.name);
        StartCoroutine(UpdateAudioSourcesDelayed());
    }

    private System.Collections.IEnumerator UpdateAudioSourcesDelayed()
    {
        Debug.Log("MusicManager UpdateAudioSourcesDelayed: Avviato nella scena: " + SceneManager.GetActiveScene().name);
        yield return null; // Aspetta un frame
        FindPersistentMusicSource();
        UpdateAudioSources();
    }

    private void FindPersistentMusicSource()
    {
        GameObject musicObject = GameObject.Find(persistentMusicObjectName);
        if (musicObject != null)
        {
            persistentMusicSource = musicObject.GetComponent<AudioSource>();
            if (persistentMusicSource == null)
            {
                Debug.LogError($"MusicManager FindPersistentMusicSource: GameObject '{persistentMusicObjectName}' trovato ma non ha un componente AudioSource!");
            }
            else
            {
                Debug.Log("MusicManager FindPersistentMusicSource: Trovato persistentMusicSource.");
            }
        }
        else
        {
            persistentMusicSource = null;
            Debug.LogWarning($"MusicManager FindPersistentMusicSource: GameObject per la musica persistente '{persistentMusicObjectName}' non trovato nella scena.");
        }
    }

    public void ToggleMusic()
    {
        musicOn = !musicOn;
        Debug.Log("MusicManager ToggleMusic: musicOn impostato a: " + musicOn);
        UpdateAudioSources();
    }

    private void UpdateAudioSources()
    {
        Debug.Log("MusicManager UpdateAudioSources: Chiamato nella scena: " + SceneManager.GetActiveScene().name + " - musicOn: " + musicOn);
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        Debug.Log("MusicManager UpdateAudioSources: Trovati " + allAudioSources.Length + " AudioSource nella scena.");
        foreach (AudioSource audioSource in allAudioSources)
        {
            AudioSourceType audioSourceType = audioSource.GetComponent<AudioSourceType>();
            if (audioSourceType != null)
            {
                Debug.Log("MusicManager UpdateAudioSources: Trovato AudioSource con tipo: " + audioSourceType.audioType + " - Stato attuale: " + audioSource.enabled);
                if (audioSourceType.audioType == AudioSourceType.AudioType.Music)
                {
                    audioSource.enabled = musicOn;
                    Debug.Log("MusicManager UpdateAudioSources: Impostato stato musica (tag) a: " + audioSource.enabled);
                }
            }
            else if (audioSource == persistentMusicSource)
            {
                Debug.Log("MusicManager UpdateAudioSources: Trovato persistentMusicSource - Stato attuale: " + audioSource.enabled);
                audioSource.enabled = musicOn;
                Debug.Log("MusicManager UpdateAudioSources: Impostato stato musica persistente a: " + audioSource.enabled);
            }
            else if (audioSource == initialMusicSource)
            {
                Debug.Log("MusicManager UpdateAudioSources: Trovato initialMusicSource - Stato attuale: " + audioSource.enabled);
                audioSource.enabled = musicOn;
                Debug.Log("MusicManager UpdateAudioSources: Impostato stato 'Music' iniziale a: " + audioSource.enabled);
            }
        }
    }
}