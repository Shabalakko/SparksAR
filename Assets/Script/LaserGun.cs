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
        inputActions.Shooting.ShootRed.performed += OnShootRedPerformed;
        inputActions.Shooting.ShootRed.canceled += OnShootRedCanceled;
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Shooting.ShootRed.performed -= OnShootRedPerformed;
        inputActions.Shooting.ShootRed.canceled -= OnShootRedCanceled;
        inputActions.Disable();
    }

    void Start()
    {
        energyManager = FindObjectOfType<LaserEnergyManager>();
    }

    void Update()
    {
        // Aggiornamento della posizione e rotazione dell'emettitore laser
        laserEmitter.position = transform.position + transform.right * 0.5f + transform.up * -0.2f;
        laserEmitter.rotation = transform.rotation;

        // Supporto touchscreen: se viene toccato il lato destro dello schermo, attiva lo sparo
        bool isTouchingRight = false;
        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed)
                {
                    Vector2 touchPosition = touch.position.ReadValue();
                    if (touchPosition.x > Screen.width / 2)
                    {
                        isTouchingRight = true;
                        break;
                    }
                }
            }
        }

        // Se si sta sparando tramite il mouse (o touchscreen) e c'è energia disponibile
        if (isTouchingRight || isShooting)
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
            StopLaser();
        }
    }

    void OnShootRedPerformed(InputAction.CallbackContext context)
    {
        StartShooting();
    }

    void OnShootRedCanceled(InputAction.CallbackContext context)
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
            // Se colpisce un nemico, applica danno e registra il colpo
            Enemy enemy = centerHit.collider.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                targetPoint = enemy.transform.position;
                enemy.TakeDamage(laserDamage * Time.deltaTime, "Red");
                FindObjectOfType<ScoreManager>().OnHit();
            }
            // Se colpisce un PowerUp, applica danno
            else if (centerHit.collider.CompareTag("PowerUp"))
            {
                PowerUp powerUp = centerHit.collider.GetComponentInParent<PowerUp>();
                if (powerUp != null)
                {
                    targetPoint = powerUp.transform.position;
                    powerUp.TakeDamage(laserDamage * Time.deltaTime, "Red");
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
