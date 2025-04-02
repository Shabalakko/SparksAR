using System.Collections.Generic;
using UnityEngine;
using System.Collections; // Necessario per le Coroutine

public class IstantiateAudioAtPosition : MonoBehaviour
{
    public GameObject audioPrefab; // Assegna il tuo prefab audio nell'Inspector
    private HashSet<int> destroyedObjectIds = new HashSet<int>(); // Store Instance IDs

    // Metodo per istanziare l'audio come Coroutine
    public IEnumerator InstantiateAudioAtPosition(Vector3 position, int instantiatingObjectId)
    {
        // Controlla se l'oggetto è già stato distrutto.
        if (destroyedObjectIds.Contains(instantiatingObjectId))
        {
            yield break; // Se è già stato distrutto, esci dalla Coroutine.
        }

        Debug.Log("InstantiateAudioAtPosition called with position: " + position);
        if (audioPrefab != null)
        {
            Debug.Log("audioPrefab is not null, instantiating at: " + position);
            GameObject audioObject = Instantiate(audioPrefab, position, Quaternion.identity);
            // Ottieni la lunghezza del clip audio e distruggi l'oggetto dopo
            AudioSource audioSource = audioObject.GetComponent<AudioSource>();
            if (audioSource != null && audioSource.clip != null)
            {
                Debug.Log("audioSource is not null and audioSource.clip is not null");
                // Usa WaitForSecondsRealtime per non essere influenzato da Time.timeScale
                yield return new WaitForSecondsRealtime(audioSource.clip.length);
            }
            else
            {
                Debug.Log("audioSource is null or audioSource.clip is null");
                // Se non c'è AudioSource o clip, distruggi l'oggetto dopo un breve ritardo di sicurezza
                yield return new WaitForSecondsRealtime(5f);
            }
            Destroy(audioObject);
        }
        else
        {
            Debug.Log("audioPrefab is null!");
        }
        destroyedObjectIds.Add(instantiatingObjectId); //memorizza id oggetto distrutto
    }

    private void OnDestroy()
    {
        destroyedObjectIds.Clear();
    }
}
