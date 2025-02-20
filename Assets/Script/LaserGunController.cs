using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class LaserGunController : MonoBehaviour
{
    [Header("Laser Settings")]
    public GameObject blueLaserPrefab;
    public GameObject redLaserPrefab;
    public Transform blueLaserEmitter; // Es. posizione per il laser blu (mano sinistra)
    public Transform redLaserEmitter;  // Es. posizione per il laser rosso (mano destra)
    public float laserDamage = 10f;
    public float maxLaserDistance = 50f;

    private GameObject currentBlueLaser;
    private GameObject currentRedLaser;
    private LaserEnergyManager energyManager;

    private PlayerInputActions inputActions; // Se usi azioni specifiche per sparare, altrimenti puoi rilevare il tocco in Update

    void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Enable();
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
        EnhancedTouchSupport.Disable();
    }

    void Start()
    {
        energyManager = FindObjectOfType<LaserEnergyManager>();
    }

    void Update()
    {
        // Se almeno un tocco è attivo, procedi con il firing.
        bool isShooting = false;
        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed)
                {
                    isShooting = true;
                    break;
                }
            }
        }

        if (isShooting)
        {
            FireLaser();
        }
        else
        {
            StopLaser("Blue");
            StopLaser("Red");
        }
    }

    void FireLaser()
    {
        // Esegui un raycast dalla posizione della camera (o da un punto definito) in avanti
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        Vector3 targetPoint = ray.origin + ray.direction * maxLaserDistance;
        string enemyColor = "";

        if (Physics.Raycast(ray, out hit, maxLaserDistance))
        {
            targetPoint = hit.point;
            // Controlla se il collider appartiene a un nemico blu o rosso
            if (hit.collider.gameObject.GetComponentInParent<EnemyTypeBTouch>() != null)
            {
                enemyColor = "Blue";
                // Per far "snappare" il laser, usiamo la posizione del pivot del nemico
                targetPoint = hit.collider.gameObject.GetComponentInParent<EnemyTypeBTouch>().transform.position;
            }
            else if (hit.collider.gameObject.GetComponentInParent<EnemyTouch>() != null)
            {
                enemyColor = "Red";
                targetPoint = hit.collider.gameObject.GetComponentInParent<EnemyTouch>().transform.position;
            }
        }

        // In base al colore rilevato, attiva il laser corrispondente
        if (enemyColor == "Blue")
        {
            // Consuma energia per il laser blu
            if (energyManager.UseBlueLaser())
            {
                // Imposta emitter: ad esempio, posiziona l'emitter in base a un offset (qui a sinistra)
                blueLaserEmitter.position = transform.position + transform.right * -0.5f + transform.up * -0.2f;
                // Calcola la direzione
                Vector3 direction = (targetPoint - blueLaserEmitter.position).normalized;
                blueLaserEmitter.rotation = Quaternion.LookRotation(direction);

                // Istanzia (se non già attivo) il laser blu
                if (currentBlueLaser == null)
                {
                    currentBlueLaser = Instantiate(blueLaserPrefab, blueLaserEmitter.position, blueLaserEmitter.rotation);
                    currentBlueLaser.transform.SetParent(blueLaserEmitter);
                    ParticleSystem ps = currentBlueLaser.GetComponent<ParticleSystem>();
                    if (ps != null)
                        ps.Play();
                }

                // Applica danni al nemico (se presente)
                EnemyTypeBTouch enemy = hit.collider.gameObject.GetComponentInParent<EnemyTypeBTouch>();
                if (enemy != null)
                {
                    enemy.TakeDamage(laserDamage * Time.deltaTime, "Blue");
                    ScoreManager sm = FindObjectOfType<ScoreManager>();
                    if (sm != null)
                        sm.OnHit();
                }
            }
            else
            {
                StopLaser("Blue");
            }
            // Assicurati che il laser rosso sia spento
            StopLaser("Red");
        }
        else if (enemyColor == "Red")
        {
            if (energyManager.UseRedLaser())
            {
                redLaserEmitter.position = transform.position + transform.right * 0.5f + transform.up * -0.2f;
                Vector3 direction = (targetPoint - redLaserEmitter.position).normalized;
                redLaserEmitter.rotation = Quaternion.LookRotation(direction);

                if (currentRedLaser == null)
                {
                    currentRedLaser = Instantiate(redLaserPrefab, redLaserEmitter.position, redLaserEmitter.rotation);
                    currentRedLaser.transform.SetParent(redLaserEmitter);
                    ParticleSystem ps = currentRedLaser.GetComponent<ParticleSystem>();
                    if (ps != null)
                        ps.Play();
                }

                EnemyTouch enemy = hit.collider.gameObject.GetComponentInParent<EnemyTouch>();
                if (enemy != null)
                {
                    enemy.TakeDamage(laserDamage * Time.deltaTime, "Red");
                    ScoreManager sm = FindObjectOfType<ScoreManager>();
                    if (sm != null)
                        sm.OnHit();
                }
            }
            else
            {
                StopLaser("Red");
            }
            StopLaser("Blue");
        }
        else
        {
            // Se il raycast non colpisce un nemico, ferma entrambi i laser
            StopLaser("Blue");
            StopLaser("Red");
        }
    }

    void StopLaser(string color)
    {
        if (color == "Blue")
        {
            if (currentBlueLaser != null)
            {
                ParticleSystem ps = currentBlueLaser.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Stop();
                    Destroy(currentBlueLaser, ps.main.duration);
                }
                else
                {
                    Destroy(currentBlueLaser);
                }
                currentBlueLaser = null;
            }
        }
        else if (color == "Red")
        {
            if (currentRedLaser != null)
            {
                ParticleSystem ps = currentRedLaser.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Stop();
                    Destroy(currentRedLaser, ps.main.duration);
                }
                else
                {
                    Destroy(currentRedLaser);
                }
                currentRedLaser = null;
            }
        }
    }
}
