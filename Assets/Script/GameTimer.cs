using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameTimer : MonoBehaviour
{
    // Durata della partita in secondi
    public float gameDuration = 60f;
    private float timeLeft;

    // Riferimenti al canvas di game over e al testo che mostra il punteggio finale
    public GameObject gameOverCanvas;
    public TextMeshProUGUI finalScoreText;

    // Riferimento al gestore del punteggio
    public ScoreManager scoreManager;

    // Riferimento al testo UI che mostra il tempo residuo durante la partita
    public TextMeshProUGUI timerText;

    void Start()
    {
        Time.timeScale = 1f;
        timeLeft = gameDuration;
        // Nascondi il canvas di game over all'inizio
        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);
    }

    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;

            // Calcola minuti e secondi
            int minutes = Mathf.FloorToInt(timeLeft / 60f);
            int seconds = Mathf.FloorToInt(timeLeft % 60f);

            // Aggiorna il testo del timer nel formato mm:ss
            if (timerText != null)
            {
                timerText.text = minutes + ":" + seconds.ToString("00");
                // In alternativa: timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
            }
        }
        else
        {
            EndGame();
        }
    }


    void EndGame()
    {
        // Imposta il tempo di gioco a 0 per fermare gli aggiornamenti
        Time.timeScale = 0f;

        // Attiva il canvas di game over
        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(true);

        // Aggiorna il testo con il punteggio finale
        if (finalScoreText != null && scoreManager != null)
            finalScoreText.text = "" + scoreManager.GetTotalScore();
    }
}
