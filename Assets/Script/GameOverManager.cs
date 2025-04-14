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

    [Header("Localization Keys")]
    public string finalScoreKey = "final_score";
    public string highScoreNewKey = "high_score_new";
    public string highScoreKey = "high_score";
    public string earnedCoinsKey = "earned_coins";
    public string coinsKey = "coins";
    public string currentCoinsKey = "current_coins";

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
        UpdateLocalizedUI(); // Aggiorna i testi localizzati all'avvio
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
            finalScoreText.text = LanguageManager.instance.GetLocalizedText(finalScoreKey, "Final Score") + ": " + finalScore;

        if (highScoreText != null)
        {
            if (isNewHighScore)
                highScoreText.text = LanguageManager.instance.GetLocalizedText(highScoreNewKey, "New!!!") + " " + LanguageManager.instance.GetLocalizedText(highScoreKey, "High Score") + ": " + currentHighScore;
            else
                highScoreText.text = LanguageManager.instance.GetLocalizedText(highScoreKey, "High Score") + ": " + currentHighScore;
        }
        UpdateCoinsUI();
        if (earnedCoinsText != null)
        {
            earnedCoinsText.text = "+" + earnedCoins + " " + LanguageManager.instance.GetLocalizedText(earnedCoinsKey, "Coins");
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
            coinsText.text = LanguageManager.instance.GetLocalizedText(coinsKey, "Coins") + ": " + WalletManager.Instance.GetTotalCoins();
        }
        if (currentCoinsText != null)
        {
            currentCoinsText.text = LanguageManager.instance.GetLocalizedText(currentCoinsKey, "Current Coins") + ": " + WalletManager.Instance.GetTotalCoins();
        }
    }

    // Metodo per aggiornare tutti i testi localizzati
    public void UpdateLocalizedUI()
    {
        if (finalScoreText != null)
            finalScoreText.text = LanguageManager.instance.GetLocalizedText(finalScoreKey, "Final Score") + ": " + (scoreManager != null ? scoreManager.GetTotalScore() : "0");

        if (highScoreText != null)
        {
            ScoringMode currentMode = (scoreManager != null) ? scoreManager.scoringMode : ScoringMode.ColorSlots; // Imposta un valore di default se scoreManager è null
            int currentHighScore = HighScoreManager.GetHighScore(currentMode);
            // Per ora, rimuoviamo la riga con HasNewHighScore perché non è definita
            bool isNewHighScore = (scoreManager != null) ? HighScoreManager.UpdateHighScore(currentMode, scoreManager.GetTotalScore()) : false; // Tentativo di usare UpdateHighScore come indicatore di nuovo record

            if (isNewHighScore)
                highScoreText.text = LanguageManager.instance.GetLocalizedText(highScoreNewKey, "New!!!") + " " + LanguageManager.instance.GetLocalizedText(highScoreKey, "High Score") + ": " + currentHighScore;
            else
                highScoreText.text = LanguageManager.instance.GetLocalizedText(highScoreKey, "High Score") + ": " + currentHighScore;
        }

        UpdateCoinsUI(); // Assicurati che anche il testo delle monete sia aggiornato

        if (earnedCoinsText != null && scoreManager != null)
        {
            int earnedCoins = CalculateCoins(scoreManager.GetTotalScore());
            earnedCoinsText.text = "+" + earnedCoins + " " + LanguageManager.instance.GetLocalizedText(earnedCoinsKey, "Coins");
        }
    }

    private void OnEnable()
    {
        // Sottoscrivi la funzione UpdateLocalizedUI all'evento di cambio lingua
        if (LanguageManager.instance != null)
        {
            LanguageManager.onLanguageChanged += UpdateLocalizedUI;
        }
    }

    private void OnDisable()
    {
        // Annulla la sottoscrizione per evitare memory leak
        if (LanguageManager.instance != null)
        {
            LanguageManager.onLanguageChanged -= UpdateLocalizedUI;
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