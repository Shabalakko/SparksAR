using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHP = 100f;
    private float currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    // Metodo per ricevere danni
    public void TakeDamage(float damage)
    {
        currentHP -= damage;

        // Se gli HP scendono a zero, distruggi il nemico
        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
