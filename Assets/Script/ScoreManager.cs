using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ScoringMode
{
    Combo,
    ColorSlots
}

public class ScoreManager : MonoBehaviour
{
    public ScoringMode scoringMode = ScoringMode.Combo;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;      // Usato per Combo o per mostrare gli slot in ColorSlots
    public TextMeshProUGUI nextMultiplierText;    // Solo per Combo (opzionale)
    public Image comboTimerRadial;

    private ScoreSystemBase scoreSystem;

    void Start()
    {
        if (scoringMode == ScoringMode.Combo)
        {
            scoreSystem = new ComboScoreSystem(multiplierText, nextMultiplierText, comboTimerRadial);
        }
        else if (scoringMode == ScoringMode.ColorSlots)
        {
            scoreSystem = new ColorCombinationScoreSystem(multiplierText);
        }
        UpdateScoreUI();
    }

    void Update()
    {
        scoreSystem.Update();
        UpdateScoreUI();
    }

    public void AddScore(string color)
    {
        scoreSystem.AddScore(color);
        UpdateScoreUI();
    }

    public void AddScoreCustom(string color, int basePoints)
    {
        scoreSystem.AddScoreCustom(color, basePoints);
        UpdateScoreUI();
    }

    public void OnHit()
    {
        scoreSystem.OnHit();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = scoreSystem.GetTotalScore().ToString();
    }

    public int GetTotalScore()
    {
        return scoreSystem.GetTotalScore();
    }
}
