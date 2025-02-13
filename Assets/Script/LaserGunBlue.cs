using UnityEngine;

public class LaserGunBlue : MonoBehaviour
{
    public GameObject laserPrefab;          // Prefab del Particle System per il laser blu
    public Transform laserEmitter;           // Punto di partenza del laser decentrato
    public float laserDamage = 10f;          // Danno per secondo
    public float maxLaserDistance = 50f;     // Distanza massima del laser

    private GameObject currentLaser;
    private LaserEnergyManager energyManager;

    void Start()
    {
        // Trova l'istanza del LaserEnergyManager
        energyManager = FindObjectOfType<LaserEnergyManager>();
    }

    void Update()
    {
        laserEmitter.position = transform.position + transform.right * -0.5f + transform.up * -0.2f;
        laserEmitter.rotation = transform.rotation;

        if (Input.GetMouseButtonDown(1))
        {
            if (energyManager.UseBlueLaser())
            {
                currentLaser = Instantiate(laserPrefab, laserEmitter.position, laserEmitter.rotation);
                currentLaser.transform.SetParent(laserEmitter);
                currentLaser.GetComponent<ParticleSystem>().Play();
            }
        }

        if (Input.GetMouseButtonUp(1) && currentLaser != null)
        {
            StopLaser();
        }

        // Se l'energia è a zero, ferma l'emissione
        if (energyManager.GetBlueEnergy() <= 0 && currentLaser != null)
        {
            StopLaser();
        }

        if (Input.GetMouseButton(1))
        {
            if (energyManager.UseBlueLaser())
            {
                FireLaser();
            }
        }
    }

    // Funzione per fermare e distruggere il laser
    void StopLaser()
    {
        currentLaser.GetComponent<ParticleSystem>().Stop();
        Destroy(currentLaser, currentLaser.GetComponent<ParticleSystem>().main.duration);
        currentLaser = null;
    }


    void FireLaser()
    {
        Ray centerRay = new Ray(transform.position, transform.forward);
        RaycastHit centerHit;
        Vector3 targetPoint;

        if (Physics.Raycast(centerRay, out centerHit, maxLaserDistance))
        {
            targetPoint = centerHit.point;
        }
        else
        {
            targetPoint = centerRay.origin + centerRay.direction * maxLaserDistance;
        }

        Vector3 direction = (targetPoint - laserEmitter.position).normalized;
        Ray laserRay = new Ray(laserEmitter.position, direction);
        RaycastHit laserHit;

        if (Physics.Raycast(laserRay, out laserHit, maxLaserDistance))
        {
            laserEmitter.rotation = Quaternion.LookRotation(direction);

            EnemyTypeB enemy = laserHit.collider.GetComponentInParent<EnemyTypeB>();
            if (enemy != null)
            {
                enemy.TakeDamage(laserDamage * Time.deltaTime, "Blue");
            }
        }
    }
}
