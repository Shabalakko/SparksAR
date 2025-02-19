using UnityEngine;

public class EnemyTypeB : MonoBehaviour, IEnemy
{
    [Header("Statistiche del Nemico (Tipo B)")]
    [SerializeField] private float _maxHP = 100f;
    public float maxHP { get { return _maxHP; } }

    private float currentHP;
    public float regenRate = 5f;
    public float regenDelay = 3f;
    private float lastDamageTime;

    private ScoreManager scoreManager;
    private LaserEnergyManager energyManager;
    private SettingsManager settingsManager;

    void Start()
    {
        currentHP = maxHP;
        scoreManager = FindObjectOfType<ScoreManager>();
        energyManager = FindObjectOfType<LaserEnergyManager>();
        settingsManager = FindObjectOfType<SettingsManager>();

        // Applica la dimensione dell'hitbox basata sulle impostazioni
        AdjustHitbox();
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
        if (color == "Blue")
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
        if (scoreManager != null)
        {
            scoreManager.AddScore("Blue");
        }

        if (energyManager != null)
        {
            energyManager.RechargeRed();
        }

        Destroy(gameObject);
    }

    private void AdjustHitbox()
    {
        if (settingsManager != null)
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>(); // Casting esplicito
            if (boxCollider != null)
            {
                boxCollider.size = Vector3.one * settingsManager.enemyHitboxSize;
            }
        }
    }



    public float GetCurrentHP()
    {
        return currentHP;
    }
}
