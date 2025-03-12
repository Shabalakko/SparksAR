using UnityEngine;
using System;

[Serializable]
public class Achievement
{
    public string name;             // Nome dell'achievement
    public string description;      // Descrizione
    public int requiredHighScore;   // Punteggio richiesto per sbloccare l'achievement
    public bool isUnlocked;         // Stato di sblocco

    public Action OnUnlocked;       // Evento che si attiva quando l'achievement viene sbloccato

    public Achievement(string name, string description, int requiredHighScore)
    {
        this.name = name;
        this.description = description;
        this.requiredHighScore = requiredHighScore;
        isUnlocked = false;
    }

    // Verifica se l'achievement va sbloccato in base all'HighScore passato
    public void CheckUnlock(int highScore)
    {
        if (!isUnlocked && highScore >= requiredHighScore)
        {
            isUnlocked = true;
            OnUnlocked?.Invoke();
            Debug.Log($"Achievement sbloccato: {name}");
        }
    }
}
