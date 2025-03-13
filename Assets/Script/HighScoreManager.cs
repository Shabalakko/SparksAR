using UnityEngine;

public static class HighScoreManager
{
    public static int GetHighScore(ScoringMode mode)
    {
        string key = (mode == ScoringMode.Combo) ? "HighScore_Combo" : "HighScore_ColorSlots";
        int score = PlayerPrefs.GetInt(key, 0);
        //Debug.Log("GetHighScore: Mode " + mode + " | Key: " + key + " | Value: " + score);
        return score;
    }

    public static void SetHighScore(ScoringMode mode, int score)
    {
        string key = (mode == ScoringMode.Combo) ? "HighScore_Combo" : "HighScore_ColorSlots";
        PlayerPrefs.SetInt(key, score);
        PlayerPrefs.Save();
        //Debug.Log("SetHighScore: Mode " + mode + " | Key: " + key + " | New Score: " + score);
    }

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

    public static void ResetHighScore(ScoringMode mode)
    {
        string key = (mode == ScoringMode.Combo) ? "HighScore_Combo" : "HighScore_ColorSlots";
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
        //Debug.Log("ResetHighScore: Mode " + mode + " | Key: " + key);
    }
}
