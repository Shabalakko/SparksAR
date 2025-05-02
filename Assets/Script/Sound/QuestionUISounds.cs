using UnityEngine;

public class PlaySoundsOnActivation : MonoBehaviour
{
    public AudioClip activationSound;
    public AudioClip deactivationSound;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnEnable()
    {
        if (activationSound != null)
        {
            audioSource.PlayOneShot(activationSound);
        }
    }

    void OnDisable()
    {
        Debug.Log("OnDisable è stato chiamato per l'oggetto: " + gameObject.name);
        if (deactivationSound != null)
        {
            audioSource.PlayOneShot(deactivationSound);
        }
    }
}