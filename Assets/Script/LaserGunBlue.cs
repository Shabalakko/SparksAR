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
        inputActions.Shooting.ShootBlue.performed += OnShootBluePerformed;
        inputActions.Shooting.ShootBlue.canceled += OnShootBlueCanceled;
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Shooting.ShootBlue.performed -= OnShootBluePerformed;
        inputActions.Shooting.ShootBlue.canceled -= OnShootBlueCanceled;
        inputActions.Disable();
    }

    void Start()
    {
        energyManager = FindObjectOfType<LaserEnergyManager>();
    }

    void Update()
    {
        // Aggiorna la posizione e rotazione dell'emettitore laser
        laserEmitter.position = transform.position + transform.right * -0.5f + transform.up * -0.2f;
        laserEmitter.rotation = transform.rotation;

        // Supporto touchscreen: se viene toccato il lato sinistro dello schermo, attiva lo sparo
        bool isTouchingLeft = false;
        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed)
                {
                    Vector2 touchPosition = touch.position.ReadValue();
                    if (touchPosition.x < Screen.width / 2)
                    {
                        isTouchingLeft = true;
                        break;
                    }
                }
            }
        }

        // Se si sta sparando tramite il mouse (o touchscreen) e c'è energia disponibile
        if (isTouchingLeft || isShooting)
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
            StopLaser();
        }
    }

    void OnShootBluePerformed(InputAction.CallbackContext context)
    {
        StartShooting();
    }

    void OnShootBlueCanceled(InputAction.CallbackContext context)
    {
        StopShooting();
    }

    void StartShooting()
    {
        isShooting = true;
    }

    public void StopShooting()
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
            // Controlla se il raycast ha colpito un nemico di tipo B
            EnemyTypeB enemy = centerHit.collider.GetComponentInParent<EnemyTypeB>();
            if (enemy != null)
            {
                targetPoint = enemy.transform.position;
                enemy.TakeDamage(laserDamage * Time.deltaTime, "Blue");
                FindObjectOfType<ScoreManager>().OnHit();
            }
            // Altrimenti, controlla se il collider ha il tag "PowerUp"
            else if (centerHit.collider.CompareTag("PowerUp"))
            {
                PowerUp powerUp = centerHit.collider.GetComponentInParent<PowerUp>();
                if (powerUp != null)
                {
                    targetPoint = powerUp.transform.position;
                    powerUp.TakeDamage(laserDamage * Time.deltaTime, "Blue");
                    FindObjectOfType<ScoreManager>().OnHit();
                }
                else
                {
                    targetPoint = centerHit.point;
                }
            }
            else
            {
                targetPoint = centerHit.point;
            }
        }
        else
        {
            targetPoint = centerRay.origin + centerRay.direction * maxLaserDistance;
        }

        // Aggiorna la direzione dell'emettitore in base al punto target
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
