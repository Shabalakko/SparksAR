using UnityEngine;
using System.Collections;

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
    public abstract string EnemyColor { get; }
    [SerializeField] protected Mesh[] _possibleMeshes;
    protected MeshFilter _meshFilter;
    private SpawnDeathSound _spawnDeathSound;
    [SerializeField] public GameObject particellePrefab; // Moved to EnemyBase
    private bool _isDead = false;

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
        if (currentHP <= 0 && !_isDead)
        {
            Die();
        }
    }

    public float GetCurrentHP()
    {
        return currentHP;
    }

    public abstract void TakeDamage(float damage, string color);

    protected virtual void Die()
    {
        _isDead = true;
        if (_spawnDeathSound != null)
        {
            _spawnDeathSound.PlaySound();
        }
        if (particellePrefab != null)
        {
            GameObject effetto = Instantiate(particellePrefab, transform.position, Quaternion.identity);
            ParticleSystem ps = effetto.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(effetto, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(effetto, 2f);
            }
        }
        if (scoreManager != null)
        {
            if (scoreManager.scoringMode == ScoringMode.Combo)
            {
                int scoreBefore = scoreManager.GetTotalScore();
                scoreManager.AddScore(EnemyColor);
                int scoreGained = scoreManager.GetTotalScore() - scoreBefore;
                scoreManager.ShowScorePopup(scoreGained);
            }
            else
            {
                scoreManager.AddScore(EnemyColor);
            }
        }
        Destroy(gameObject);
    }
}