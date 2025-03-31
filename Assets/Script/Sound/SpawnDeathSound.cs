using UnityEngine;

public class SpawnDeathSound : MonoBehaviour
{
    [SerializeField]
    private GameObject _soundPrefab; // Trascinaci un prefab con un AudioSource nell'Inspector

    // Funzione per far partire il suono. Dovrebbe essere chiamata da chi distrugge il nemico.
    public void PlaySound()
    {
        if (_soundPrefab != null)
        {
            // Istanzia il prefab del suono nella posizione del nemico che sta morendo.
            GameObject soundObject = Instantiate(_soundPrefab, transform.position, transform.rotation);

            // Ottieni l'AudioSource dal prefab istanziato.
            AudioSource audioSource = soundObject.GetComponent<AudioSource>();

            if (audioSource != null && audioSource.clip != null)
            {
                // Riproduci il suono.
                audioSource.Play();

                // Distruggi il GameObject dopo che il suono è finito.
                Destroy(soundObject, audioSource.clip.length);
            }
            else
            {
                Debug.LogWarning("AudioSource o Audio Clip non trovato nel prefab del suono!", soundObject);
                Destroy(soundObject); // Distruggi subito se manca l'AudioSource o il clip.
            }
        }
        else
        {
            Debug.LogError("Sound Prefab is not assigned!", gameObject);
        }
    }
}
