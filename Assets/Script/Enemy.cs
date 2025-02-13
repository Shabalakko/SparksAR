using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Comportamento del Nemico")]
    public float orbitSpeed = 1f;             // Velocità dell'orbita
    public float ellipseWidth = 5f;           // Ampiezza dell'ellisse sull'asse X
    public float ellipseHeight = 3f;          // Ampiezza dell'ellisse sull'asse Z
    public float verticalAmplitude = 2f;      // Ampiezza verticale dell'ellisse (asse Y)
    public float rotationSpeed = 20f;         // Velocità di rotazione dell'ellisse
    public float transitionSpeed = 0.5f;      // Velocità di transizione tra le orbite

    [Header("Statistiche del Nemico")]
    public EnemyHealthBar healthBar;
    public float maxHP = 100f;
    protected float currentHP;
    public float regenRate = 5f;
    public float regenDelay = 3f;
    protected float lastDamageTime;

    private Vector3 startPosition;
    private Vector3 currentEllipse;
    private Vector3 nextEllipse;
    private float angle = 0f;
    private float rotationAngle = 0f;

    void Start()
    {
        // Memorizza la posizione iniziale
        startPosition = transform.position;

        // Imposta un'ellisse di partenza
        currentEllipse = CalculateEllipse();
        nextEllipse = currentEllipse;

        // Imposta gli HP iniziali
        currentHP = maxHP;
        healthBar = GetComponentInChildren<EnemyHealthBar>();
    }

    void Update()
    {
        // --- Movimento Ellittico su tutte le assi ---
        angle += orbitSpeed * Time.deltaTime;
        if (angle > 360f) angle -= 360f;

        rotationAngle += rotationSpeed * Time.deltaTime;
        if (rotationAngle > 360f) rotationAngle -= 360f;
        Quaternion rotation = Quaternion.Euler(0, rotationAngle, 0);

        Vector3 targetPosition = startPosition;
        targetPosition.x += Mathf.Cos(angle) * currentEllipse.x;
        targetPosition.y += Mathf.Sin(angle * 2) * currentEllipse.y;
        targetPosition.z += Mathf.Sin(angle) * currentEllipse.z;

        targetPosition = startPosition + rotation * (targetPosition - startPosition);
        transform.position = Vector3.Lerp(transform.position, targetPosition, transitionSpeed * Time.deltaTime);

        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, transitionSpeed * Time.deltaTime);
        }

        if (angle >= 359f)
        {
            currentEllipse = nextEllipse;
            nextEllipse = CalculateEllipse();
        }

        // --- Rigenerazione degli HP ---
        if (currentHP < maxHP && Time.time > lastDamageTime + regenDelay)
        {
            currentHP += regenRate * Time.deltaTime;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);

            healthBar.UpdateHealthBar();
        }
    }

    Vector3 CalculateEllipse()
    {
        return new Vector3(
            Random.Range(ellipseWidth * 0.8f, ellipseWidth * 1.2f),
            Random.Range(verticalAmplitude * 0.5f, verticalAmplitude),
            Random.Range(ellipseHeight * 0.8f, ellipseHeight * 1.2f)
        );
    }

    public virtual void TakeDamage(float damage, string laserType)


    {
        if (laserType == "Red")
        {
            currentHP -= damage;
            lastDamageTime = Time.time;

            healthBar.TakeDamage();

            if (currentHP <= 0)
            {
                // Ricarica il Laser Blu alla distruzione
                FindObjectOfType<LaserEnergyManager>().RechargeBlue();

                // Aggiunge il punteggio per il nemico rosso
                FindObjectOfType<ScoreManager>().AddScore("Red");

                Destroy(gameObject);
            }
        }
    }


    public float GetCurrentHP()
    {
        return currentHP;
    }
}
