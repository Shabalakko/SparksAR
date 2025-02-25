using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PowerUpUIManager : MonoBehaviour
{
    [System.Serializable]
    public class PowerUpIcon
    {
        public string powerUpName;   // Nome del power-up
        public Image powerUpImage;   // Icona associata
    }

    [Header("Lista delle icone dei Power-Up")]
    public List<PowerUpIcon> powerUpIcons = new List<PowerUpIcon>();

    private Dictionary<string, Coroutine> activePowerUps = new Dictionary<string, Coroutine>();

    void Start()
    {
        foreach (var icon in powerUpIcons)
        {
            if (icon.powerUpImage != null)
            {
                icon.powerUpImage.fillAmount = 0f;
                icon.powerUpImage.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Attiva l'icona del power-up e avvia il timer per il fill radiale.
    /// </summary>
    public void ActivatePowerUp(string powerUpName, float duration)
    {
        
        PowerUpIcon icon = powerUpIcons.Find(p => p.powerUpName == powerUpName);
        
        if (icon != null && icon.powerUpImage != null)
        {
            icon.powerUpImage.gameObject.SetActive(true);
            icon.powerUpImage.fillAmount = 1f;

            if (activePowerUps.ContainsKey(powerUpName))
            {
                StopCoroutine(activePowerUps[powerUpName]);
                activePowerUps.Remove(powerUpName);
            }

            Coroutine timerCoroutine = StartCoroutine(FillRadialTimer(icon.powerUpImage, duration, powerUpName));
            activePowerUps.Add(powerUpName, timerCoroutine);
        }
    }

    /// <summary>
    /// Coroutine per il countdown del power-up, aggiorna il fill radiale e nasconde l'icona alla fine.
    /// </summary>
    private IEnumerator FillRadialTimer(Image icon, float duration, string powerUpName)
    {
        float timeRemaining = duration;
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            icon.fillAmount = timeRemaining / duration;
            yield return null;
        }

        icon.gameObject.SetActive(false);
        activePowerUps.Remove(powerUpName);
    }
}
