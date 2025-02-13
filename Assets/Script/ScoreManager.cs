using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;
    public TextMeshProUGUI nextMultiplierText;
    public Image comboTimerRadial; // Immagine radiale per il timer della combo

    private int totalScore = 0;
    private int currentMultiplier = 1;
    private int colorComboStreak = 0; // Combo consecutiva dello stesso colore
    private string lastColor = "";
    private float comboTimer = 0f;
    private float baseComboTime = 10f; // Tempo base per mantenere la combo

    // --- Parametri modificabili in Inspector ---
    [Header("Impostazioni del Timer della Combo")]
    public float timerSpeed = 1f;          // Velocità con cui il timer scende a zero
    public float exponentialFactor = 1f;   // Coefficiente per la curva di accelerazione
    public float slowDownFactor = 0.5f;    // Rallentamento quando si colpisce un nemico

    // Modificatore per rallentare temporaneamente il timer
    private float timerSpeedModifier = 1f;

    void Start()
    {
        UpdateScoreUI();
    }

    void Update()
    {
        // --- Incrementa exponentialFactor con la combo ---
        exponentialFactor = 0.8f + (currentMultiplier * 0.1f);
        exponentialFactor = Mathf.Clamp(exponentialFactor, 0.8f, 2f);

        // Aggiorna il timer della combo
        if (comboTimer > 0)
        {
            // --- MODIFICA: Isoliamo il calcolo del decremento del timer ---
            float timerDecrement = Time.deltaTime * timerSpeed * currentMultiplier * timerSpeedModifier;
            timerDecrement = Mathf.Pow(timerDecrement, exponentialFactor);

            // Applica il decremento al timer senza toccare Time.deltaTime
            comboTimer -= timerDecrement;

            // Aggiorna l'indicatore radiale in base al tempo rimanente
            comboTimerRadial.fillAmount = comboTimer / (baseComboTime);

            // Se il timer scade, resetta la combo
            if (comboTimer <= 0)
            {
                ResetCombo();
            }
        }

        // --- Ripristina gradualmente la velocità normale quando non si colpisce più ---
        if (timerSpeedModifier < 1f)
        {
            timerSpeedModifier += Time.deltaTime;
            timerSpeedModifier = Mathf.Clamp(timerSpeedModifier, 0.5f, 1f);
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

    public void OnHit()
    {
        timerSpeedModifier = 0.5f;
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
