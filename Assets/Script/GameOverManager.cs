using UnityEngine;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverCanvas;      // Pannello Game Over
    public TextMeshProUGUI finalScoreText;   // Testo per mostrare il punteggio finale
    public TextMeshProUGUI highScoreText;    // Testo per mostrare l'HighScore

    [Header("Score Manager Reference")]
    public ScoreManager scoreManager;

    public void OnGameOver()
    {
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager non assegnato in GameOverManager!");
            return;
        }

        // Calcola il punteggio finale
        int finalScore = scoreManager.GetTotalScore();

        // Aggiorna il final score nella UI
        if (finalScoreText != null)
            finalScoreText.text = finalScore.ToString();

        // Aggiorna l'HighScore per la modalità corrente
        bool newHighScore = HighScoreManager.UpdateHighScore(scoreManager.scoringMode, finalScore);
        int highScore = HighScoreManager.GetHighScore(scoreManager.scoringMode);
        if (highScoreText != null)
        {
            if (newHighScore)
                highScoreText.text = "New!!! " + highScore;
            else
                highScoreText.text = "" + highScore;
        }

        // Mostra il pannello di Game Over
        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(true);

        // Verifica e aggiorna gli achievement basati sull'HighScore
        if (AchievementManager.Instance != null)
            AchievementManager.Instance.CheckAchievements(scoreManager.scoringMode);
    }
}
