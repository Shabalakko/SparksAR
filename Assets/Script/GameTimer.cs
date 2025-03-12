using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float gameDuration = 60f;
    private float timeLeft;

    public GameObject gameOverCanvas;
    public TextMeshProUGUI timerText;

    public ScoreManager scoreManager;
    public GameOverManager gameOverManager;  // Riferimento al GameOverManager

    void Start()
    {
        Time.timeScale = 1f;
        timeLeft = gameDuration;
        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);
    }

    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(timeLeft / 60f);
            int seconds = Mathf.FloorToInt(timeLeft % 60f);

            if (timerText != null)
                timerText.text = minutes + ":" + seconds.ToString("00");
        }
        else
        {
            EndGame();
        }
    }

    void EndGame()
    {
        timerText.text = "0:00";
        Time.timeScale = 0f;

        // Chiama il GameOverManager per gestire la fine partita
        if (gameOverManager != null)
        {
            gameOverManager.OnGameOver();
        }
    }
}
