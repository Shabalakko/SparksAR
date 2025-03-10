using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LaserEnergyManager : MonoBehaviour
{
    [Header("Impostazioni Energia")]
    // Valori massimi separati per ciascun laser
    public float redMaxEnergy = 100f;
    public float blueMaxEnergy = 100f;
    public float drainRate = 20f;           // Consumo di energia per secondo
    public float rechargeRate = 10f;        // Ricarica passiva per secondo
    public float bonusRecharge = 30f;       // Ricarica ottenuta sconfiggendo nemici

    [Header("Durata Power Up")]
    public float powerUpDuration = 10f;     // Durata dell'effetto power up in secondi

    [Header("Barre dell'energia")]
    public Image laserBarRed;
    public Image laserBarBlue;

    private PowerUpUIManager powerUpUIManager;
    private float redEnergy;
    private float blueEnergy;
    private bool redOverheated = false;
    private bool blueOverheated = false;

    void Start()
    {
        redEnergy = redMaxEnergy;
        blueEnergy = blueMaxEnergy;
        powerUpUIManager = FindObjectOfType<PowerUpUIManager>();
    }

    void Update()
    {
        if (!redOverheated)
        {
            redEnergy += rechargeRate * Time.deltaTime;
            redEnergy = Mathf.Clamp(redEnergy, 0, redMaxEnergy);
        }

        if (!blueOverheated)
        {
            blueEnergy += rechargeRate * Time.deltaTime;
            blueEnergy = Mathf.Clamp(blueEnergy, 0, blueMaxEnergy);
        }

        laserBarRed.fillAmount = redEnergy / redMaxEnergy;
        laserBarBlue.fillAmount = blueEnergy / blueMaxEnergy;
    }

    public float GetRedEnergy()
    {
        return redEnergy;
    }

    public float GetBlueEnergy()
    {
        return blueEnergy;
    }

    public bool UseRedLaser()
    {
        if (redOverheated) return false;

        redEnergy -= drainRate * Time.deltaTime;
        redEnergy = Mathf.Clamp(redEnergy, 0, redMaxEnergy);

        if (redEnergy <= 0)
        {
            redOverheated = true;
            Invoke(nameof(CoolDownRed), 3f);
        }

        return true;
    }

    public bool UseBlueLaser()
    {
        if (blueOverheated) return false;

        blueEnergy -= drainRate * Time.deltaTime;
        blueEnergy = Mathf.Clamp(blueEnergy, 0, blueMaxEnergy);

        if (blueEnergy <= 0)
        {
            blueOverheated = true;
            Invoke(nameof(CoolDownBlue), 3f);
        }

        return true;
    }

    void CoolDownRed()
    {
        redOverheated = false;
    }

    void CoolDownBlue()
    {
        blueOverheated = false;
    }

    public void RechargeRed()
    {
        redEnergy += bonusRecharge;
        redEnergy = Mathf.Clamp(redEnergy, 0, redMaxEnergy);
    }

    public void RechargeBlue()
    {
        blueEnergy += bonusRecharge;
        blueEnergy = Mathf.Clamp(blueEnergy, 0, blueMaxEnergy);
    }

    public void AddRedEnergy(float amount)
    {
        redEnergy += amount;
        redEnergy = Mathf.Clamp(redEnergy, 0, redMaxEnergy);
        Debug.Log("Red energy increased by: " + amount);
    }

    public void AddBlueEnergy(float amount)
    {
        blueEnergy += amount;
        blueEnergy = Mathf.Clamp(blueEnergy, 0, blueMaxEnergy);
        Debug.Log("Blue energy increased by: " + amount);
    }

    // Applica l'effetto power up e avvia il timer per il ripristino
    public void ApplyPowerUp(string powerUp)
    {
        Debug.Log("Applied Power-Up: " + powerUp);
        float duration = 10f; // Durata standard dei power-up

        switch (powerUp)
        {
            case "300 LR Energy Gauge":
                redMaxEnergy += 300f;
                redEnergy += 300f;
                StartCoroutine(RevertRedMaxEnergyAfterDelay(300f, duration));
                break;
            case "300 LB Energy Gauge":
                blueMaxEnergy += 300f;
                blueEnergy += 300f;
                StartCoroutine(RevertBlueMaxEnergyAfterDelay(300f, duration));
                break;
            case "10 /s LR Energy Consumption":
                drainRate = 10f;
                StartCoroutine(RevertDrainRateAfterDelay(10f, duration));
                break;
            case "20 /s LR Damage Output":
                LaserGun laserGun = FindObjectOfType<LaserGun>();
                if (laserGun != null)
                {
                    laserGun.laserDamage += 20f;
                    StartCoroutine(RevertLaserDamageRedAfterDelay(20f, duration));
                }
                break;
            case "40 Recharge LB Energy Gauge per D destroyed / C collected":
                bonusRecharge += 40f;
                StartCoroutine(RevertBonusRechargeAfterDelay(40f, duration));
                break;
            case "10 /s LB Energy Consumption":
                drainRate = 10f;
                StartCoroutine(RevertDrainRateAfterDelay(10f, duration));
                break;
            case "20 /s LB Damage Output":
                LaserGunBlue laserGunBlue = FindObjectOfType<LaserGunBlue>();
                if (laserGunBlue != null)
                {
                    laserGunBlue.laserDamage += 20f;
                    StartCoroutine(RevertLaserDamageBlueAfterDelay(20f, duration));
                }
                break;
            case "40 Recharge LR Energy Gauge per D destroyed / C collected":
                bonusRecharge += 40f;
                StartCoroutine(RevertBonusRechargeAfterDelay(40f, duration));
                break;
        }

        // Attiva l'icona corrispondente nel UI Manager
        if (powerUpUIManager != null)
        {
            powerUpUIManager.ActivatePowerUp(powerUp, duration);

        }
    }
    private IEnumerator RevertRedMaxEnergyAfterDelay(float amount, float delay)
    {
        yield return new WaitForSeconds(delay);
        redMaxEnergy -= amount;
        redEnergy = Mathf.Clamp(redEnergy, 0, redMaxEnergy);
        Debug.Log("Reverted Red Max Energy modification");
    }

    private IEnumerator RevertBlueMaxEnergyAfterDelay(float amount, float delay)
    {
        yield return new WaitForSeconds(delay);
        blueMaxEnergy -= amount;
        blueEnergy = Mathf.Clamp(blueEnergy, 0, blueMaxEnergy);
        Debug.Log("Reverted Blue Max Energy modification");
    }

    private IEnumerator RevertDrainRateAfterDelay(float amount, float delay)
    {
        yield return new WaitForSeconds(delay);
        drainRate += amount;
        Debug.Log("Reverted drain rate modification");
    }

    private IEnumerator RevertBonusRechargeAfterDelay(float amount, float delay)
    {
        yield return new WaitForSeconds(delay);
        bonusRecharge -= amount;
        Debug.Log("Reverted bonus recharge modification");
    }

    private IEnumerator RevertLaserDamageRedAfterDelay(float amount, float delay)
    {
        yield return new WaitForSeconds(delay);
        LaserGun laserGun = FindObjectOfType<LaserGun>();
        if (laserGun != null)
        {
            laserGun.laserDamage -= amount;
            Debug.Log("Reverted LR Damage Output modification");
        }
    }

    private IEnumerator RevertLaserDamageBlueAfterDelay(float amount, float delay)
    {
        yield return new WaitForSeconds(delay);
        LaserGunBlue laserGunBlue = FindObjectOfType<LaserGunBlue>();
        if (laserGunBlue != null)
        {
            laserGunBlue.laserDamage -= amount;
            Debug.Log("Reverted LB Damage Output modification");
        }
    }
}
