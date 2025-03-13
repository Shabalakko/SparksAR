using UnityEngine;
using TMPro;

public class HighScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI highScoreTextCombo;    // Testo per la modalit� Combo
    public TextMeshProUGUI highScoreTextColorSlots; // Testo per la modalit� ColorCombination

    void Start()
    {
        // Recupera i punteggi salvati nei PlayerPrefs
        int highScoreCombo = HighScoreManager.GetHighScore(ScoringMode.Combo);
        int highScoreColorSlots = HighScoreManager.GetHighScore(ScoringMode.ColorSlots);

        // Aggiorna i testi della UI
        if (highScoreTextCombo != null)
            highScoreTextCombo.text = "" + highScoreCombo;

        if (highScoreTextColorSlots != null)
            highScoreTextColorSlots.text = "" + highScoreColorSlots;
    }
}
