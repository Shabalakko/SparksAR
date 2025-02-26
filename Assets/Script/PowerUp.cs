using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour, IEnemy
{
    [Header("Statistiche del PowerUp")]
    [SerializeField] private float _maxHP = 100f;

    // Implementazione della proprietà richiesta dall'interfaccia IEnemy
    public float maxHP { get { return _maxHP; } }

    private float currentHP;
    public float regenRate = 5f;
    public float regenDelay = 3f;
    private float lastDamageTime;

    private ScoreManager scoreManager;
    private LaserEnergyManager energyManager;
    private SettingsManager settingsManager;
    private PowerUpQuestionUI questionUI;
    private string pendingPowerUp;

    // Liste dei power-up
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

    // Variabile per memorizzare il colore dell'ultimo laser che ha inflitto danno
    private string lastHitByColor;

    void Start()
    {
        // Inizializza currentHP con il valore di maxHP definito in Inspector
        currentHP = _maxHP;
        scoreManager = FindObjectOfType<ScoreManager>();
        energyManager = FindObjectOfType<LaserEnergyManager>();
        settingsManager = FindObjectOfType<SettingsManager>();
        AdjustHitbox();
        questionUI = FindObjectOfType<PowerUpQuestionUI>();

    }

    void Update()
    {
        if (currentHP < _maxHP && Time.time > lastDamageTime + regenDelay)
        {
            currentHP += regenRate * Time.deltaTime;
            currentHP = Mathf.Clamp(currentHP, 0, _maxHP);
        }
    }

    public void TakeDamage(float damage, string color)
    {
        lastHitByColor = color;
        currentHP -= damage;
        lastDamageTime = Time.time;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
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

        questionUI.ShowQuestion(this);  // Mostra la domanda
    }


    private void AdjustHitbox()
    {
        if (settingsManager != null)
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.size = Vector3.one * settingsManager.enemyHitboxSize;
            }
        }
    }
    public void GrantPowerUp()
    {
        if (energyManager != null && pendingPowerUp != "")
        {
            energyManager.ApplyPowerUp(pendingPowerUp);
        }
        Destroy(gameObject);
    }

    public float GetCurrentHP()
    {
        return currentHP;
    }
}
