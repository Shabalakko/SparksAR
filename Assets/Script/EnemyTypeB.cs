using UnityEngine;

public class EnemyTypeB : Enemy
{
    public override void TakeDamage(float damage, string laserType)
    {
        // Accetta danno solo dal laser blu
        if (laserType == "Blue")
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
    }
}
