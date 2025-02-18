using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;
    public TextMeshProUGUI nextMultiplierText;
    public Image comboTimerRadial;

    private int totalScore = 0;
    private int currentMultiplier = 1;
    private int colorComboStreak = 0;
    private string lastColor = "";
    private float comboTimer = 0f;
    private float baseComboTime = 10f;

    [Header("Impostazioni del Timer della Combo")]
    public float timerSpeed = 1f;
    public float exponentialFactor = 1f;
    public float slowDownFactor = 0.5f;

    // Modificatore per rallentare temporaneamente il timer
    private float timerSpeedModifier = 1f;

    void Start()
    {
        UpdateScoreUI();
    }

    void Update()
    {
        // Incrementa exponentialFactor con la combo
        exponentialFactor = 0.8f + (currentMultiplier * 0.1f);
        exponentialFactor = Mathf.Clamp(exponentialFactor, 0.8f, 2f);

        // Aggiorna il timer della combo
        if (comboTimer > 0)
        {
            float timerDecrement = Time.deltaTime * timerSpeed * currentMultiplier * timerSpeedModifier;
            timerDecrement = Mathf.Pow(timerDecrement, exponentialFactor);

            // Applica il decremento al timer
            comboTimer -= timerDecrement;

            // Aggiorna l'indicatore radiale in base al tempo rimanente
            comboTimerRadial.fillAmount = comboTimer / baseComboTime;

            // Se il timer scade, resetta la combo
            if (comboTimer <= 0)
            {
                ResetCombo();
            }
        }

        // Gradualmente ritorna alla velocità normale
        if (timerSpeedModifier < 1f)
        {
            timerSpeedModifier += Time.deltaTime;
            timerSpeedModifier = Mathf.Clamp(timerSpeedModifier, slowDownFactor, 1f);
        }
    }

    public void AddScore(string color)
    {
        currentMultiplier++;

        if (color == lastColor)
        {
            colorComboStreak++;
        }
        else
        {
            colorComboStreak = 1;
        }

        int comboBonus = currentMultiplier * colorComboStreak;
        totalScore += 1 * comboBonus;

        comboTimer = baseComboTime;
        comboTimerRadial.fillAmount = 1;

        lastColor = color;

        UpdateScoreUI();
    }

    // Rallenta temporaneamente il timer quando si colpisce un nemico
    public void OnHit()
    {
        timerSpeedModifier = slowDownFactor;
    }

    private void ResetCombo()
    {
        colorComboStreak = 0;
        currentMultiplier = 1;
        comboTimer = 0;
        comboTimerRadial.fillAmount = 0;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = "Score: " + totalScore;
        multiplierText.text = "x" + (currentMultiplier);
        nextMultiplierText.text = "x" + (colorComboStreak);
    }
}
