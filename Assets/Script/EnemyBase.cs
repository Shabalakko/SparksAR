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

    [SerializeField] protected Mesh[] _possibleMeshes; // Array di mesh da assegnare nell'Inspector
    protected MeshFilter _meshFilter;

    private SpawnDeathSound _spawnDeathSound; // Riferimento allo script SpawnDeathSound

    protected virtual void Start()
    {
        currentHP = _maxHP;
        scoreManager = FindObjectOfType<ScoreManager>();
        energyManager = FindObjectOfType<LaserEnergyManager>();
        settingsManager = FindObjectOfType<SettingsManager>();
        _meshFilter = GetComponent<MeshFilter>();
        if (_meshFilter != null && _possibleMeshes != null && _possibleMeshes.Length > 0)
        {
            int randomIndex = Random.Range(0, _possibleMeshes.Length);
            _meshFilter.mesh = _possibleMeshes[randomIndex];
        }
        //AdjustHitbox();

        // Ottieni il componente SpawnDeathSound
        _spawnDeathSound = GetComponent<SpawnDeathSound>();
        if (_spawnDeathSound == null)
        {
            Debug.LogError("SpawnDeathSound non trovato nel GameObject!", gameObject);
        }
    }

    protected virtual void Update()
    {
        if (currentHP < maxHP && Time.time > lastDamageTime + regenDelay)
        {
            currentHP += regenRate * Time.deltaTime;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        }
        if (currentHP <= 0)
        {
            Die();
        }
    }

    public float GetCurrentHP()
    {
        return currentHP;
    }

    // Ogni nemico dovrà implementare come reagire al danno
    public abstract void TakeDamage(float damage, string color);

    /// <summary>
    /// Metodo comune da chiamare quando il nemico muore.
    /// Usa il proprio EnemyColor per registrarsi al punteggio.
    /// </summary>
    protected virtual void Die()
    {
        // Riproduci il suono di morte usando lo script SpawnDeathSound
        if (_spawnDeathSound != null)
        {
            _spawnDeathSound.PlaySound();
        }

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
                scoreManager.AddScore(EnemyColor);
            }
        }
        Destroy(gameObject); // Distruggi il nemico immediatamente
    }
}
