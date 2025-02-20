using UnityEngine;
using UnityEngine.InputSystem;

public class LaserGunBlueTouch : MonoBehaviour
{
    public GameObject laserPrefab;
    public Transform laserEmitter;
    
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
        // Aggiorna la posizione base e la rotazione dell'emitter
        laserEmitter.position = transform.position + transform.right * -0.5f + transform.up * -0.2f;
        laserEmitter.rotation = transform.rotation;

        bool foundBlueEnemy = false;
        int layerMask = ~LayerMask.GetMask("IgnoreTouch");

        // Per ogni tocco attivo, eseguiamo un raycast dalla posizione del tocco
        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed)
                {
                    Vector2 touchPos = touch.position.ReadValue();
                    Ray ray = Camera.main.ScreenPointToRay(touchPos);
                    if (Physics.Raycast(ray, out RaycastHit hit, maxLaserDistance, layerMask))
                    {
                        // Se il tocco ha colpito un nemico blu...
                        EnemyTypeBTouch enemy = hit.collider.GetComponentInParent<EnemyTypeBTouch>();
                        if (enemy != null)
                        {
                            foundBlueEnemy = true;
                            if (energyManager.UseBlueLaser())
                            {
                                if (currentLaser == null)
                                {
                                    currentLaser = Instantiate(laserPrefab, laserEmitter.position, laserEmitter.rotation);
                                    currentLaser.transform.SetParent(laserEmitter);
                                    ParticleSystem ps = currentLaser.GetComponent<ParticleSystem>();
                                    if (ps != null)
                                    {
                                        ps.Play();
                                    }
                                }
                                FireLaser(hit);
                                // Applica il danno al nemico blu
                               
                                
                            }
                            else
                            {
                                StopLaser();
                            }
                            break; // elaboriamo solo il primo tocco valido
                        }
                    }
                }
            }
        }

        if (!foundBlueEnemy)
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

    // Ruota l'emitter affinché il laser "snappi" al centro del nemico colpito
    void FireLaser(RaycastHit hit)
    {
        Vector3 targetPoint = hit.point;
        EnemyTypeBTouch enemy = hit.collider.GetComponentInParent<EnemyTypeBTouch>();
        if (enemy != null)
        {
            targetPoint = enemy.transform.position;
        }
        Vector3 direction = (targetPoint - laserEmitter.position).normalized;
        laserEmitter.rotation = Quaternion.LookRotation(direction);
    }

    void StopLaser()
    {
        if (currentLaser != null)
        {
            ParticleSystem ps = currentLaser.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop();
                Destroy(currentLaser, ps.main.duration);
            }
            else
            {
                Destroy(currentLaser);
            }
            currentLaser = null;
        }
    }
}
