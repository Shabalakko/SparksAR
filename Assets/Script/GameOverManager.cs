using UnityEngine;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverCanvas;       // Pannello Game Over (da disattivare all'inizio)
    public TextMeshProUGUI finalScoreText;    // Testo per il punteggio finale
    public TextMeshProUGUI highScoreText;     // Testo per l'highscore

    [Header("Score Manager Reference")]
    public ScoreManager scoreManager;         // Riferimento allo ScoreManager

    public void OnGameOver()
    {
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager non assegnato in GameOverManager!");
            return;
        }

        int finalScore = scoreManager.GetTotalScore();

        // Aggiorna l'highscore per la modalità corrente
        bool isNewHighScore = HighScoreManager.UpdateHighScore(scoreManager.scoringMode, finalScore);
        int currentHighScore = HighScoreManager.GetHighScore(scoreManager.scoringMode);

        if (finalScoreText != null)
            finalScoreText.text = "" + finalScore;

        if (highScoreText != null)
        {
            if (isNewHighScore)
                highScoreText.text = "New!!! " + currentHighScore;
            else
                highScoreText.text = "" + currentHighScore;
        }

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(true);

        // Controlla eventuali achievement basati sull'highscore
        if (AchievementManager.Instance != null)
            AchievementManager.Instance.CheckAchievements(scoreManager.scoringMode, finalScore);

    }
}
