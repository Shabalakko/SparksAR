using UnityEngine;
using UnityEngine.InputSystem;

public class LaserGunBlue : MonoBehaviour
{
    public GameObject laserPrefab;
    public Transform laserEmitter;
    public float laserDamage = 10f;
    public float maxLaserDistance = 50f;

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
        inputActions.Shooting.ShootBlue.performed += ctx => StartShooting();
        inputActions.Shooting.ShootBlue.canceled += ctx => StopShooting();

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
        laserEmitter.position = transform.position + transform.right * -0.5f + transform.up * -0.2f;
        laserEmitter.rotation = transform.rotation;

        // --- Controllo su PC (Mouse) ---
        if (Application.platform != RuntimePlatform.Android)
        {
            if (isShooting)
            {
                if (energyManager.UseBlueLaser())
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
        // --- Controllo su Android (Multi-Touch) ---
        else
        {
            bool isTouchingLeft = false;

            // Verifica tutti i tocchi attivi
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed)
                {
                    Vector2 touchPosition = touch.position.ReadValue();

                    // Se almeno un tocco è sul lato sinistro
                    if (touchPosition.x < Screen.width / 2)
                    {
                        isTouchingLeft = true;
                        break;
                    }
                }
            }

            // Spara se almeno un dito è sul lato sinistro
            if (isTouchingLeft)
            {
                if (energyManager.UseBlueLaser())
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
            else
            {
                StopLaser(); // Ferma il laser se nessun dito è sul lato sinistro
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

            // Se colpisce un nemico, punta al suo pivot
            EnemyTypeB enemy = centerHit.collider.GetComponentInParent<EnemyTypeB>();
            if (enemy != null)
            {
                targetPoint = enemy.transform.position;
                enemy.TakeDamage(laserDamage * Time.deltaTime, "Blue");
                FindObjectOfType<ScoreManager>().OnHit(); // Rallenta il timer della combo
            }
        }
        else
        {
            targetPoint = centerRay.origin + centerRay.direction * maxLaserDistance;
        }

        Vector3 direction = (targetPoint - laserEmitter.position).normalized;
        laserEmitter.rotation = Quaternion.LookRotation(direction);
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
