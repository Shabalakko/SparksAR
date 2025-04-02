using UnityEngine;

public class Enemy : EnemyBase
{
    // Specifica il colore di questo nemico
    public override string EnemyColor => "Red";

    public override void TakeDamage(float damage, string color)
    {
        // Il nemico subisce danno solo se il colore coincide
        if (color == EnemyColor)
        {
            currentHP -= damage;
            lastDamageTime = Time.time;
            if (currentHP <= 0)
            {
                // Se il nemico rosso muore, ad esempio ricarica l'energia blu
                if (energyManager != null)
                {
                    energyManager.RechargeBlue();
                }
                Die();
            }
        }
    }
    public GameObject particellePrefab; // Aggiungi questo campo e assegnagli il prefab tramite Inspector

    protected override void Die()
    {
        // Istanzia l'effetto particellare nella posizione del nemico
        if (particellePrefab != null)
        {
            GameObject effetto = Instantiate(particellePrefab, transform.position, Quaternion.identity);
            ParticleSystem ps = effetto.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                // Distruggi l'effetto dopo la sua durata
                Destroy(effetto, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(effetto, 2f);
            }
        }

        // Chiama la logica di morte originale (punteggio, popup, etc.) e distruggi il nemico
        base.Die();
    }

}
