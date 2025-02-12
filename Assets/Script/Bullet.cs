using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 5f; // Durata del proiettile in secondi

    void Start()
    {
        // Distrugge il proiettile dopo un tot di secondi
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Se il proiettile colpisce un nemico, si distrugge
        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }

}
