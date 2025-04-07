using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{
    public string sceneName = "MainMenu"; // Nome della scena da caricare
    public Button button; // Riferimento al bottone

    void Start()
    {
        // Se il bottone non è stato assegnato, cerca di trovarlo sull'oggetto corrente
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        // Assicura che il bottone sia assegnato
        if (button != null)
        {
            // Aggiunge un listener al click del bottone
            button.onClick.AddListener(LoadScene);
        }
        else
        {
            Debug.LogError("Button not assigned to LoadSceneButton script!");
        }
    }

    void LoadScene()
    {
        // Carica la scena specificata
        SceneManager.LoadScene(sceneName);
    }
}
