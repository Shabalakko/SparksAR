using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class TutorialPanel
{
    [Header("Oggetti panel")]
    public GameObject panel;

    [Header("Contenuti del panel (Immagini)")]
    public List<Sprite> pageContents;

    [Header("Indicatori di pagina")]
    public Image[] pageIndicators;
    public Sprite activeIndicatorSprite;
    public Sprite inactiveIndicatorSprite;

    [Header("Testi Localizzati")]
    public string[] titleKeys; // Array di chiavi di localizzazione per i titoli
    public string[] descriptionKeys; // Array di chiavi di localizzazione per le descrizioni
    public TMP_Text titleText; // Riferimento al TextMesh Pro per il titolo
    public TMP_Text descriptionText; // Riferimento al TextMesh Pro per la descrizione

    [HideInInspector]
    public int currentPage = 0;
}

public class TutorialManager : MonoBehaviour
{
    [Header("Configurazione Panels")]
    public TutorialPanel[] tutorialPanels;

    private int currentPanelIndex = 0;

    [Header("Oggetto Image Target")]
    public Image imageTargetImage; // L'oggetto Image designato come target

    [Header("Bottoni di navigazione")]
    public Button nextButton;
    public Button prevButton;

    [Header("Bottoni dei capitoli (Skip)")]
    public Button[] chapterButtons;

    void Awake()
    {
        // Assicura che l'evento onLanguageChanged sia sottoscritto prima di qualsiasi Start()
        LanguageManager.onLanguageChanged += UpdateAllPanelTexts;

        if (imageTargetImage == null)
        {
            Debug.LogError("Oggetto Image target non assegnato!");
        }
    }

    void OnDestroy()
    {
        LanguageManager.onLanguageChanged -= UpdateAllPanelTexts;
    }

    void Start()
    {
        // Attiva solo il primo pannello
        for (int i = 0; i < tutorialPanels.Length; i++)
        {
            tutorialPanels[i].panel.SetActive(i == currentPanelIndex);
        }
        UpdatePanelUI();

        if (nextButton) nextButton.onClick.AddListener(OnNextButton);
        if (prevButton) prevButton.onClick.AddListener(OnPrevButton);

        if (chapterButtons != null && chapterButtons.Length == tutorialPanels.Length)
        {
            for (int i = 0; i < chapterButtons.Length; i++)
            {
                int index = i;
                chapterButtons[i].onClick.AddListener(() => SelectPanel(index));
            }
        }
    }

    public void OnNextButton()
    {
        TutorialPanel currentPanel = tutorialPanels[currentPanelIndex];

        if (currentPanel.currentPage < currentPanel.pageContents.Count - 1)
        {
            currentPanel.currentPage++;
            UpdatePanelUI();
        }
        else
        {
            if (currentPanelIndex < tutorialPanels.Length - 1)
            {
                currentPanelIndex++;
                tutorialPanels[currentPanelIndex].currentPage = 0;
                AttivaSoloPanel(currentPanelIndex);
                UpdatePanelUI();
            }
        }
    }

    public void OnPrevButton()
    {
        TutorialPanel currentPanel = tutorialPanels[currentPanelIndex];

        if (currentPanel.currentPage > 0)
        {
            currentPanel.currentPage--;
            UpdatePanelUI();
        }
        else
        {
            if (currentPanelIndex > 0)
            {
                currentPanelIndex--;
                TutorialPanel prevPanel = tutorialPanels[currentPanelIndex];
                prevPanel.currentPage = prevPanel.pageContents.Count - 1;
                AttivaSoloPanel(currentPanelIndex);
                UpdatePanelUI();
            }
        }
    }

    // Metodo per selezionare direttamente un pannello tramite i bottoni laterali
    public void SelectPanel(int panelIndex)
    {
        if (panelIndex < 0 || panelIndex >= tutorialPanels.Length)
        {
            Debug.LogWarning("Indice pannello non valido: " + panelIndex);
            return;
        }

        currentPanelIndex = panelIndex;
        tutorialPanels[currentPanelIndex].currentPage = 0;
        AttivaSoloPanel(currentPanelIndex);
        UpdatePanelUI();
    }

