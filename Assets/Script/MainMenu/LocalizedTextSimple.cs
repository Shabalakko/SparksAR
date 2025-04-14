using UnityEngine;
using UnityEngine.UI;
using TMPro; // Se stai usando TextMesh Pro

public class LocalizedTextSimple : MonoBehaviour
{
    public string englishText;
    public string frenchText;
    private Text uiText;
    private TextMeshProUGUI tmpProText;

    void Start()
    {
        uiText = GetComponent<Text>();
        tmpProText = GetComponent<TextMeshProUGUI>();
        UpdateText();
    }

    public void UpdateText()
    {
        if (LanguageManager.instance != null)
        {
            string localizedString = LanguageManager.instance.currentLanguage == "FRENCH" ? frenchText : englishText;
            if (uiText != null)
            {
                uiText.text = localizedString;
            }
            else if (tmpProText != null)
            {
                tmpProText.text = localizedString;
            }
        }
    }

    void OnEnable()
    {
        if (LanguageManager.instance != null)
        {
            LanguageManager.onLanguageChanged += UpdateText;
        }
    }

    void OnDisable()
    {
        if (LanguageManager.instance != null)
        {
            LanguageManager.onLanguageChanged -= UpdateText;
        }
    }
}