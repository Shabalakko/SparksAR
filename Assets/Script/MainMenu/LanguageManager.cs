using UnityEngine;
using UnityEngine.UI;
using TMPro; // Se stai usando TextMesh Pro
using System.Collections.Generic; // Necessario per Dictionary

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager instance;
    public string currentLanguage = "ENGLISH"; // Lingua di default

    public delegate void LanguageChanged();
    public static event LanguageChanged onLanguageChanged;

    public LocalizationData localizationData; // Riferimento allo ScriptableObject

    // Usa un Dictionary per memorizzare le traduzioni in modo efficiente
    private Dictionary<string, Dictionary<string, string>> localizedText = new Dictionary<string, Dictionary<string, string>>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSavedLanguage();
            LoadLocalizationData(); // Carica i dati di localizzazione all'avvio
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetLanguage(currentLanguage);
    }

    void LoadLocalizationData()
    {
        localizedText.Clear(); // Pulisci il dizionario prima di caricare nuovi dati
        if (localizationData != null)
        {
            // Assicurati che le lingue siano presenti nel dizionario principale
            if (!localizedText.ContainsKey("ENGLISH"))
                localizedText.Add("ENGLISH", new Dictionary<string, string>());
            if (!localizedText.ContainsKey("FRENCH"))
                localizedText.Add("FRENCH", new Dictionary<string, string>());

            // Itera attraverso le entries dello ScriptableObject e popola il dizionario
            foreach (var entry in localizationData.entries)
            {
                if (!localizedText["ENGLISH"].ContainsKey(entry.key))
                    localizedText["ENGLISH"].Add(entry.key, entry.englishText);
                if (!localizedText["FRENCH"].ContainsKey(entry.key))
                    localizedText["FRENCH"].Add(entry.key, entry.frenchText);
            }
        }
        else
        {
            Debug.LogError("Localization Data is not assigned in LanguageManager!");
        }
    }

    public void SetLanguage(string language)
    {
        currentLanguage = language.ToUpper();
        SaveLanguage();
        if (onLanguageChanged != null)
        {
            onLanguageChanged();
        }
    }

    public string GetLocalizedText(string key, string englishTextFallback)
    {
        if (localizedText.ContainsKey(currentLanguage) && localizedText[currentLanguage].ContainsKey(key))
        {
            return localizedText[currentLanguage][key];
        }
        else
        {
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
