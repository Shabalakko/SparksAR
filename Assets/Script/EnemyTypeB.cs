using UnityEngine;

public class EnemyTypeB : Enemy
{
    public override void TakeDamage(float damage, string laserType)
    {
        if (laserType == "Blue")
        {
            currentHP -= damage;
            lastDamageTime = Time.time;

            healthBar.TakeDamage();

            if (currentHP <= 0)
            {
                // Ricarica il Laser Rosso alla distruzione
                FindObjectOfType<LaserEnergyManager>().RechargeRed();

                // Aggiunge il punteggio per il nemico blu
                FindObjectOfType<ScoreManager>().AddScore("Blue");

                Destroy(gameObject);
            }
        }
    }

}
