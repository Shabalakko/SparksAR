using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IEnemy
{
    [SerializeField] protected float _maxHP = 100f;
    public virtual float maxHP { get { return _maxHP; } }
    protected float currentHP;
    public float regenRate = 5f;
    public float regenDelay = 3f;
    protected float lastDamageTime;

    protected ScoreManager scoreManager;
    protected LaserEnergyManager energyManager;
    protected SettingsManager settingsManager;

    // IEnemy richiede la proprietà per il colore
    public abstract string EnemyColor { get; }

    protected virtual void Start()
    {
        currentHP = _maxHP;
        scoreManager = FindObjectOfType<ScoreManager>();
        energyManager = FindObjectOfType<LaserEnergyManager>();
        settingsManager = FindObjectOfType<SettingsManager>();
        AdjustHitbox();
    }

    protected virtual void Update()
    {
        if (currentHP < maxHP && Time.time > lastDamageTime + regenDelay)
        {
            currentHP += regenRate * Time.deltaTime;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        }
    }

    public float GetCurrentHP()
    {
        return currentHP;
    }

    protected virtual void AdjustHitbox()
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

    // Ogni nemico dovrà implementare come reagire al danno
    public abstract void TakeDamage(float damage, string color);

    /// <summary>
    /// Metodo comune da chiamare quando il nemico muore.
    /// Usa il proprio EnemyColor per registrarsi al punteggio.
    /// </summary>
    protected virtual void Die()
    {
        if (scoreManager != null)
        {
            // Se stai usando la modalità ColorSlots, lascia che sia il sistema di combo a gestire il popup.
            if (scoreManager.scoringMode == ScoringMode.Combo)
            {
                int scoreBefore = scoreManager.GetTotalScore();
                scoreManager.AddScore(EnemyColor);
                int scoreGained = scoreManager.GetTotalScore() - scoreBefore;
                scoreManager.ShowScorePopup(scoreGained);
            }
            else // Modalità ColorSlots
            {
                // In modalità combo basata su combinazioni, aggiungi lo score
                // e lascia che EvaluateColorCombo gestisca il popup.
                scoreManager.AddScore(EnemyColor);
            }
        }
        Destroy(gameObject);
    }


}
