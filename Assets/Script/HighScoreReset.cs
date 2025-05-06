using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HighScoreReset : MonoBehaviour
{
    // Riferimenti UI: verifica che siano collegati a due oggetti distinti!
    public TextMeshProUGUI highScoreTextCombo;
    public TextMeshProUGUI highScoreTextColorSlots;

    public void ResetComboHighScore()
    {
        HighScoreManager.ResetHighScore(ScoringMode.Combo);
        UpdateHighScoreUI();
        ReloadScene(); // Ricarica la scena dopo il reset
    }

    public void ResetColorSlotsHighScore()
    {
        HighScoreManager.ResetHighScore(ScoringMode.ColorSlots);
        UpdateHighScoreUI();
        ReloadScene(); // Ricarica la scena dopo il reset
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

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}