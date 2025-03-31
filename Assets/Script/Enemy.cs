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
}
