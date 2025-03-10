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
}
