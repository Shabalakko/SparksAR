using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using System.Collections.Generic;

public class EnemyTypeBTouch : MonoBehaviour, IEnemy
{
    [Header("Statistiche del Nemico")]
    [SerializeField] private float _maxHP = 100f;
    public float maxHP { get { return _maxHP; } }

    private float currentHP;
    public float regenRate = 5f;
    public float regenDelay = 3f;
    private float lastDamageTime;

    private ScoreManager scoreManager;
    private LaserEnergyManager energyManager;

    // Energia da dare al laser rosso quando il nemico blu viene sconfitto
    public float energyReward = 10f;

    private HashSet<int> activeTouches = new HashSet<int>();
    public float touchDamagePerSecond = 10f;
    public float energyCostPerSecond = 5f;

    void Start()
    {
        currentHP = maxHP;
        scoreManager = FindFirstObjectByType<ScoreManager>();
        energyManager = FindFirstObjectByType<LaserEnergyManager>();
        EnhancedTouchSupport.Enable();
    }

    void Update()
    {
        if (currentHP < maxHP && Time.time > lastDamageTime + regenDelay)
        {
            currentHP += regenRate * Time.deltaTime;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        }

        CheckActiveTouches();

        if (activeTouches.Count > 0)
        {
            ApplyTouchDamage();
        }
    }

    private void ApplyTouchDamage()
    {
        if (energyManager.UseBlueLaser())
        {
            TakeDamage(touchDamagePerSecond * Time.deltaTime, "Blue");
            scoreManager.OnHit(); //  Rallenta il timer della combo
        }
    }

    private void OnEnable()
    {
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += HandleTouchDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp += HandleTouchUp;
    }

    private void OnDisable()
    {
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= HandleTouchDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp -= HandleTouchUp;
    }

    private void HandleTouchDown(Finger finger)
    {
        TryAddTouch(finger);
    }

    private void HandleTouchUp(Finger finger)
    {
        activeTouches.Remove(finger.index);
    }

    private void CheckActiveTouches()
    {
        foreach (Finger finger in UnityEngine.InputSystem.EnhancedTouch.Touch.activeFingers)
        {
            TryAddTouch(finger);
        }
    }

    private void TryAddTouch(Finger finger)
    {
        // Accediamo alla fase del tocco tramite currentTouch
        if (finger.currentTouch.phase == UnityEngine.InputSystem.TouchPhase.Stationary ||
            finger.currentTouch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
        {
            if (IsTouchStillOnEnemy(finger))
            {
                activeTouches.Add(finger.index);
            }
            else
            {
                activeTouches.Remove(finger.index);
            }
        }
        else
        {
            activeTouches.Remove(finger.index);
        }
    }

    private bool IsTouchStillOnEnemy(Finger finger)
    {
        Ray ray = Camera.main.ScreenPointToRay(finger.screenPosition);
        int layerMask = ~LayerMask.GetMask("IgnoreTouch");

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            return hit.collider.gameObject == gameObject;
        }

        return false;
    }

    public void TakeDamage(float damage, string color)
    {
        currentHP -= damage;
        lastDamageTime = Time.time;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (scoreManager != null)
        {
            scoreManager.AddScore("Blue"); // Aggiunge combo per colore blu
        }
        // Aggiungi energia al laser rosso
        if (energyManager != null)
        {
            energyManager.AddRedEnergy(energyReward);
        }
        Destroy(gameObject);
    }

    public float GetCurrentHP()
    {
        return currentHP;
    }
}
