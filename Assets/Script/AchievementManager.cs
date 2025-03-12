using UnityEngine;
using System.Collections.Generic;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    public List<Achievement> achievements = new List<Achievement>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Esempio di achievement basati sull'HighScore:
        achievements.Add(new Achievement("Score Novice", "Raggiungi un HighScore di 1000", 1000));
        achievements.Add(new Achievement("Score Expert", "Raggiungi un HighScore di 5000", 5000));
        achievements.Add(new Achievement("Score Master", "Raggiungi un HighScore di 10000", 10000));

        // Imposta il callback per mostrare la UI degli achievement sbloccati
        foreach (Achievement ach in achievements)
        {
            ach.OnUnlocked += () => ShowAchievementUI(ach);
        }
    }

    // Verifica tutti gli achievement in base all'HighScore della modalità attuale
    public void CheckAchievements(ScoringMode mode)
    {
        int highScore = HighScoreManager.GetHighScore(mode);
        foreach (Achievement ach in achievements)
        {
            ach.CheckUnlock(highScore);
        }
    }


    private void ShowAchievementUI(Achievement achievement)
    {
        if (AchievementUIManager.Instance != null)
        {
            AchievementUIManager.Instance.ShowAchievement(achievement.name, achievement.description);
        }
    }
}
