using UnityEngine;
using UnityEngine.SceneManagement;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance { get; private set; }

    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float effectsVolume = 1f;

    private const string MusicVolumeKey = "MusicVolume";
    private const string EffectsVolumeKey = "EffectsVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
            effectsVolume = PlayerPrefs.GetFloat(EffectsVolumeKey, 1f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        PlayerPrefs.SetFloat(EffectsVolumeKey, effectsVolume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        UpdateAudioSources();
    }

    public void SetEffectsVolume(float volume)
    {
        effectsVolume = volume;
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
                    audioSource.volume = musicVolume;
                }
                else if (audioSource.GetComponent<AudioSourceType>().audioType == AudioSourceType.AudioType.Effect)
                {
                    audioSource.volume = effectsVolume;
                }
            }
        }
    }
}