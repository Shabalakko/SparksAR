using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab del proiettile
    public float bulletSpeed = 50f; // Velocit� del proiettile
    public float fireRate = 0.5f; // Tempo tra uno sparo e l'altro
    private float nextTimeToFire = 0f;

    void Update()
    {
        // Verifica se il tasto sinistro del mouse � premuto e rispetta il rateo di fuoco
        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        // Istanzia il proiettile al centro della telecamera
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);

        // Applica la velocit� al proiettile
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * bulletSpeed;
    }
}
