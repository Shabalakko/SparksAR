using UnityEngine;
using TMPro;

public class HighScoreReset : MonoBehaviour
{
    // Riferimenti UI: verifica che siano collegati a due oggetti distinti!
    public TextMeshProUGUI highScoreTextCombo;
    public TextMeshProUGUI highScoreTextColorSlots;

    public void ResetComboHighScore()
    {
        HighScoreManager.ResetHighScore(ScoringMode.Combo);
        UpdateHighScoreUI();
    }

    public void ResetColorSlotsHighScore()
    {
        HighScoreManager.ResetHighScore(ScoringMode.ColorSlots);
        UpdateHighScoreUI();
    }

    private void UpdateHighScoreUI()
    {
        int highScoreCombo = HighScoreManager.GetHighScore(ScoringMode.Combo);
        int highScoreColorSlots = HighScoreManager.GetHighScore(ScoringMode.ColorSlots);

        if (highScoreTextCombo != null)
        {
            highScoreTextCombo.text = "" + highScoreCombo;
            //Debug.Log("HighScoreTextCombo: " + highScoreTextCombo.gameObject.name);
        }
        if (highScoreTextColorSlots != null)
        {
            highScoreTextColorSlots.text = "" + highScoreColorSlots;
            //Debug.Log("HighScoreTextColorSlots: " + highScoreTextColorSlots.gameObject.name);
        }
        //Debug.Log("UpdateHighScoreUI: Combo = " + highScoreCombo + ", ColorSlots = " + highScoreColorSlots);
    }
}
