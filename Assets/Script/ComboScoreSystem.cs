using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComboScoreSystem : ScoreSystemBase
{
    private int currentMultiplier = 0;
    private int colorComboStreak = 0;
    private string lastColor = "";
    private float comboTimer = 0f;
    private float baseComboTime = 10f;

    private TextMeshProUGUI multiplierText;
    private TextMeshProUGUI nextMultiplierText;
    private Image comboTimerRadial;

    public float timerSpeed = 1f;
    public float exponentialFactor = 1f;
    public float slowDownFactor = 0.5f;
    private float timerSpeedModifier = 1f;

    public ComboScoreSystem(TextMeshProUGUI multiplierText, TextMeshProUGUI nextMultiplierText, Image comboTimerRadial)
    {
        this.multiplierText = multiplierText;
        this.nextMultiplierText = nextMultiplierText;
        this.comboTimerRadial = comboTimerRadial;
    }

    public override void AddScore(string color)
    {
        currentMultiplier++;

        if (color == lastColor)
            colorComboStreak++;
        else
            colorComboStreak = 1;

        int comboBonus = currentMultiplier * colorComboStreak;
        totalScore += 1 * comboBonus;
        comboTimer = baseComboTime;
        if (comboTimerRadial) comboTimerRadial.fillAmount = 1f;
        lastColor = color;
        UpdateUI();
    }

    public override void AddScoreCustom(string color, int basePoints)
    {
        currentMultiplier++;

        if (color == lastColor)
            colorComboStreak++;
        else
            colorComboStreak = 1;

        int comboBonus = currentMultiplier * colorComboStreak;
        totalScore += basePoints * comboBonus;
        comboTimer = baseComboTime;
        if (comboTimerRadial) comboTimerRadial.fillAmount = 1f;
        lastColor = color;
        UpdateUI();
    }

    public override void Update()
    {
        exponentialFactor = 0.8f + (currentMultiplier * 0.1f);
        exponentialFactor = Mathf.Clamp(exponentialFactor, 0.8f, 2f);

        if (comboTimer > 0)
        {
            float timerDecrement = Time.deltaTime * timerSpeed * currentMultiplier * timerSpeedModifier;
            timerDecrement = Mathf.Pow(timerDecrement, exponentialFactor);

            comboTimer -= timerDecrement;
            if (comboTimerRadial) comboTimerRadial.fillAmount = comboTimer / baseComboTime;

            if (comboTimer <= 0)
                ResetCombo();
        }

        if (timerSpeedModifier < 1f)
        {
            timerSpeedModifier += Time.deltaTime;
            timerSpeedModifier = Mathf.Clamp(timerSpeedModifier, slowDownFactor, 1f);
        }
    }

    public override void OnHit()
    {
        timerSpeedModifier = slowDownFactor;
    }

    private void ResetCombo()
    {
        colorComboStreak = 0;
        currentMultiplier = 1;
        comboTimer = 0f;
        if (comboTimerRadial) comboTimerRadial.fillAmount = 0f;
        lastColor = "";
        UpdateUI();
    }

    // Aggiorna la UI: imposta il testo e il colore in base al tipo di combo attivo.
    private void UpdateUI()
    {
        if (multiplierText != null)
            multiplierText.text = "x" + currentMultiplier;
        if (nextMultiplierText != null)
            nextMultiplierText.text = "x" + colorComboStreak;

        // Se la combo è attiva e lastColor è valorizzato, imposta il colore del testo.
        if (comboTimer > 0 && !string.IsNullOrEmpty(lastColor))
        {
            if (lastColor.ToLower() == "red")
            {
                //multiplierText.color = Color.red;
                nextMultiplierText.color = Color.red;
            }
            else if (lastColor.ToLower() == "blue")
            {
                //multiplierText.color = Color.blue;
                nextMultiplierText.color = Color.blue;
            }
            else
            {
                //multiplierText.color = Color.white;
                nextMultiplierText.color = Color.white;
            }
        }
        else
        {
            // Se il timer è scaduto o non c'è una combo attiva, il testo diventa bianco.
            //multiplierText.color = Color.white;
            nextMultiplierText.color = Color.white;
        }
    }
}
