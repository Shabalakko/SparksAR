using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ColorCombinationScoreSystem : ScoreSystemBase
{
    private List<string> colorSlots = new List<string>();

    private Dictionary<string, int> colorCombinations = new Dictionary<string, int>()
    {
        { "RedRedRed", 100 },
        { "BlueBlueBlue", 150 },
        { "GreenGreenGreen", 200 },
        { "RedBlueGreen", 250 }
    };

    private TextMeshProUGUI slotDisplayText;

    public ColorCombinationScoreSystem(TextMeshProUGUI slotDisplayText)
    {
        this.slotDisplayText = slotDisplayText;
    }

    public override void AddScore(string color)
    {
        if (colorSlots.Count >= 3)
            colorSlots.RemoveAt(0);

        colorSlots.Add(color);
        EvaluateColorCombo();
        UpdateUI();
    }

    public override void AddScoreCustom(string color, int basePoints)
    {
        if (colorSlots.Count >= 3)
            colorSlots.RemoveAt(0);

        colorSlots.Add(color);
        EvaluateColorCombo();
        totalScore += basePoints;
        UpdateUI();
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
                // Se preferisci, puoi resettare gli slot qui: colorSlots.Clear();
            }
        }
    }

    private void UpdateUI()
    {
        if (slotDisplayText != null)
            slotDisplayText.text = string.Join(" - ", colorSlots.ToArray());
    }
}
