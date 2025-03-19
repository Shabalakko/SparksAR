using UnityEngine;
using System;

[Serializable]
public class Achievement
{
    public string name;
    public string description;
    public int requiredScore;       // Usato per il punteggio corrente
    public ScoringMode mode;        // Nuovo: modalità per cui è valido l'achievement
    public bool isUnlocked;

    public Action OnUnlocked;

    public Achievement(string name, string description, int requiredScore, ScoringMode mode)
    {
        this.name = name;
        this.description = description;
        this.requiredScore = requiredScore;
        this.mode = mode;
        // Carica lo stato salvato, default false (0)
        isUnlocked = PlayerPrefs.GetInt("Achievement_" + name, 0) == 1;
    }

    // Verifica se l'achievement va sbloccato in base al punteggio passato
    public void CheckUnlock(int currentScore)
    {
        if (!isUnlocked && currentScore >= requiredScore)
        {
            isUnlocked = true;
            PlayerPrefs.SetInt("Achievement_" + name, 1);
            PlayerPrefs.Save();
            OnUnlocked?.Invoke();
            Debug.Log($"Achievement sbloccato: {name}");
        }
    }
}
