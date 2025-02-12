using UnityEngine;

public class LaserGun : MonoBehaviour
{
    public ParticleSystem laserParticles;   // Sistema di particelle per il laser
    public Transform laserEmitter;          // Punto di partenza del laser
    public float laserDamage = 10f;          // Danno per secondo
    public float maxLaserDistance = 50f;     // Distanza massima del laser

    private ParticleSystem.EmissionModule emissionModule;  // Emission Module per controllare le particelle

    void Start()
    {
        // Ottiene il modulo di emissione del Particle System
        emissionModule = laserParticles.emission;
        emissionModule.rateOverTime = 0; // Imposta l'emissione iniziale a zero
    }

    void Update()
    {
        // Mantiene l'emettitore davanti alla telecamera
        laserEmitter.position = transform.position + transform.forward * 0.5f;
        laserEmitter.rotation = transform.rotation;

        // Se il tasto sinistro del mouse è premuto, inizia a sparare
        if (Input.GetMouseButton(0))
        {
            // Attiva l'emissione delle particelle
            emissionModule.rateOverTime = 100;

            // Esegue sempre il Raycast mentre il tasto è premuto
            FireLaser();
        }
        else
        {
            // Se il tasto viene rilasciato, disattiva l'emissione
            emissionModule.rateOverTime = 0;
        }
    }

    void FireLaser()
    {
        // Emette un Raycast dalla telecamera verso la direzione in cui sta guardando
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxLaserDistance))
        {
            // Se colpisce qualcosa, il laser si ferma lì
            Vector3 direction = (hit.point - laserEmitter.position).normalized;
            laserEmitter.rotation = Quaternion.LookRotation(direction);

            // Controlla se ha colpito un nemico
            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                // Infligge danno continuo in base al tempo
                enemy.TakeDamage(laserDamage * Time.deltaTime);
            }
        }
        else
        {
            // Se non colpisce nulla, il laser va alla massima distanza
            Vector3 endPoint = ray.origin + ray.direction * maxLaserDistance;
            Vector3 direction = (endPoint - laserEmitter.position).normalized;
            laserEmitter.rotation = Quaternion.LookRotation(direction);
        }
    }
}
