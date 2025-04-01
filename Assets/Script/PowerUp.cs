using UnityEngine;
using System.Collections.Generic;

public class PowerUp : EnemyBase
{
    [Header("Statistiche del PowerUp")]
    [SerializeField] private float _powerUpMaxHP = 100f;
    public override float maxHP { get { return _powerUpMaxHP; } }

    [Header("Tipo di PowerUp")]
    [SerializeField] private bool isGreenEnemy = false;

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

        questionUI.ShowQuestion(this);
    }

    public void GrantPowerUp(bool correct)
    {
        if (scoreManager != null)
        {
            if (correct)
            {
                int scoreBefore = scoreManager.GetTotalScore();
                if (scoreManager.scoringMode == ScoringMode.Combo)
                {
                    if (energyManager != null && !string.IsNullOrEmpty(pendingPowerUp))
                    {
                        energyManager.ApplyPowerUp(pendingPowerUp);
                    }
                    if (string.IsNullOrEmpty(scoreManager.CurrentComboColor))
                    {
                        scoreManager.AddScoreCustom(lastHitByColor, 15);
                    }

                    int scoreGained = scoreManager.GetTotalScore() - scoreBefore;
                    scoreManager.ShowScorePopup(scoreGained);
                }
                else if (scoreManager.scoringMode == ScoringMode.ColorSlots)
                {
                    if (isGreenEnemy)
                    {
                        scoreManager.AddScoreCustom("green", 0); //non aggiunge punti, lascia gestire tutto a colorCombinationScoreSystem
                    }
                    else
                    {
                        scoreManager.AddScoreCustom(lastHitByColor, 0); //non aggiunge punti, lascia gestire tutto a colorCombinationScoreSystem
                    }
                    int scoreGained = scoreManager.GetTotalScore() - scoreBefore;
                    scoreManager.ShowScorePopup(scoreGained);
                }
            }
            else
            {
                scoreManager.ShowScorePopup(0);
            }
        }
        base.Die();
    }
}
