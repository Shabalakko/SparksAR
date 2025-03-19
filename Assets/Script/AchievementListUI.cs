using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementListUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform achievementListContainer;  // Contenitore per gli elementi degli achievement (es. un pannello con Vertical Layout Group)
    public GameObject achievementEntryPrefab;   // Prefab per rappresentare un singolo achievement (con TextMeshProUGUI per nome, descrizione e stato)
    public Button resetButton;                  // Bottone per resettare tutti gli achievement

    void Start()
    {
        // Popola la lista all'avvio
        PopulateAchievements();

        // Associa l'azione di reset al bottone
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetAchievements);
    }

    // Metodo per popolare la lista degli achievement
    void PopulateAchievements()
    {
        // Se ci fossero già elementi, li rimuoviamo
        foreach (Transform child in achievementListContainer)
        {
            Destroy(child.gameObject);
        }

        // Per ogni achievement registrato, creiamo un elemento UI
        foreach (Achievement achievement in AchievementManager.Instance.achievements)
        {
            GameObject entry = Instantiate(achievementEntryPrefab, achievementListContainer);

            // Supponiamo che il prefab abbia 3 TextMeshProUGUI come figli:
            // - Il primo per il nome
            // - Il secondo per la descrizione
            // - Il terzo per lo stato ("Unlocked" o "Locked")
            TextMeshProUGUI[] texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 3)
            {
                texts[0].text = achievement.name;
                texts[1].text = achievement.description;
                texts[2].text = achievement.isUnlocked ? "Unlocked" : "Locked";
            }
        }
    }

    // Metodo chiamato al click del bottone di reset
    void ResetAchievements()
    {
        // Per ogni achievement, resettiamo lo stato e il relativo valore in PlayerPrefs
        foreach (Achievement achievement in AchievementManager.Instance.achievements)
        {
            achievement.isUnlocked = false;
            PlayerPrefs.SetInt("Achievement_" + achievement.name, 0);
        }
        PlayerPrefs.Save();

        // Aggiorna la lista UI per riflettere i cambiamenti
        PopulateAchievements();
    }
}
