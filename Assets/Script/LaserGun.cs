using UnityEngine;
using UnityEngine.InputSystem;

public class LaserGun : MonoBehaviour
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
        laserEmitter.position = transform.position + transform.right * 0.5f + transform.up * -0.2f;
        laserEmitter.rotation = transform.rotation;

        // --- Controllo su PC (Mouse) ---
        if (Application.platform != RuntimePlatform.Android)
        {
            if (isShooting)
            {
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
        // --- Controllo su Android (Multi-Touch) ---
        else
        {
            bool isTouchingRight = false;

            // Verifica tutti i tocchi attivi
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed)
                {
                    Vector2 touchPosition = touch.position.ReadValue();

                    // Se almeno un tocco è sul lato destro
                    if (touchPosition.x > Screen.width / 2)
                    {
                        isTouchingRight = true;
                        break;
                    }
                }
            }

            // Spara se almeno un dito è sul lato destro
            if (isTouchingRight)
            {
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
            else
            {
                StopLaser(); // Ferma il laser se nessun dito è sul lato destro
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

            Enemy enemy = centerHit.collider.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                targetPoint = enemy.transform.position;
                enemy.TakeDamage(laserDamage * Time.deltaTime, "Red");
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
