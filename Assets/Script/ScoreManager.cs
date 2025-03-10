using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public enum ScoringMode
{
    Combo,
    ColorSlots
}

public class ScoreManager : MonoBehaviour
{
    // Elementi dell'interfaccia utente
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;
    public TextMeshProUGUI nextMultiplierText;
    public Image comboTimerRadial;

    // Variabili per il punteggio e il sistema di combo
    private int totalScore = 0;
    private int currentMultiplier = 0;
    private int colorComboStreak = 0;
    // lastColor viene usato nella modalità Combo
    private string lastColor = "";
    private float comboTimer = 0f;
    private float baseComboTime = 10f;

    // Impostazioni del timer della combo
    public float timerSpeed = 1f;
    public float exponentialFactor = 1f;
    public float slowDownFactor = 0.5f;
    private float timerSpeedModifier = 1f;

    // Modalità di punteggio: Combo (vecchio sistema) o ColorSlots (nuovo sistema)
    public ScoringMode scoringMode = ScoringMode.Combo;

    // Variabili per il sistema ColorSlots
    private List<string> colorSlots = new List<string>();
    private Dictionary<string, int> colorCombinations = new Dictionary<string, int>()
    {
        { "RedRedRed", 100 },
        { "BlueBlueBlue", 150 },
        { "GreenGreenGreen", 200 },
        { "RedBlueGreen", 250 }
        // Aggiungi altre combinazioni se necessario
    };

    void Start()
    {
        UpdateScoreUI();
        // Se usi il sistema combo, inizializza il timer; altrimenti, non serve
        comboTimerRadial.fillAmount = (scoringMode == ScoringMode.Combo) ? 0f : 0f;
    }

    void Update()
    {
        if (scoringMode == ScoringMode.Combo)
        {
            // Aggiorna il fattore esponenziale in base al moltiplicatore attuale
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
    }

    /// <summary>
    /// Metodo per aggiungere punteggio in base al colore del nemico.
    /// Funziona in modo diverso a seconda della modalità di punteggio.
    /// </summary>
    public void AddScore(string color)
    {
        if (scoringMode == ScoringMode.Combo)
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
            comboTimerRadial.fillAmount = 1f;
            lastColor = color;
        }
        else if (scoringMode == ScoringMode.ColorSlots)
        {
            // Se abbiamo già tre slot, rimuoviamo il più vecchio
            if (colorSlots.Count >= 3)
            {
                colorSlots.RemoveAt(0);
            }
            colorSlots.Add(color);
            EvaluateColorCombo();
        }

        UpdateScoreUI();
    }

    /// <summary>
    /// Variante di AddScore che applica dei punti base custom.
    /// </summary>
    public void AddScoreCustom(string color, int basePoints)
    {
        if (scoringMode == ScoringMode.Combo)
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
            comboTimerRadial.fillAmount = 1f;
            lastColor = color;
        }
        else if (scoringMode == ScoringMode.ColorSlots)
        {
            if (colorSlots.Count >= 3)
            {
                colorSlots.RemoveAt(0);
            }
            colorSlots.Add(color);
            EvaluateColorCombo();
            // Se vuoi, aggiungi anche i basePoints direttamente
            totalScore += basePoints;
        }

        UpdateScoreUI();
    }

    /// <summary>
    /// Metodo per rallentare temporaneamente il timer (ad es. quando il nemico viene colpito).
    /// </summary>
    public void OnHit()
    {
        timerSpeedModifier = slowDownFactor;
    }

    private void ResetCombo()
    {
        colorComboStreak = 0;
        currentMultiplier = 1;
        comboTimer = 0f;
        comboTimerRadial.fillAmount = 0f;
        lastColor = "";
        UpdateScoreUI();
    }

    /// <summary>
    /// Verifica se i tre slot di colore formano una combinazione valida e, in tal caso, aggiunge il bonus.
    /// </summary>
    private void EvaluateColorCombo()
    {
        if (colorSlots.Count == 3)
        {
            // Crea una chiave concatenando i colori nello stesso ordine
            string comboKey = string.Join("", colorSlots.ToArray());
            if (colorCombinations.ContainsKey(comboKey))
            {
                int bonus = colorCombinations[comboKey];
                totalScore += bonus;
                Debug.Log($"Combo colore {comboKey} ottenuta! Bonus: {bonus} punti");
                // Se desideri, puoi svuotare gli slot dopo una combo vincente:
                // colorSlots.Clear();
            }
        }
    }

    private void UpdateScoreUI()
    {
        scoreText.text = totalScore.ToString();

        if (scoringMode == ScoringMode.Combo)
        {
            multiplierText.text = "x" + currentMultiplier;
            nextMultiplierText.text = "x" + colorComboStreak;
        }
        else if (scoringMode == ScoringMode.ColorSlots)
        {
            // Mostra i colori attualmente registrati, separati da un trattino
            string slotDisplay = string.Join(" - ", colorSlots.ToArray());
            multiplierText.text = slotDisplay;
            nextMultiplierText.text = "";
            comboTimerRadial.fillAmount = 0f;
        }

        // Imposta il colore del testo in base al valore di lastColor (utile per la modalità Combo)
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
