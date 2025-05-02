using UnityEngine;
using UnityEngine.SceneManagement;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance { get; private set; }

    public bool musicOn = true;
    public bool effectsOn = true;

    private const string MusicOnKey = "MusicOn";
    private const string EffectsOnKey = "EffectsOn";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            musicOn = PlayerPrefs.GetInt(MusicOnKey, 1) == 1;
            effectsOn = PlayerPrefs.GetInt(EffectsOnKey, 1) == 1;

            UpdateAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt(MusicOnKey, musicOn ? 1 : 0);
        PlayerPrefs.SetInt(EffectsOnKey, effectsOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleMusic()
    {
        musicOn = !musicOn;
        UpdateAudioSources();
    }

    public void ToggleEffects()
    {
        effectsOn = !effectsOn;
        UpdateAudioSources();
    }

    private void UpdateAudioSources()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource.GetComponent<AudioSourceType>() != null)
            {
                if (audioSource.GetComponent<AudioSourceType>().audioType == AudioSourceType.AudioType.Music)
                {
                    audioSource.volume = musicOn ? 1f : 0f;
                }
                else if (audioSource.GetComponent<AudioSourceType>().audioType == AudioSourceType.AudioType.Effect)
                {
                    audioSource.volume = effectsOn ? 1f : 0f;
                }
            }
        }
    }
}