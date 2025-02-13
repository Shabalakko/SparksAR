using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;

    private int totalScore = 0;
    private int currentMultiplier = 1;
    private string lastColor = "";

    void Start()
    {
        // Inizializza la UI
        UpdateScoreUI();
    }

    // Chiamato quando un nemico viene distrutto
    public void AddScore(string color)
    {
        // Se il colore è lo stesso del precedente, aumenta il moltiplicatore
        if (color == lastColor)
        {
            currentMultiplier++;
        }
        else
        {
            // Se il colore è diverso, resetta il moltiplicatore
            currentMultiplier = 1;
        }

        // Aggiunge il punteggio moltiplicato
        totalScore += 1 * currentMultiplier;

        // Salva il colore corrente come ultimo colore distrutto
        lastColor = color;

        // Aggiorna la UI
        UpdateScoreUI();
    }

    // Aggiorna la UI del punteggio e del moltiplicatore
    private void UpdateScoreUI()
    {
        scoreText.text = "Score: " + totalScore;
        multiplierText.text = "Multiplier: x" + currentMultiplier;
    }
}
