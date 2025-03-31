using UnityEngine;

public class CesiumGeoreferenceController : MonoBehaviour
{
    void Start()
    {
        // Legge lo stato salvato e applica SetActive
        bool isActive = PlayerPrefs.GetInt("CesiumGeoreferenceActive", 1) == 1;
        gameObject.SetActive(isActive);

        Debug.Log("CesiumGeoreference 1 stato caricato: " + isActive);
    }
}
