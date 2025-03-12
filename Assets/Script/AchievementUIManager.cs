using UnityEngine;
using TMPro;
using System.Collections;

public class AchievementUIManager : MonoBehaviour
{
    public static AchievementUIManager Instance;  // Singleton

    public GameObject achievementPopupPrefab; // Prefab della notifica
    public Transform achievementPanel;        // Pannello in cui vengono inseriti i popup

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowAchievement(string achievementName, string description)
    {
        // Istanzia il prefab nel pannello (usa il percorso corretto per i componenti se necessario)
        GameObject popup = Instantiate(achievementPopupPrefab, achievementPanel);
        // Se il prefab ha la seguente gerarchia:
        // AchievementPopup (Canvas)
        // └── Panel
        //       ├── AchievementTitle
        //       └── AchievementDescription

        TextMeshProUGUI titleText = popup.transform.Find("Panel/AchievementTitle")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descriptionText = popup.transform.Find("Panel/AchievementDescription")?.GetComponent<TextMeshProUGUI>();

        if (titleText != null)
            titleText.text = $"{achievementName}";
        else
            Debug.LogWarning("AchievementTitle non trovato nel prefab!");

        if (descriptionText != null)
            descriptionText.text = description;
        else
            Debug.LogWarning("AchievementDescription non trovato nel prefab!");

        StartCoroutine(FadeOutAndDestroy(popup));
    }

    private IEnumerator FadeOutAndDestroy(GameObject popup)
    {
        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = popup.AddComponent<CanvasGroup>();

        yield return new WaitForSecondsRealtime(2f);

        float fadeDuration = 1f;
        float startAlpha = canvasGroup.alpha;
        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t / fadeDuration);
            yield return null;
        }
        Destroy(popup);
    }
}
