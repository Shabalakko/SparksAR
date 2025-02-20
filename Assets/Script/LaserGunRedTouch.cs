using UnityEngine;
using UnityEngine.InputSystem;

public class LaserGunRedTouch : MonoBehaviour
{
    public GameObject laserPrefab;
    public Transform laserEmitter;
    public float maxLaserDistance = 50f;

    private GameObject currentLaser;
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

    void Update()
    {
        // Aggiorna la posizione e la rotazione base dell'emitter
        laserEmitter.position = transform.position + transform.right * 0.5f + transform.up * -0.2f;
        laserEmitter.rotation = transform.rotation;

        bool foundRedEnemy = false;
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
                        // Se il tocco ha colpito un nemico rosso (con componente EnemyTouch)
                        EnemyTouch enemy = hit.collider.GetComponentInParent<EnemyTouch>();
                        if (enemy != null)
                        {
                            foundRedEnemy = true;
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
                            break; // elaboriamo il primo tocco valido
                        }
                    }
                }
            }
        }

        if (!foundRedEnemy)
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

    // Ruota l'emitter in modo che il laser "snappi" al centro del nemico colpito
    void FireLaser(RaycastHit hit)
    {
        Vector3 targetPoint = hit.point;
        EnemyTouch enemy = hit.collider.GetComponentInParent<EnemyTouch>();
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
