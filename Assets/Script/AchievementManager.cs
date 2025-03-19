using UnityEngine;
using System.Collections.Generic;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    public List<Achievement> achievements = new List<Achievement>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Rende l'istanza persistente
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Achievement per modalità Combo (Combo multiplier)
        achievements.Add(new Achievement("Color Combo Novice I", "Reach 500 points", 500, ScoringMode.Combo));
        achievements.Add(new Achievement("Color Combo Novice II", "Reach 1000 points", 1000, ScoringMode.Combo));
        achievements.Add(new Achievement("Color Combo Novice III", "Reach 2000 points", 2000, ScoringMode.Combo));
        achievements.Add(new Achievement("Color Combo Expert I", "Reach 3000 points", 3000, ScoringMode.Combo));
        achievements.Add(new Achievement("Color Combo Expert II", "Reach 5000 points", 5000, ScoringMode.Combo));
        achievements.Add(new Achievement("Color Combo Expert III", "Reach 7500 points", 7500, ScoringMode.Combo));
        achievements.Add(new Achievement("Color Combo Master I", "Reach 10000 points", 10000, ScoringMode.Combo));
        achievements.Add(new Achievement("Color Combo Master II", "Reach 20000 points", 20000, ScoringMode.Combo));
        achievements.Add(new Achievement("Color Combo Master III", "Reach 30000 points", 30000, ScoringMode.Combo));

        // Achievement per modalità ColorSlots (Color Combination)
        achievements.Add(new Achievement("Color Combo Novice I", "Reach 500 points", 500, ScoringMode.ColorSlots));
        achievements.Add(new Achievement("Color Combo Novice II", "Reach 1000 points", 1000, ScoringMode.ColorSlots));
        achievements.Add(new Achievement("Color Combo Novice III", "Reach 1500 points", 1500, ScoringMode.ColorSlots));
        achievements.Add(new Achievement("Color Combo Expert I", "Reach 2000 points", 2000, ScoringMode.ColorSlots));
        achievements.Add(new Achievement("Color Combo Expert II", "Reach 2500 points", 2500, ScoringMode.ColorSlots));
        achievements.Add(new Achievement("Color Combo Expert III", "Reach 5000 points", 5000, ScoringMode.ColorSlots));
        achievements.Add(new Achievement("Color Combo Master I", "Reach 6000 points", 6000, ScoringMode.ColorSlots));
        achievements.Add(new Achievement("Color Combo Master II", "Reach 7000 points", 7000, ScoringMode.ColorSlots));
        achievements.Add(new Achievement("Color Combo Master III", "Reach 10000 points", 10000, ScoringMode.ColorSlots));
        // Imposta il callback per mostrare la UI degli achievement sbloccati
        foreach (Achievement ach in achievements)
        {
            ach.OnUnlocked += () => ShowAchievementUI(ach);
        }
    }

    // Modifica del metodo: ora riceve anche la modalità corrente
    public void CheckAchievements(ScoringMode mode, int currentScore)
    {
        foreach (Achievement ach in achievements)
        {
            // Controlla solo gli achievement relativi alla modalità corrente
            if (ach.mode == mode)
            {
                ach.CheckUnlock(currentScore);
            }
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
