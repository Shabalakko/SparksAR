using UnityEngine;
using UnityEngine.InputSystem;

public class LaserGun : MonoBehaviour
{
    public GameObject laserPrefab;          // Prefab del Particle System per il laser rosso
    public Transform laserEmitter;           // Punto di partenza del laser decentrato
    public float laserDamage = 10f;          // Danno per secondo
    public float maxLaserDistance = 50f;     // Distanza massima del laser

    private GameObject currentLaser;
    private LaserEnergyManager energyManager;
    private PlayerInputActions inputActions;
    private bool isShooting = false;

    void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Shooting.ShootRed.performed += ctx => StartShooting();
        inputActions.Shooting.ShootRed.canceled += ctx => StopShooting();
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Start()
    {
        energyManager = FindObjectOfType<LaserEnergyManager>();
    }

    void Update()
    {
        // Mantiene l'emettitore decentrato rispetto alla telecamera
        laserEmitter.position = transform.position + transform.right * 0.5f + transform.up * -0.2f;
        laserEmitter.rotation = transform.rotation;

        // Se si sta sparando, gestisci l'emissione e il danno
        if (isShooting)
        {
            // Verifica l'energia prima di sparare
            if (energyManager.UseRedLaser())
            {
                if (currentLaser == null)
                {
                    currentLaser = Instantiate(laserPrefab, laserEmitter.position, laserEmitter.rotation);
                    currentLaser.transform.SetParent(laserEmitter);
                    currentLaser.GetComponent<ParticleSystem>().Play();
                }

                FireLaser();
            }
            else
            {
                StopLaser();
            }
        }
    }

    void StartShooting()
    {
        isShooting = true;
    }

    void StopShooting()
    {
        isShooting = false;
        StopLaser();
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

            Enemy enemy = laserHit.collider.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(laserDamage * Time.deltaTime, "Red");
            }
        }
    }

    void StopLaser()
    {
        if (currentLaser != null)
        {
            currentLaser.GetComponent<ParticleSystem>().Stop();
            Destroy(currentLaser, currentLaser.GetComponent<ParticleSystem>().main.duration);
            currentLaser = null;
        }
    }
}
