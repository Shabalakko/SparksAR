using UnityEngine;
using System.Collections.Generic;

public class PowerUp : EnemyBase
{
    [Header("Statistiche del PowerUp")]
    [SerializeField] private float _powerUpMaxHP = 100f;
    public override float maxHP { get { return _powerUpMaxHP; } }

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

        // Seleziona il power-up in base al colore dell'ultimo colpo
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

        // Mostra la domanda tramite la UI dedicata
        questionUI.ShowQuestion(this);
    }

    public void GrantPowerUp()
    {
        if (energyManager != null && !string.IsNullOrEmpty(pendingPowerUp))
        {
            energyManager.ApplyPowerUp(pendingPowerUp);
        }
        if (scoreManager != null)
        {
            // Se siamo in modalità ColorSlots, consideriamo il power-up come "green"
            // solo se l'ultimo colpo non era "Red" o "Blue".
            if (scoreManager.scoringMode == ScoringMode.ColorSlots && lastHitByColor != "Red" && lastHitByColor != "Blue")
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
