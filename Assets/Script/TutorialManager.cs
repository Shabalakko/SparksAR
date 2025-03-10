using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class TutorialPanel
{
    [Header("Oggetti panel")]
    public GameObject panel;

    [Header("Contenuti del panel")]
    public List<Sprite> pageContents;
    public Image displayImage;

    [Header("Indicatori di pagina")]
    public Image[] pageIndicators;
    public Sprite activeIndicatorSprite;
    public Sprite inactiveIndicatorSprite;

    [HideInInspector]
    public int currentPage = 0;
}

public class TutorialManager : MonoBehaviour
{
    [Header("Configurazione Panels")]
    public TutorialPanel[] tutorialPanels;

    private int currentPanelIndex = 0;

    [Header("Bottoni di navigazione")]
    public Button nextButton;
    public Button prevButton;

    [Header("Bottoni dei capitoli (Skip)")]
    public Button[] chapterButtons;

    void Start()
    {
        // Attiva solo il primo panel
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

    // Metodo per selezionare direttamente un panel tramite i bottoni laterali
    public void SelectPanel(int panelIndex)
    {
        if (panelIndex < 0 || panelIndex >= tutorialPanels.Length)
        {
            Debug.LogWarning("Indice panel non valido: " + panelIndex);
            return;
        }
        currentPanelIndex = panelIndex;
        tutorialPanels[currentPanelIndex].currentPage = 0;
        AttivaSoloPanel(currentPanelIndex);
        UpdatePanelUI();
    }

    // Aggiorna il contenuto del panel attivo e gli indicatori
    void UpdatePanelUI()
    {
        TutorialPanel currentPanel = tutorialPanels[currentPanelIndex];
        if (currentPanel.displayImage && currentPanel.pageContents.Count > 0)
            currentPanel.displayImage.sprite = currentPanel.pageContents[currentPanel.currentPage];

        for (int i = 0; i < currentPanel.pageIndicators.Length; i++)
        {
            if (i < currentPanel.pageContents.Count)
            {
                currentPanel.pageIndicators[i].sprite = (i == currentPanel.currentPage) ?
                    currentPanel.activeIndicatorSprite : currentPanel.inactiveIndicatorSprite;
            }
        }

        // Aggiorna l'alpha dei bottoni capitolo
        UpdateChapterButtonsAlpha();
    }

    // Attiva solo il panel specificato e disattiva gli altri
    void AttivaSoloPanel(int index)
    {
        for (int i = 0; i < tutorialPanels.Length; i++)
        {
            tutorialPanels[i].panel.SetActive(i == index);
        }
    }

    // Aggiorna l'alpha dei bottoni dei capitoli: alpha 0.3 per il bottone del panel attivo, 1 per gli altri
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
