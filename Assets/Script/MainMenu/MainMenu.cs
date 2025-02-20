using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuCanvas;  // Canvas del menu principale
    public GameObject optionsCanvas;   // Canvas delle opzioni di gioco

    // --- 1. Play Button ---
    public void PlayGameGyro()
    {
        // Carica la scena di gioco
        SceneManager.LoadScene("GyroLevel");
    }

    public void PlayGameTouch()
    {
        SceneManager.LoadScene("TouchLevel");
    }

    // --- 2. Options Button ---
    public void OpenOptions()
    {
        // Mostra il Canvas delle opzioni
        optionsCanvas.SetActive(true);

        // Nasconde il Canvas del menu principale
        mainMenuCanvas.SetActive(false);
    }

    // --- 3. Exit Button ---
    public void ExitGame()
    {
        // Chiude l'applicazione su Android e PC
        Application.Quit();

        // Ferma l'editor di Unity (solo per test)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // --- Torna al Menu Principale dalle Opzioni ---
    public void BackToMenu()
    {
        // Mostra il Canvas del menu principale
        mainMenuCanvas.SetActive(true);

        // Nasconde il Canvas delle opzioni
        optionsCanvas.SetActive(false);
    }
}
