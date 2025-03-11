using UnityEngine;
using System.Collections.Generic;

public class PowerUp : EnemyBase
{
    [Header("Statistiche del PowerUp")]
    [SerializeField] private float _powerUpMaxHP = 100f;
    public override float maxHP { get { return _powerUpMaxHP; } }

    [Header("Tipo di PowerUp")]
    [SerializeField] private bool isGreenEnemy = false; // Imposta true per i power-up che devono essere trattati come "green" nella modalità ColorCombination

    // Liste di power-up per i colpi di tipo Red e Blue
    public static List<string> PURList = new List<string>
    {
        "300 LR Energy Gauge",
        "10 /s LR Energy Consumption",
        "20 /s LR Damage Output",
        "40 Recharge LB Energy Gauge per D destroyed / C collected"
    };

    public static List<string> PUBList = new List<string>
    {
        "300 LB Energy Gauge",
        "10 /s LB Energy Consumption",
        "20 /s LB Damage Output",
        "40 Recharge LR Energy Gauge per D destroyed / C collected"
    };

    private PowerUpQuestionUI questionUI;
    private string pendingPowerUp;
    private string lastHitByColor = "";

    // Per i power-up, il colore viene determinato dall'ultimo colpo subito
    public override string EnemyColor => lastHitByColor;

    protected override void Start()
    {
        base.Start();
        questionUI = FindObjectOfType<PowerUpQuestionUI>();
        if (questionUI == null)
        {
            Debug.LogError("PowerUpQuestionUI non trovato!");
        }
    }

    public override void TakeDamage(float damage, string color)
    {
        lastHitByColor = color;
        currentHP -= damage;
        lastDamageTime = Time.time;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    protected override void Die()
    {
        if (questionUI == null)
        {
            Debug.LogError("PowerUpQuestionUI non trovato!");
            Destroy(gameObject);
            return;
        }

        // Selezione del power-up in base al colore dell'ultimo colpo
        if (lastHitByColor == "Red")
        {
            pendingPowerUp = PUBList[Random.Range(0, PUBList.Count)];
        }
        else if (lastHitByColor == "Blue")
        {
            pendingPowerUp = PURList[Random.Range(0, PURList.Count)];
        }
        else
        {
            List<string> combinedList = new List<string>(PURList);
            combinedList.AddRange(PUBList);
            pendingPowerUp = combinedList[Random.Range(0, combinedList.Count)];
        }

        // Mostra la UI per la conferma del power-up
        questionUI.ShowQuestion(this);
    }

    /// <summary>
    /// Metodo chiamato dalla UI dopo che il giocatore ha risposto correttamente.
    /// In modalità ColorCombination, se questo power-up è "verde", viene forzato il colore "green"
    /// per assegnare 515 punti direttamente; altrimenti, si utilizza il colore dell'ultimo laser.
    /// </summary>
    public void GrantPowerUp()
    {
        if (energyManager != null && !string.IsNullOrEmpty(pendingPowerUp))
        {
            energyManager.ApplyPowerUp(pendingPowerUp);
        }
        if (scoreManager != null)
        {
            if (scoreManager.scoringMode == ScoringMode.ColorSlots && isGreenEnemy)
            {
                scoreManager.AddScoreCustom("green", 5);
            }
            else
            {
                scoreManager.AddScoreCustom(lastHitByColor, 5);
            }
        }
        Destroy(gameObject);
    }
}
