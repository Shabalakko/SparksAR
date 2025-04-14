using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleLanguageButton : MonoBehaviour
{
    public string englishLanguageCode = "ENGLISH";
    public string frenchLanguageCode = "FRENCH";
    public Color englishColor = Color.blue;
    public Color frenchColor = Color.red;
    public Graphic targetGraphic; // Riferimento all'elemento grafico del bottone (Image o TextMeshProUGUI)
    private Text buttonText;
    private TextMeshProUGUI tmpButtonText;

    void Start()
    {
        buttonText = GetComponentInChildren<Text>();
        tmpButtonText = GetComponentInChildren<TextMeshProUGUI>();

        // Se targetGraphic non è assegnato, prova a prendere l'Image o TextMeshProUGUI del bottone stesso
        if (targetGraphic == null)
        {
            targetGraphic = GetComponent<Image>();
            if (targetGraphic == null)
            {
                targetGraphic = GetComponent<TextMeshProUGUI>();
            }
        }

        UpdateVisuals();
    }

    public void ToggleLanguage()
    {
        if (LanguageManager.instance != null)
        {
            if (LanguageManager.instance.currentLanguage == englishLanguageCode)
            {
                LanguageManager.instance.SetLanguage(frenchLanguageCode);
            }
            else
            {
                LanguageManager.instance.SetLanguage(englishLanguageCode);
            }
            UpdateVisuals();
        }
    }

    void UpdateVisuals()
    {
        if (buttonText != null && LanguageManager.instance != null)
        {
            buttonText.text = LanguageManager.instance.currentLanguage == englishLanguageCode ? "EN" : "FR";
        }
        else if (tmpButtonText != null && LanguageManager.instance != null)
        {
            tmpButtonText.text = LanguageManager.instance.currentLanguage == englishLanguageCode ? "EN" : "FR";
        }

        if (targetGraphic != null && LanguageManager.instance != null)
        {
            targetGraphic.color = LanguageManager.instance.currentLanguage == englishLanguageCode ? englishColor : frenchColor;
        }
        else
        {
            Debug.LogWarning("Target Graphic non assegnato al bottone di cambio lingua.");
        }
    }
}