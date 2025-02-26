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
    // Utilizziamo lastColor per tenere traccia dell'ultimo colore passato (ad es. "Red" o "Blue")
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
        comboTimerRadial.fillAmount = 0f;
    }

    void Update()
    {
        exponentialFactor = 0.8f + (currentMultiplier * 0.1f);
        exponentialFactor = Mathf.Clamp(exponentialFactor, 0.8f, 2f);

        if (comboTimer > 0)
        {
            float timerDecrement = Time.deltaTime * timerSpeed * currentMultiplier * timerSpeedModifier;
            timerDecrement = Mathf.Pow(timerDecrement, exponentialFactor);

            comboTimer -= timerDecrement;
            comboTimerRadial.fillAmount = comboTimer / baseComboTime;

            if (comboTimer <= 0)
            {
                ResetCombo();
            }
        }

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

        // Imposta il colore corrente in base al parametro ricevuto ("Red" o "Blue")
        lastColor = color;

        UpdateScoreUI();
    }
    public void AddScoreCustom(string color, int basePoints)
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
        totalScore += basePoints * comboBonus;

        comboTimer = baseComboTime;
        comboTimerRadial.fillAmount = 1;
        lastColor = color;
        UpdateScoreUI();
    }

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
        // Al reset, se non c'è un colore specificato, impostiamo il colore a bianco
        lastColor = "";
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = totalScore.ToString();
        multiplierText.text = "x" + currentMultiplier;
        nextMultiplierText.text = "x" + colorComboStreak;

        // Controlla il valore di lastColor e imposta il colore del testo:
        // Se "Red", il testo sarà rosso (FF0000)
        // Se "Blue", il testo sarà blu (0006FF)
        // Altrimenti, il testo sarà bianco (FFFFFF)
        if (lastColor.Equals("Red", System.StringComparison.OrdinalIgnoreCase))
        {
            Color red;
            if (ColorUtility.TryParseHtmlString("#FF0000", out red))
            {
                nextMultiplierText.color = red;
            }
        }
        else if (lastColor.Equals("Blue", System.StringComparison.OrdinalIgnoreCase))
        {
            Color blue;
            if (ColorUtility.TryParseHtmlString("#0006FF", out blue))
            {
                nextMultiplierText.color = blue;
            }
        }
        else
        {
            nextMultiplierText.color = Color.white;
        }
    }
    public int GetTotalScore()
    {
        return totalScore;
    }

}
