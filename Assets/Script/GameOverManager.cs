using UnityEngine;
using TMPro;
using System;
using System.Collections; // Necessario per le coroutine

public class GameOverManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverCanvas;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI earnedCoinsText;
    public TextMeshProUGUI currentCoinsText;

    [Header("Score Manager Reference")]
    public ScoreManager scoreManager;

    [Header("Terrain Reference")]
    public GameObject Terrain;

    [Header("Coins Configuration")]
    public int pointsPerCoin = 500;
    private bool coinsAwarded = false;
    public bool resetCoinsOnStart = false; //aggiunto per il reset

    private void Start()
    {
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }
        if (resetCoinsOnStart) //reset all'avvio se la variabile è true
        {
            ResetTotalCoins();
        }
        UpdateCoinsUI();
    }

    public void OnGameOver()
    {
        if (Terrain != null)
        {
            Terrain.SetActive(false);
        }

        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager non assegnato in GameOverManager!");
            return;
        }

        if (coinsAwarded)
            return;

        coinsAwarded = true;

        int finalScore = scoreManager.GetTotalScore();
        int earnedCoins = CalculateCoins(finalScore);
        WalletManager.Instance.AddCoins(earnedCoins);

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
        UpdateCoinsUI();
        if (earnedCoinsText != null)
        {
            earnedCoinsText.text = "+" + earnedCoins + " Coins";
        }

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(true);

        if (AchievementManager.Instance != null)
            AchievementManager.Instance.CheckAchievements(scoreManager.scoringMode, finalScore);
    }

    private int CalculateCoins(int score)
    {
        return score / pointsPerCoin;
    }

    private void UpdateCoinsUI()
    {
        if (coinsText != null)
        {
            coinsText.text = "Coins: " + WalletManager.Instance.GetTotalCoins();
        }
        if (currentCoinsText != null)
        {
            currentCoinsText.text = "Current Coins: " + WalletManager.Instance.GetTotalCoins();
        }
    }

    //Metodo per resettare le monete
    private void ResetTotalCoins()
    {
        PlayerPrefs.SetInt("TotalCoins", 0);
        PlayerPrefs.Save();
        UpdateCoinsUI(); //aggiorna la UI per mostrare 0 monete
        Debug.Log("Coins reset to 0");
    }
}
