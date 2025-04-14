using UnityEngine;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    public string key; // Chiave di localizzazione per questo testo
    private TMP_Text textComponent;

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
        if (textComponent == null)
        {
            Debug.LogError("LocalizedText script attached to an object without a TMP_Text component!", gameObject);
            return;
        }
        UpdateText();
    }

    void OnEnable()
    {
        // Aggiorna il testo quando l'oggetto è abilitato.
        UpdateText();
        // Ascolta l'evento di cambio lingua
        LanguageManager.onLanguageChanged += UpdateText;
    }

    void OnDisable()
    {
        // Disiscriviti dall'evento di cambio lingua
        LanguageManager.onLanguageChanged -= UpdateText;
    }

    public void UpdateText()
    {
        if (textComponent != null && LanguageManager.instance != null)
        {
            textComponent.text = LanguageManager.instance.GetLocalizedText(key, textComponent.text);
        }
    }
}