using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Video;

[System.Serializable]
public class TutorialPanel
{
    [Header("Oggetti panel")]
    public GameObject panel;

    [Header("Contenuti del panel (Video Clips)")]
    public List<VideoClip> pageContents;

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

    [Header("Oggetto Video Target")]
    public RawImage videoTargetRawImage; // L'oggetto RawImage designato come target

    private VideoPlayer videoPlayer;

    [Header("Bottoni di navigazione")]
    public Button nextButton;
    public Button prevButton;

    [Header("Bottoni dei capitoli (Skip)")]
    public Button[] chapterButtons;

    void Awake()
    {
        // Assicura che l'evento onLanguageChanged sia sottoscritto prima di qualsiasi Start()
        LanguageManager.onLanguageChanged += UpdateAllPanelTexts;

        if (videoTargetRawImage != null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
            if (videoPlayer == null)
            {
                videoPlayer = gameObject.AddComponent<VideoPlayer>();
                videoPlayer.playOnAwake = false;
                videoPlayer.isLooping = false;
                videoPlayer.renderMode = VideoRenderMode.APIOnly;
                Debug.Log("VideoPlayer component added.");
            }
            else
            {
                Debug.Log("VideoPlayer component found.");
            }

            int width = (int)videoTargetRawImage.rectTransform.rect.width;
            int height = (int)videoTargetRawImage.rectTransform.rect.height;
            Debug.Log($"RawImage Width (Awake): {width}, Height (Awake): {height}");

            if (width > 0 && height > 0)
            {
                videoPlayer.targetTexture = new RenderTexture(width, height, 0);
                Debug.Log($"RenderTexture created (Awake): Width={videoPlayer.targetTexture.width}, Height={videoPlayer.targetTexture.height}");
                videoTargetRawImage.texture = videoPlayer.targetTexture;
                Debug.Log($"RawImage texture assigned (Awake): {videoTargetRawImage.texture}");
            }
            else
            {
                Debug.LogError("RawImage dimensions are zero or negative in Awake(). Cannot create RenderTexture.");
            }
        }
        else
        {
            Debug.LogError("Oggetto RawImage target per il video non assegnato!");
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
        StopCurrentVideo(); // Ferma il video corrente

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
        StopCurrentVideo(); // Ferma il video corrente

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

        StopCurrentVideo(); // Ferma il video corrente
        currentPanelIndex = panelIndex;
        tutorialPanels[currentPanelIndex].currentPage = 0;
        AttivaSoloPanel(currentPanelIndex);
        UpdatePanelUI();
    }

    // Aggiorna il contenuto del pannello attivo e gli indicatori
    void UpdatePanelUI()
    {
        TutorialPanel currentPanel = tutorialPanels[currentPanelIndex];

        // Gestione dei video
        if (videoTargetRawImage != null && currentPanel.pageContents.Count > 0)
        {
            PlayCurrentVideo(currentPanel.pageContents[currentPanel.currentPage]);
        }
        else if (videoTargetRawImage != null)
        {
            StopCurrentVideo();
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
        // Il video viene gestito in UpdatePanelUI in base al pannello attivo e alla pagina.
    }

    private void PlayCurrentVideo(VideoClip clip)
    {
        if (videoPlayer != null)
        {
            Debug.Log($"Attempting to play clip: {clip}");
            if (clip != null)
            {
                videoPlayer.clip = clip;
                videoPlayer.Play();
                Debug.Log($"Video started playing. Is Playing: {videoPlayer.isPlaying}");
            }
            else
            {
                Debug.LogWarning("No VideoClip assigned for this page.");
                StopCurrentVideo();
            }
        }
        else
        {
            Debug.LogError("VideoPlayer is null in PlayCurrentVideo!");
        }
    }

    private void StopCurrentVideo()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            Debug.Log("Stopping current video.");
            videoPlayer.Stop();
        }
        else if (videoPlayer != null)
        {
            Debug.Log("No video playing to stop.");
        }
        else
        {
            Debug.LogError("VideoPlayer is null in StopCurrentVideo!");
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