using UnityEngine;

public class LaserGun : MonoBehaviour
{
    public GameObject laserPrefab;          // Prefab del Particle System per il laser
    public Transform laserEmitter;          // Punto di partenza del laser decentrato
    public float laserDamage = 10f;          // Danno per secondo
    public float maxLaserDistance = 50f;     // Distanza massima del laser

    private GameObject currentLaser;         // Istanza corrente del laser

    void Update()
    {
        // Mantiene l'emettitore decentrato rispetto alla telecamera
        laserEmitter.position = transform.position + transform.right * 0.5f + transform.up * -0.2f;
        laserEmitter.rotation = transform.rotation;

        // Se il tasto sinistro del mouse è premuto, spara il laser
        if (Input.GetMouseButtonDown(0))
        {
            // Istanzia un nuovo Particle System dal prefab
            currentLaser = Instantiate(laserPrefab, laserEmitter.position, laserEmitter.rotation);
            currentLaser.transform.SetParent(laserEmitter); // Lo attacca al LaserEmitter
            currentLaser.GetComponent<ParticleSystem>().Play();
        }

        // Se il tasto viene rilasciato, ferma l'emissione e autodistrugge il Particle System
        if (Input.GetMouseButtonUp(0) && currentLaser != null)
        {
            // Ferma l'emissione delle particelle
            currentLaser.GetComponent<ParticleSystem>().Stop();

            // Distrugge l'istanza quando ha finito di emettere particelle
            Destroy(currentLaser, currentLaser.GetComponent<ParticleSystem>().main.duration);
        }

        // Esegue sempre i Raycast mentre il tasto è premuto
        if (Input.GetMouseButton(0))
        {
            FireLaser();
        }
    }

    void FireLaser()
    {
        // PRIMO RAYCAST: Dal centro della telecamera per capire dove stai mirando
        Ray centerRay = new Ray(transform.position, transform.forward);
        RaycastHit centerHit;
        Vector3 targetPoint;

        if (Physics.Raycast(centerRay, out centerHit, maxLaserDistance))
        {
            // Se colpisce qualcosa, il punto di destinazione è il punto di impatto
            targetPoint = centerHit.point;
        }
        else
        {
            // Se non colpisce nulla, il punto di destinazione è il massimo raggio d'azione
            targetPoint = centerRay.origin + centerRay.direction * maxLaserDistance;
        }

        // SECONDO RAYCAST: Dal LaserEmitter verso il punto colpito dal primo Raycast
        Vector3 direction = (targetPoint - laserEmitter.position).normalized;
        Ray laserRay = new Ray(laserEmitter.position, direction);
        RaycastHit laserHit;

        if (Physics.Raycast(laserRay, out laserHit, maxLaserDistance))
        {
            // Se il secondo Raycast colpisce qualcosa, orienta il Particle System lì
            laserEmitter.rotation = Quaternion.LookRotation(direction);

            // Controlla se ha colpito un nemico
            Enemy enemy = laserHit.collider.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                // Infligge danno continuo in base al tempo
                enemy.TakeDamage(laserDamage * Time.deltaTime);
            }
        }
        else
        {
            // Se non colpisce nulla, il laser va alla massima distanza
            Vector3 endPoint = laserRay.origin + laserRay.direction * maxLaserDistance;
            laserEmitter.rotation = Quaternion.LookRotation(direction);
        }
    }
}