    // Aggiorna il contenuto del pannello attivo e gli indicatori
    void UpdatePanelUI()
    {
        TutorialPanel currentPanel = tutorialPanels[currentPanelIndex];

        // Gestione delle immagini
        if (imageTargetImage != null && currentPanel.pageContents.Count > 0)
        {
            DisplayCurrentImage(currentPanel.pageContents[currentPanel.currentPage]);
        }
        else if (imageTargetImage != null)
        {
            imageTargetImage.sprite = null; // Pulisci l'immagine se non ci sono contenuti
        }

        // Gestione degli indicatori di pagina
        if (currentPanel.pageIndicators != null)
        {
            for (int i = 0; i < currentPanel.pageIndicators.Length; i++)
            {
                if (i < currentPanel.pageContents.Count)
                {
                    currentPanel.pageIndicators[i].sprite = (i == currentPanel.currentPage) ?
                        currentPanel.activeIndicatorSprite : currentPanel.inactiveIndicatorSprite;
                }
                else if (currentPanel.pageIndicators[i] != null)
                {
                    // Nascondi gli indicatori in eccesso se ci sono meno contenuti
                    currentPanel.pageIndicators[i].gameObject.SetActive(false);
                }
            }
            // Assicurati che solo gli indicatori necessari siano attivi
            for (int i = currentPanel.pageContents.Count; i < currentPanel.pageIndicators.Length; i++)
            {
                if (currentPanel.pageIndicators[i] != null)
                {
                    currentPanel.pageIndicators[i].gameObject.SetActive(false);
                }
            }
            // Riattiva gli indicatori necessari
            for (int i = 0; i < currentPanel.pageContents.Count; i++)
            {
                if (currentPanel.pageIndicators[i] != null)
                {
                    currentPanel.pageIndicators[i].gameObject.SetActive(true);
                }
            }
        }

        // Gestione dei testi localizzati
        UpdatePanelText(currentPanel);

        // Aggiorna l'alpha dei bottoni capitolo
        UpdateChapterButtonsAlpha();
    }

    private void UpdatePanelText(TutorialPanel panel)
    {
        if (panel.titleText != null && panel.titleKeys != null && panel.titleKeys.Length > panel.currentPage)
        {
            panel.titleText.text = LanguageManager.instance.GetLocalizedText(panel.titleKeys[panel.currentPage], panel.titleText.text);
        }
        if (panel.descriptionText != null && panel.descriptionKeys != null && panel.descriptionKeys.Length > panel.currentPage)
        {
            panel.descriptionText.text = LanguageManager.instance.GetLocalizedText(panel.descriptionKeys[panel.currentPage], panel.descriptionText.text);
        }
    }

    private void UpdateAllPanelTexts()
    {
        foreach (TutorialPanel panel in tutorialPanels)
        {
            if (panel != null)
            {
                UpdatePanelText(panel);
            }
        }
    }

    // Attiva solo il pannello specificato e disattiva gli altri
    void AttivaSoloPanel(int index)
    {
        for (int i = 0; i < tutorialPanels.Length; i++)
        {
            tutorialPanels[i].panel.SetActive(i == index);
        }
        // L'immagine viene gestita in UpdatePanelUI in base al pannello attivo e alla pagina.
    }

    private void DisplayCurrentImage(Sprite image)
    {
        if (imageTargetImage != null)
        {
            imageTargetImage.sprite = image;
        }
        else
        {
            Debug.LogError("ImageTargetImage is null in DisplayCurrentImage!");
        }
    }

    // Aggiorna l'alpha dei bottoni dei capitoli: alpha 0.3 per il bottone del pannello attivo, 1 per gli altri
    void UpdateChapterButtonsAlpha()
    {
        if (chapterButtons == null)
            return;
        for (int i = 0; i < chapterButtons.Length; i++)
        {
            Image btnImage = chapterButtons[i].GetComponent<Image>();
            if (btnImage != null)
            {
                Color c = btnImage.color;
                c.a = (i == currentPanelIndex) ? 0.3f : 1f;
                btnImage.color = c;
            }
        }
    }
}