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

    [Header("Laser Management")]
    public GameObject Lasers;
    public GameObject PLaserR, PLaserB;

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
            finalScoreText.text = LanguageManager.instance.GetLocalizedText(finalScoreKey, "") + "" + finalScore;

        if (highScoreText != null)
        {
            if (isNewHighScore)
                highScoreText.text = "" + currentHighScore;
            else
                highScoreText.text = "" + currentHighScore;
        }
        UpdateCoinsUI();
        if (earnedCoinsText != null)
        {
            earnedCoinsText.text = "+" + earnedCoins; // Rimossa la localizzazione e la parola "Coins"
        }

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(true);

        DisableLasers(); // Disattiva i laser quando il game over viene mostrato

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
            coinsText.text = WalletManager.Instance.GetTotalCoins().ToString(); // Mostra solo il numero
        }
        if (currentCoinsText != null)
        {
            currentCoinsText.text = WalletManager.Instance.GetTotalCoins().ToString(); // Mostra solo il numero
        }
    }

    // Metodo per aggiornare tutti i testi localizzati
    public void UpdateLocalizedUI()
    {
        if (finalScoreText != null)
            finalScoreText.text = LanguageManager.instance.GetLocalizedText(finalScoreKey, "") + "" + (scoreManager != null ? scoreManager.GetTotalScore() : "0");

        if (highScoreText != null)
        {
            ScoringMode currentMode = (scoreManager != null) ? scoreManager.scoringMode : ScoringMode.ColorSlots; // Imposta un valore di default se scoreManager è null
            int currentHighScore = HighScoreManager.GetHighScore(currentMode);
            bool isNewHighScore = (scoreManager != null) ? HighScoreManager.UpdateHighScore(currentMode, scoreManager.GetTotalScore()) : false; // Tentativo di usare UpdateHighScore come indicatore di nuovo record

            if (isNewHighScore)
                highScoreText.text = LanguageManager.instance.GetLocalizedText(highScoreNewKey, "") + " " + LanguageManager.instance.GetLocalizedText(highScoreKey, "High Score") + ": " + currentHighScore;
            else
                highScoreText.text = LanguageManager.instance.GetLocalizedText(highScoreKey, "") + ": " + currentHighScore;
        }

        UpdateCoinsUI(); // Assicurati che anche il testo delle monete sia aggiornato

        if (earnedCoinsText != null && scoreManager != null)
        {
            int earnedCoins = CalculateCoins(scoreManager.GetTotalScore());
            earnedCoinsText.text = "+" + earnedCoins; // Rimossa la localizzazione e la parola "Coins"
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

    private void DisableLasers()
    {
        if (Lasers != null)
        {
            LaserGun compR = Lasers.GetComponent<LaserGun>();
            LaserGunBlue compB = Lasers.GetComponent<LaserGunBlue>();
            if (compB != null) compB.enabled = false;
            if (compR != null) compR.enabled = false;
        }
        if (PLaserR != null) PLaserR.SetActive(false);
        if (PLaserB != null) PLaserB.SetActive(false);

        StopParticleSystems();
    }

    private void EnableLasers()
    {
        if (Lasers != null)
        {
            LaserGun compR = Lasers.GetComponent<LaserGun>();
            LaserGunBlue compB = Lasers.GetComponent<LaserGunBlue>();
            if (compB != null) compB.enabled = true;
            if (compR != null) compR.enabled = true;
        }
        if (PLaserR != null) PLaserR.SetActive(true);
        if (PLaserB != null) PLaserB.SetActive(true);
    }

    private void StopParticleSystems()
    {
        if (PLaserR != null)
        {
            ParticleSystem psR = PLaserR.GetComponent<ParticleSystem>();
            if (psR != null) psR.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        if (PLaserB != null)
        {
            ParticleSystem psB = PLaserB.GetComponent<ParticleSystem>();
            if (psB != null) psB.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    // Aggiungi qui un metodo per riavviare la partita (se non esiste già)
    public void RestartGame()
    {
        // Inserisci qui la logica per riavviare la partita
        // Ad esempio, ricaricare la scena corrente
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        EnableLasers(); // Riattiva i laser al riavvio
    }

    // Aggiungi qui un metodo per tornare al menù principale (se non esiste già)
    public void GoToMainMenu()
    {
        // Inserisci qui la logica per tornare al menù principale
        // SceneManager.LoadScene("MainMenu");
        EnableLasers(); // Riattiva i laser tornando al menù (se necessario)
    }
}