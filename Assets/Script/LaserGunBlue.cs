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
            StopLaser();
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

            EnemyTypeB enemy = centerHit.collider.GetComponentInParent<EnemyTypeB>();
            if (enemy != null)
            {
                targetPoint = enemy.transform.position;
                enemy.TakeDamage(laserDamage * Time.deltaTime, "Blue");
                FindObjectOfType<ScoreManager>().OnHit();
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
