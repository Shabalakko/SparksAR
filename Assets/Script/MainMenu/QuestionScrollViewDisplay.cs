using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Necessario per SceneManager

public class QuestionScrollViewDisplay : MonoBehaviour
{
    public Transform contentPanel;
    public GameObject questionEntryPrefab;
    public string panelName = "QuestionPanel";
    public string questionTextName = "QuestionText";
    public string correctAnswerTextName = "CorrectAnswer";
    public string descriptionTextName = "Description";
    public QuestionData questionDataSource;

    private List<QuestionData.LocalizedQuestion> questions;
    private bool isInitialized = false; // Flag per controllare l'inizializzazione

    void Awake() // Usiamo Awake per inizializzare prima di Start
    {
        if (contentPanel == null || questionEntryPrefab == null || questionDataSource == null)
        {
            Debug.LogError("Uno o più riferimenti UI o la sorgente dati non sono stati assegnati nell'Inspector!");
            return;
        }
        LoadQuestions(); // Carica i dati una volta all'avvio
        LanguageManager.onLanguageChanged += HandleLanguageChanged; // Usa HandleLanguageChanged
        isInitialized = true; // Imposta il flag di inizializzazione
    }

    void Start()
    {
        // Controlla se è già stato inizializzato.
        if (isInitialized)
        {
            DisplayQuestions();
        }

    }

    void LoadQuestions()
    {
        questions = new List<QuestionData.LocalizedQuestion>(questionDataSource.localizedQuestions);
    }

    void DisplayQuestions()
    {
        if (!isInitialized) return; // Assicurati che tutto sia inizializzato.
        if (questions == null || questions.Count == 0)
        {
            Debug.Log("Nessuna domanda disponibile da visualizzare.");
            return;
        }

        string currentLanguage = (LanguageManager.instance != null) ? LanguageManager.instance.currentLanguage : "ENGLISH";

        // Disattiva il layout group prima di aggiungere/rimuovere elementi
        if (contentPanel.GetComponent<LayoutGroup>() != null)
            contentPanel.GetComponent<LayoutGroup>().enabled = false;

        // Pulisci prima i contenuti del contentPanel
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var questionData in questions)
        {
            GameObject entry = Instantiate(questionEntryPrefab, contentPanel);
            if (entry != null)
            {
                Transform panelTransform = entry.transform.Find(panelName);

                if (panelTransform != null)
                {
                    TMP_Text questionTextComponent = panelTransform.Find(questionTextName)?.GetComponent<TMP_Text>();
                    TMP_Text correctAnswerTextComponent = panelTransform.Find(correctAnswerTextName)?.GetComponent<TMP_Text>();
                    TMP_Text descriptionTextComponent = panelTransform.Find(descriptionTextName)?.GetComponent<TMP_Text>();

                    if (questionTextComponent != null)
                    {
                        questionTextComponent.text = (currentLanguage == "FRENCH") ? questionData.frenchText : questionData.englishText;
                    }
                    else
                    {
                        Debug.LogWarning($"Non trovato TMP_Text con nome '{questionTextName}' nel Panel '{panelName}'.");
                    }

                    if (correctAnswerTextComponent != null)
                    {
                        correctAnswerTextComponent.text = "" +
                                                        ((currentLanguage == "FRENCH") ? questionData.correctAnswerFrench : questionData.correctAnswerEnglish);
                    }
                    else
                    {
                        Debug.LogWarning($"Non trovato TMP_Text con nome '{correctAnswerTextName}' nel Panel '{panelName}'.");
                    }

                    if (descriptionTextComponent != null)
                    {
                        descriptionTextComponent.text = "" +
                                                        ((currentLanguage == "FRENCH") ? questionData.descriptionFrench : questionData.descriptionEnglish);
                    }
                    else
                    {
                        Debug.LogWarning($"Non trovato TMP_Text con nome '{descriptionTextName}' nel Panel '{panelName}'.");
                    }
                }
                else
                {
                    Debug.LogError($"Non trovato Panel con nome '{panelName}' nel Canvas prefab.");
                }
            }
        }
        // Riattiva il layout group dopo aver aggiunto tutti gli elementi
        if (contentPanel.GetComponent<LayoutGroup>() != null)
            contentPanel.GetComponent<LayoutGroup>().enabled = true;
    }

    // Cambiato da UpdateLocalizedQuestions a HandleLanguageChanged
    public void HandleLanguageChanged()
    {
        DisplayQuestions(); // Richiama direttamente DisplayQuestions
    }

    private void OnEnable()
    {
        if (isInitialized && LanguageManager.instance != null)
        {
            LanguageManager.onLanguageChanged += HandleLanguageChanged;
        }
    }

    private void OnDisable()
    {
        if (LanguageManager.instance != null)
        {
            LanguageManager.onLanguageChanged -= HandleLanguageChanged;
        }
    }

    private void OnDestroy()
    {
        if (LanguageManager.instance != null)
        {
            LanguageManager.onLanguageChanged -= HandleLanguageChanged;
        }
    }
}
