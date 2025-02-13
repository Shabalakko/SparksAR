using UnityEngine;

public class Enemy : MonoBehaviour, IEnemy
{
    [Header("Statistiche del Nemico")]
    [SerializeField] private float _maxHP = 100f;
    public float maxHP { get { return _maxHP; } }

    private float currentHP;
    public float regenRate = 5f;
    public float regenDelay = 3f;
    private float lastDamageTime;

    private ScoreManager scoreManager;
    private LaserEnergyManager energyManager;

    void Start()
    {
        currentHP = maxHP;

        // Trova ScoreManager e LaserEnergyManager nella scena
        scoreManager = FindObjectOfType<ScoreManager>();
        energyManager = FindObjectOfType<LaserEnergyManager>();
    }

    void Update()
    {
        if (currentHP < maxHP && Time.time > lastDamageTime + regenDelay)
        {
            currentHP += regenRate * Time.deltaTime;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        }
    }

    public void TakeDamage(float damage, string color)
    {
        if (color == "Red")
        {
            currentHP -= damage;
            lastDamageTime = Time.time;

            if (currentHP <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        // Aggiunge punteggio e ricarica il laser blu
        if (scoreManager != null)
        {
            scoreManager.AddScore("Red");
        }

        if (energyManager != null)
        {
            energyManager.RechargeBlue();
        }

        // Distrugge il GameObject dopo aver notificato
        Destroy(gameObject);
    }

    public float GetCurrentHP()
    {
        return currentHP;
    }
}
