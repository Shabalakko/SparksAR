using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LocalizedShopItemUI : MonoBehaviour
{
    public TextMeshProUGUI partNameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI descriptionText;
    public Button purchaseButton;
    public TextMeshProUGUI purchaseButtonText; // Text on the button

    [Header("Localization Keys")]
    public string partNameKey;
    public string descriptionKey;
    public string purchaseButtonKey = "PURCHASE_BUTTON"; // Default key for the button

    [HideInInspector] public ModelPartData partData;
    [HideInInspector] public ModelShopManager shopManager;

    public void Initialize(ModelPartData data, ModelShopManager manager)
    {
        partData = data;
        shopManager = manager;

        // Set initial texts (will be updated in UpdateText)
        if (partNameText != null) partNameText.text = partNameKey;
        if (costText != null) costText.text = partData.cost.ToString() + "";
        if (descriptionText != null) descriptionText.text = descriptionKey;
        if (purchaseButtonText != null) purchaseButtonText.text = purchaseButtonKey;

        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
        }

        UpdateText(); // Update text immediately on initialization
    }

    public void UpdateText()
    {
        if (partNameText != null && LanguageManager.instance != null)
        {
            partNameText.text = LanguageManager.instance.GetLocalizedText(partNameKey, partData.partName); // Fallback to partName
        }
        if (descriptionText != null && LanguageManager.instance != null)
        {
            descriptionText.text = LanguageManager.instance.GetLocalizedText(descriptionKey, partData.description); // Fallback to description
        }
        if (purchaseButtonText != null && LanguageManager.instance != null)
        {
            purchaseButtonText.text = LanguageManager.instance.GetLocalizedText(purchaseButtonKey, "Buy"); // Fallback to "Acquista"
        }
        if (costText != null)
        {
           
            costText.text = partData.cost.ToString();
        }
    }

    void OnPurchaseButtonClicked()
    {
        shopManager.PurchasePart(partData);
    }
}