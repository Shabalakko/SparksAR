using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Riferimento al Canvas del menu di pausa
    private bool isPaused = false;

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Ferma il tempo di gioco
        pauseMenuUI.SetActive(true); // Mostra il menu di pausa
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Riporta il tempo alla normalità
        pauseMenuUI.SetActive(false); // Nasconde il menu di pausa
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f; // Assicurati che il tempo sia normale prima di cambiare scena
        SceneManager.LoadScene("MainMenu"); // Cambia scena (usa il nome corretto della tua scena)
    }
}
