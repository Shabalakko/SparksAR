using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyHealthBar healthBar;
    public float maxHP = 100f;
    private float currentHP;
    public float regenRate = 5f;         // HP rigenerati per secondo
    public float regenDelay = 3f;        // Tempo di attesa prima di iniziare la rigenerazione

    private float lastDamageTime;        // Tempo dell'ultimo danno subito

    void Start()
    {
        currentHP = maxHP;
        healthBar = GetComponentInChildren<EnemyHealthBar>();
    }

    void Update()
    {
        // Se il nemico è danneggiato e non ha subito danni di recente, rigenera gli HP
        if (currentHP < maxHP && Time.time > lastDamageTime + regenDelay)
        {
            currentHP += regenRate * Time.deltaTime;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);

            // Aggiorna la barra degli HP durante la rigenerazione
            healthBar.UpdateHealthBar();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        lastDamageTime = Time.time;  // Registra l'ultimo danno subito

        // Comunica all'HPBar di aggiornarsi
        healthBar.TakeDamage();

        // Se gli HP scendono a zero, distruggi il nemico
        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    public float GetCurrentHP()
    {
        return currentHP;
    }
}
