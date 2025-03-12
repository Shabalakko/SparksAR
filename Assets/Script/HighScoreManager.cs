using UnityEngine;

public static class HighScoreManager
{
    // Recupera l'HighScore in base alla modalit�
    public static int GetHighScore(ScoringMode mode)
    {
        string key = (mode == ScoringMode.Combo) ? "HighScore_Combo" : "HighScore_ColorSlots";
        return PlayerPrefs.GetInt(key, 0);
    }

    // Salva l'HighScore in base alla modalit�
    public static void SetHighScore(ScoringMode mode, int score)
    {
        string key = (mode == ScoringMode.Combo) ? "HighScore_Combo" : "HighScore_ColorSlots";
        PlayerPrefs.SetInt(key, score);
        PlayerPrefs.Save(); // Salva immediatamente i dati
    }


    // Se il nuovo punteggio � maggiore dell'HighScore salvato, lo aggiorna e restituisce true
    public static bool UpdateHighScore(ScoringMode mode, int newScore)
    {
        int currentHighScore = GetHighScore(mode);
        if (newScore > currentHighScore)
        {
            SetHighScore(mode, newScore);
            return true;
        }
        return false;
    }
}
