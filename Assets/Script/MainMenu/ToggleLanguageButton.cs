using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleLanguageButton : MonoBehaviour
{
    public string englishLanguageCode = "ENGLISH";
    public string frenchLanguageCode = "FRENCH";
    public Texture2D englishTexture;
    public Texture2D frenchTexture;
    public RawImage targetRawImage; // Riferimento all'elemento RawImage del bottone
    private Image targetImage; // Riferimento all'elemento Image del bottone (per Sprite)

    void Start()
    {
        // Se targetRawImage non è assegnato, prova a prendere il RawImage del bottone stesso
        if (targetRawImage == null)
        {
            targetRawImage = GetComponent<RawImage>();
            if (targetRawImage == null)
            {
                // Se non c'è un RawImage, cerca un componente Image (per usare Sprite)
                targetImage = GetComponent<Image>();
                if (targetImage == null)
                {
                    Debug.LogError("Nessun componente RawImage o Image trovato su questo bottone di cambio lingua.");
                }
            }
        }

        UpdateVisuals();
    }

    public void ToggleLanguage()
    {
        if (LanguageManager.instance != null)
        {
            if (LanguageManager.instance.currentLanguage == englishLanguageCode)
            {
                LanguageManager.instance.SetLanguage(frenchLanguageCode);
            }
            else
            {
                LanguageManager.instance.SetLanguage(englishLanguageCode);
            }
            UpdateVisuals();
        }
    }

    void UpdateVisuals()
    {
        if (targetRawImage != null && LanguageManager.instance != null)
        {
            targetRawImage.texture = LanguageManager.instance.currentLanguage == englishLanguageCode ? englishTexture : frenchTexture;
        }
        else if (targetImage != null && LanguageManager.instance != null)
        {
            // Se stai usando un componente Image, puoi cambiare lo Sprite
            Sprite englishSprite = englishTexture != null ? Sprite.Create(englishTexture, new Rect(0, 0, englishTexture.width, englishTexture.height), Vector2.one * 0.5f) : null;
            Sprite frenchSprite = frenchTexture != null ? Sprite.Create(frenchTexture, new Rect(0, 0, frenchTexture.width, frenchTexture.height), Vector2.one * 0.5f) : null;

            targetImage.sprite = LanguageManager.instance.currentLanguage == englishLanguageCode ? englishSprite : frenchSprite;
        }
        else
        {
            Debug.LogWarning("Componente RawImage o Image non assegnato al bottone di cambio lingua.");
        }
    }
}