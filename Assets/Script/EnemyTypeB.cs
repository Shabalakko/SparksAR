using UnityEngine;

public class EnemyTypeB : EnemyBase
{
    // Specifica il colore di questo nemico
    public override string EnemyColor => "Blue";

    public override void TakeDamage(float damage, string color)
    {
        if (color == EnemyColor)
        {
            currentHP -= damage;
            lastDamageTime = Time.time;
            if (currentHP <= 0)
            {
                // Se il nemico blu muore, ricarica l'energia rossa
                if (energyManager != null)
                {
                    energyManager.RechargeRed();
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
