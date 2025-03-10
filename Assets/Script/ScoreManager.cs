using System.Collections;
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
    public TextMeshProUGUI multiplierText;
    public TextMeshProUGUI nextMultiplierText;
    public Image comboTimerRadial;

    public Image[] combinationIcons; // Icone UI per i colori

    private ScoreSystemBase scoreSystem;
    private Coroutine resetCoroutine;
    

    void Start()
    {
        if(scoringMode == ScoringMode.Combo)
        {
            scoreSystem = new ComboScoreSystem(multiplierText, nextMultiplierText, comboTimerRadial);
        }
        else if(scoringMode == ScoringMode.ColorSlots)
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

    public int GetTotalScore()
    {
        return scoreSystem.GetTotalScore();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = scoreSystem.GetTotalScore().ToString();

        if (scoringMode == ScoringMode.ColorSlots && combinationIcons != null && combinationIcons.Length > 0)
        {
            ColorCombinationScoreSystem colorSystem = scoreSystem as ColorCombinationScoreSystem;
            if (colorSystem != null)
            {
                var currentColors = colorSystem.CurrentColors;

                // Update the UI icons to match the current colors
                for (int i = 0; i < combinationIcons.Length; i++)
                {
                    if (i < currentColors.Count)
                    {
                        combinationIcons[i].color = GetColorFromString(currentColors[i]);
                    }
                    else
                    {
                        combinationIcons[i].color = Color.white;
                    }
                }

                // If we have a full combination, start (or restart) the reset timer...
                if (currentColors.Count == 3)
                {
                    // Stop current coroutine to reset timing if needed
                    if (resetCoroutine != null)
                        StopCoroutine(resetCoroutine);

                    resetCoroutine = StartCoroutine(ResetIconsAfterDelay(2.0f));
                }
                // ...but if the combination is no longer complete,
                // cancel any pending reset so that the new combination remains visible.
                else if (resetCoroutine != null)
                {
                    StopCoroutine(resetCoroutine);
                    resetCoroutine = null;
                }
            }
        }
    }



    private IEnumerator ResetIconsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        ColorCombinationScoreSystem colorSystem = scoreSystem as ColorCombinationScoreSystem;
        if (colorSystem != null)
        {
            colorSystem.ResetColorSlots();
            // Reset della combinazione
            UpdateScoreUI(); // Aggiorniamo la UI per mostrare le icone bianche
        }
    }

    private Color GetColorFromString(string colorName)
    {
        switch(colorName.ToLower())
        {
            case "red": return Color.red;
            case "blue": return Color.blue;
            //case "green": return Color.green;
            default: return Color.white;
        }
    }
}
