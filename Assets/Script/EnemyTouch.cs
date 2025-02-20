using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using System.Collections.Generic;

public class EnemyTouch : MonoBehaviour, IEnemy
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

    private HashSet<int> activeTouches = new HashSet<int>(); // Tocchi attivi su questo nemico
    public float touchDamagePerSecond = 10f; // Danno per secondo

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

        //  Controlla costantemente se il tocco è ancora sopra il nemico
        CheckActiveTouches();

        if (activeTouches.Count > 0)
        {
            ApplyTouchDamage();
        }
    }

    private void ApplyTouchDamage()
    {
        TakeDamage(touchDamagePerSecond * Time.deltaTime, "Touch");
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

    //  Nuova funzione: Controlla costantemente se un dito è sopra il nemico
    private void CheckActiveTouches()
    {
        foreach (Finger finger in UnityEngine.InputSystem.EnhancedTouch.Touch.activeFingers)
        {
            TryAddTouch(finger);
        }
    }

    private void TryAddTouch(Finger finger)
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

    private bool IsTouchStillOnEnemy(Finger finger)
    {
        Ray ray = Camera.main.ScreenPointToRay(finger.screenPosition);
        int layerMask = ~LayerMask.GetMask("IgnoreTouch"); // Ignora il layer dello Spawner

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
            scoreManager.AddScore("Touch");
        }

        Destroy(gameObject);
    }

    public float GetCurrentHP()
    {
        return currentHP;
    }
}
