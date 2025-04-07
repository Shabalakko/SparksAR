using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    public TextMeshProUGUI partNameText;
    public TextMeshProUGUI costText;
    public Button purchaseButton;
    public Image partImage;
    public ModelPartData partData; // Modificato da private a public

    private ModelShopManager shopManager;

    public void Initialize(ModelPartData data, ModelShopManager manager)
    {
        partData = data;
        shopManager = manager;

        partNameText.text = partData.partName;
        costText.text = partData.cost + " Monete";
        purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
    }

    void OnPurchaseButtonClicked()
    {
        shopManager.PurchasePart(partData);
    }
}
