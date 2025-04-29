using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QuestionScrollViewDisplay : MonoBehaviour
{
    public Transform contentPanel;
    public GameObject questionEntryPrefab; // Questo ora è un Canvas
    public string panelName = "QuestionPanel"; // Nome del Panel dentro il Canvas
    public string questionTextName = "QuestionText"; // Nome del TMP_Text per la domanda
    public string correctAnswerTextName = "CorrectAnswer"; // Nome del TMP_Text per la risposta corretta
    public string descriptionTextName = "Description"; // Nome del TMP_Text per la descrizione
    public QuestionData questionDataSource;

    private List<QuestionData.LocalizedQuestion> questions;

    void Start()
    {
        if (contentPanel == null || questionEntryPrefab == null || questionDataSource == null)
        {
            Debug.LogError("Uno o più riferimenti UI o la sorgente dati non sono stati assegnati nell'Inspector!");
            return;
        }

        LoadQuestions();
        DisplayQuestions();
    }

    void LoadQuestions()
    {
        questions = new List<QuestionData.LocalizedQuestion>(questionDataSource.localizedQuestions);
    }

    void DisplayQuestions()
    {
        if (questions == null || questions.Count == 0)
        {
            Debug.Log("Nessuna domanda disponibile da visualizzare.");
            return;
        }

        string currentLanguage = (LanguageManager.instance != null) ? LanguageManager.instance.currentLanguage : "ENGLISH";

        foreach (var questionData in questions)
        {
            GameObject entry = Instantiate(questionEntryPrefab, contentPanel);
            if (entry != null)
            {
                // Trova il Panel dentro il Canvas
                Transform panelTransform = entry.transform.Find(panelName);

                if (panelTransform != null)
                {
                    // Cerca i TMP_Text come figli diretti del Panel
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
    }

    public void UpdateLocalizedQuestions()
    {
        LoadQuestions();
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
        DisplayQuestions();
    }

    private void OnEnable()
    {
        if (LanguageManager.instance != null)
        {
            LanguageManager.onLanguageChanged += UpdateLocalizedQuestions;
        }
    }

    private void OnDisable()
    {
        if (LanguageManager.instance != null)
        {
            LanguageManager.onLanguageChanged -= UpdateLocalizedQuestions;
        }
    }
}