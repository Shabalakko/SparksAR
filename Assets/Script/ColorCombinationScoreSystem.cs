using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ColorCombinationScoreSystem : ScoreSystemBase
{
    private List<string> colorSlots = new List<string>();

    private Dictionary<string, int> colorCombinations = new Dictionary<string, int>()
    {
        { "RedRedRed", 228 },
        { "BlueBlueBlue", 228 },
        { "RedRedBlue", 337 },
        { "BlueBlueRed", 337 },
        { "BlueRedRed",373 },
        { "RedBlueBlue", 373 },
        { "RedBlueRed", 446 },
        { "BlueRedBlue", 446 },
        // Aggiungi altre combinazioni se necessario
    };

    private TextMeshProUGUI slotDisplayText;
    // Callback da invocare quando la combo è valutata
    private Action<int> onComboEvaluated;

    public ColorCombinationScoreSystem(TextMeshProUGUI slotDisplayText, Action<int> onComboEvaluated)
    {
        this.slotDisplayText = slotDisplayText;
        this.onComboEvaluated = onComboEvaluated;
    }

    public override void AddScore(string color)
    {
        if (color.ToLower() == "green")
        {
            totalScore += 515;
            Debug.Log("Green enemy destroyed, awarding 515 points");
            return;
        }
        if (colorSlots.Count >= 3)
            colorSlots.RemoveAt(0);
        colorSlots.Add(color);
        EvaluateColorCombo();
    }

    public override void AddScoreCustom(string color, int basePoints)
    {
        if (color.ToLower() == "green")
        {
            totalScore += 515;
            Debug.Log("Green enemy destroyed, awarding 515 points");
            return;
        }
        if (colorSlots.Count >= 3)
            colorSlots.RemoveAt(0);
        colorSlots.Add(color);
        EvaluateColorCombo();
        totalScore += basePoints;
    }

    public override void Update()
    {
        // Nessun aggiornamento periodico necessario per questo sistema.
    }

    private void EvaluateColorCombo()
    {
        if (colorSlots.Count == 3)
        {
            string comboKey = string.Join("", colorSlots.ToArray());
            if (colorCombinations.ContainsKey(comboKey))
            {
                int bonus = colorCombinations[comboKey];
                totalScore += bonus;
                Debug.Log($"Combo colore {comboKey} ottenuta! Bonus: {bonus} punti");

                // Invoca la callback per mostrare il popup del punteggio
                onComboEvaluated?.Invoke(bonus);
            }
        }
    }

    public void ResetColorSlots()
    {
        colorSlots.Clear();
    }

    // Proprietà per esporre la lista dei colori correnti
    public List<string> CurrentColors
    {
        get { return colorSlots; }
    }
}
