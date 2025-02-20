using UnityEngine;
using UnityEngine.UI;

public class LaserEnergyManager : MonoBehaviour
{
    [Header("Impostazioni Energia")]
    public float maxEnergy = 100f;
    public float drainRate = 20f;           // Consumo di energia per secondo
    public float rechargeRate = 10f;        // Ricarica passiva per secondo
    public float bonusRecharge = 30f;       // Ricarica ottenuta sconfiggendo nemici

    [Header("Barre dell'energia")]
    public Image laserBarRed;
    public Image laserBarBlue;

    private float redEnergy;
    private float blueEnergy;
    private bool redOverheated = false;
    private bool blueOverheated = false;

    void Start()
    {
        // Inizia con energia massima per entrambi i laser
        redEnergy = maxEnergy;
        blueEnergy = maxEnergy;
    }

    void Update()
    {
        // Ricarica passiva se non sono surriscaldati
        if (!redOverheated)
        {
            redEnergy += rechargeRate * Time.deltaTime;
            redEnergy = Mathf.Clamp(redEnergy, 0, maxEnergy);
        }

        if (!blueOverheated)
        {
            blueEnergy += rechargeRate * Time.deltaTime;
            blueEnergy = Mathf.Clamp(blueEnergy, 0, maxEnergy);
        }

        // Aggiorna le barre dell'energia
        laserBarRed.fillAmount = redEnergy / maxEnergy;
        laserBarBlue.fillAmount = blueEnergy / maxEnergy;
    }
    // Ritorna l'energia del Laser Rosso
    public float GetRedEnergy()
    {
        return redEnergy;
    }

    // Ritorna l'energia del Laser Blu
    public float GetBlueEnergy()
    {
        return blueEnergy;
    }

    public bool UseRedLaser()
    {
        // Se è surriscaldato, non può sparare
        if (redOverheated) return false;

        // Consuma energia
        redEnergy -= drainRate * Time.deltaTime;
        redEnergy = Mathf.Clamp(redEnergy, 0, maxEnergy);

        // Se l'energia è a zero, il laser si surriscalda
        if (redEnergy <= 0)
        {
            redOverheated = true;
            Invoke(nameof(CoolDownRed), 3f); // Si raffredda dopo 3 secondi
        }

        return true;
    }

    public bool UseBlueLaser()
    {
        if (blueOverheated) return false;

        blueEnergy -= drainRate * Time.deltaTime;
        blueEnergy = Mathf.Clamp(blueEnergy, 0, maxEnergy);

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

    // Ricarica il laser rosso quando viene sconfitto un nemico blu
    public void RechargeRed()
    {
        redEnergy += bonusRecharge;
        redEnergy = Mathf.Clamp(redEnergy, 0, maxEnergy);
    }

    // Ricarica il laser blu quando viene sconfitto un nemico rosso
    public void RechargeBlue()
    {
        blueEnergy += bonusRecharge;
        blueEnergy = Mathf.Clamp(blueEnergy, 0, maxEnergy);
    }

    // Aggiunge una quantità specifica di energia al laser blu
    public void AddBlueEnergy(float amount)
    {
        blueEnergy += amount;
        blueEnergy = Mathf.Clamp(blueEnergy, 0, maxEnergy);
        Debug.Log("Blue energy increased by: " + amount);
    }

    // Aggiunge una quantità specifica di energia al laser rosso
    public void AddRedEnergy(float amount)
    {
        redEnergy += amount;
        redEnergy = Mathf.Clamp(redEnergy, 0, maxEnergy);
        Debug.Log("Red energy increased by: " + amount);
    }
}
