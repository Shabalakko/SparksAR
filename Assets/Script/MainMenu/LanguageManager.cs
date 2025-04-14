using UnityEngine;
using UnityEngine.UI;
using TMPro; // Se stai usando TextMesh Pro

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager instance;
    public string currentLanguage = "ENGLISH"; // Lingua di default

    public delegate void LanguageChanged();
    public static event LanguageChanged onLanguageChanged;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            // Inizializza la lingua salvata se presente, altrimenti usa quella di default
            LoadSavedLanguage();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLanguage(string language)
    {
        currentLanguage = language.ToUpper();
        SaveLanguage(); // Salva la lingua corrente
        // Notifica tutti gli interessati del cambio di lingua
        if (onLanguageChanged != null)
        {
            onLanguageChanged();
        }
        // Aggiorna anche gli oggetti LocalizedTextSimple
        LocalizedTextSimple[] allLocalizedTexts = FindObjectsOfType<LocalizedTextSimple>();
        foreach (LocalizedTextSimple localizedText in allLocalizedTexts)
        {
            localizedText.UpdateText();
        }
    }

    public string GetLocalizedText(string key, string englishTextFallback)
    {
        switch (key)
        {
            case "final_score":
                return currentLanguage == "FRENCH" ? "Score Final" : englishTextFallback;
            case "high_score_new":
                return currentLanguage == "FRENCH" ? "Nouveau!!!" : englishTextFallback;
            case "high_score":
                return currentLanguage == "FRENCH" ? "Meilleur Score" : englishTextFallback;
            case "earned_coins":
                return currentLanguage == "FRENCH" ? "Pièces Gagnées" : englishTextFallback;
            case "coins":
                return currentLanguage == "FRENCH" ? "Pièces" : englishTextFallback;
            case "current_coins":
                return currentLanguage == "FRENCH" ? "Pièces Actuelles" : englishTextFallback;
            case "time_up": // Aggiungi questo caso
                return currentLanguage == "FRENCH" ? "Temps écoulé !" : englishTextFallback; // Traduzione francese
                                                                                             // Aggiungi altri casi per le tue chiavi
            default:
                Debug.LogWarning("Chiave di localizzazione non trovata: " + key);
                return englishTextFallback;
        }
    }

    // Funzioni per salvare e caricare la lingua (opzionale, ma consigliato)
    private void SaveLanguage()
    {
        PlayerPrefs.SetString("CurrentLanguage", currentLanguage);
        PlayerPrefs.Save();
    }

    private void LoadSavedLanguage()
    {
        if (PlayerPrefs.HasKey("CurrentLanguage"))
        {
            currentLanguage = PlayerPrefs.GetString("CurrentLanguage");
        }
        else
        {
            currentLanguage = "ENGLISH"; // Imposta la lingua di default se non c'è nulla salvato
        }
    }
}
