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
    private string lastCombo = null; // Tiene traccia della combo corrente

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

    // Se la combo è completa, la resetta e prende il nuovo colore come primo elemento
    public void AddScore(string color)
    {
        if (scoringMode == ScoringMode.ColorSlots)
        {
            ColorCombinationScoreSystem colorSystem = scoreSystem as ColorCombinationScoreSystem;
            if (colorSystem.CurrentColors.Count == 3)
            {
                colorSystem.ResetColorSlots();
                if (resetCoroutine != null)
                {
                    StopCoroutine(resetCoroutine);
                    resetCoroutine = null;
                    lastCombo = null;
                }
            }
        }
        scoreSystem.AddScore(color);
        UpdateScoreUI();
    }

    public void AddScoreCustom(string color, int basePoints)
    {
        if (scoringMode == ScoringMode.ColorSlots)
        {
            ColorCombinationScoreSystem colorSystem = scoreSystem as ColorCombinationScoreSystem;
            if (colorSystem.CurrentColors.Count == 3)
            {
                colorSystem.ResetColorSlots();
                if (resetCoroutine != null)
                {
                    StopCoroutine(resetCoroutine);
                    resetCoroutine = null;
                    lastCombo = null;
                }
            }
        }
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

                // Aggiorna le icone UI in base ai colori correnti
                for (int i = 0; i < combinationIcons.Length; i++)
                {
                    if (i < currentColors.Count)
                        combinationIcons[i].color = GetColorFromString(currentColors[i]);
                    else
                        combinationIcons[i].color = Color.white;
                }

                // Avvia il timer di reset solo se la combo è completa (3 colori)
                if (currentColors.Count == 3)
                {
                    string currentCombo = string.Join("", currentColors);
                    if (resetCoroutine == null || lastCombo != currentCombo)
                    {
                        if (resetCoroutine != null)
                            StopCoroutine(resetCoroutine);
                        resetCoroutine = StartCoroutine(ResetIconsAfterDelay(2.0f));
                        lastCombo = currentCombo;
                    }
                }
                else
                {
                    // Se la combo non è completa, interrompi il timer (se in esecuzione)
                    if (resetCoroutine != null)
                    {
                        StopCoroutine(resetCoroutine);
                        resetCoroutine = null;
                        lastCombo = null;
                    }
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
            UpdateScoreUI(); // Aggiorna la UI per mostrare le icone resettate
        }
        resetCoroutine = null;
        lastCombo = null;
    }

    private Color GetColorFromString(string colorName)
    {
        switch (colorName.ToLower())
        {
            case "red": return Color.red;
            case "blue": return Color.blue;
            case "green": return Color.green;
            default: return Color.white;
        }
    }
}
