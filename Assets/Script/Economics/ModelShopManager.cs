using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class ModelShopManager : MonoBehaviour
{
    [Header("Wallet Manager")]
    public WalletManager walletManager;

    [Header("UI References")]
    public TextMeshProUGUI totalCoinsText;
    public TextMeshProUGUI shopTitleText;
    public Transform shopItemsContainer;
    public GameObject shopItemPrefab;

    [Header("Model Parts Data")]
    public List<ModelPartData> modelPartsData = new List<ModelPartData>();
    public GameObject modelContainer;

    private List<GameObject> modelParts = new List<GameObject>();
    public static event Action<ModelPartData> OnPartPurchased;

    [Header("Localization Keys")]
    public string shopTitleKey = "SHOP_TITLE";
    // You can define a base key or a pattern here if needed,
    // or directly use the partName as a basis.
    // Example: public string partNameKeyPrefix = "PART_NAME_";
    // Example: public string descriptionKeyPrefix = "PART_DESC_";

    [Header("Testing Options")]
    public bool resetPartsOnStart = false;

    void Start()
    {
        Time.timeScale = 1f;
        if (walletManager == null)
        {
            Debug.LogError("WalletManager non assegnato a ModelShopManager!");
        }

        if (modelContainer == null)
        {
            Debug.LogError("Model Container non assegnato a ModelShopManager!");
        }
        if (resetPartsOnStart)
        {
            //ResetUnlockedParts();
        }
        InitializeModel();
        PopulateShop();
        UpdateCoinsUI();
        UpdateLocalizedUI();
    }

    void OnEnable()
    {
        WalletManager.OnCoinsChanged += UpdateCoinsUI;
        LanguageManager.onLanguageChanged += UpdateLocalizedUI;
    }

    void OnDisable()
    {
        WalletManager.OnCoinsChanged -= UpdateCoinsUI;
        LanguageManager.onLanguageChanged -= UpdateLocalizedUI;
    }

    private void InitializeModel()
    {
        if (modelContainer != null)
        {
            foreach (Transform child in modelContainer.transform)
            {
                modelParts.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("Model Container is not assigned!");
        }
        LoadUnlockedParts();
    }

    void PopulateShop()
    {
        foreach (Transform child in shopItemsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (ModelPartData partData in modelPartsData)
        {
            GameObject shopItem = Instantiate(shopItemPrefab, shopItemsContainer);
            LocalizedShopItemUI shopItemUI = shopItem.GetComponent<LocalizedShopItemUI>();
            if (shopItemUI != null)
            {
                // Dynamically set the localization keys based on the ModelPartData
                shopItemUI.partNameKey = partData.partName.ToUpper().Replace(" ", "_") + "_NAME";
                shopItemUI.descriptionKey = partData.partName.ToUpper().Replace(" ", "_") + "_DESC";
                shopItemUI.Initialize(partData, this);
            }
            else
            {
                Debug.LogError("ShopItemPrefab non ha uno script LocalizedShopItemUI!");
            }
        }
        LoadButtonStates();
    }

    void UpdateCoinsUI(int totalCoins)
    {
        if (totalCoinsText != null)
        {
            totalCoinsText.text = "" + walletManager.GetTotalCoins();
        }
    }
    void UpdateCoinsUI()
    {
        if (totalCoinsText != null)
        {
            totalCoinsText.text = "" + walletManager.GetTotalCoins();
        }
    }

    void UpdateLocalizedUI()
    {
        if (shopTitleText != null && LanguageManager.instance != null)
        {
            shopTitleText.text = LanguageManager.instance.GetLocalizedText(shopTitleKey, "Shop");
        }

        foreach (Transform child in shopItemsContainer)
        {
            LocalizedShopItemUI shopItemUI = child.GetComponent<LocalizedShopItemUI>();
            if (shopItemUI != null)
            {
                shopItemUI.UpdateText();
            }
        }
    }

    public void PurchasePart(ModelPartData partData)
    {
        if (walletManager.GetTotalCoins() >= partData.cost)
        {
            walletManager.RemoveCoins(partData.cost);
            UnlockPart(partData);
            SaveUnlockedPart(partData);
            OnPartPurchased?.Invoke(partData);
            UpdateCoinsUI();
            DisablePurchaseButton(partData);
        }
        else
        {
            Debug.Log("Non abbastanza monete per acquistare " + partData.partName);
        }
    }

    void UnlockPart(ModelPartData partData)
    {
        for (int i = 0; i < modelParts.Count; i++)
        {
            if (modelParts[i].name == partData.partName)
            {
                modelParts[i].SetActive(true);
                break;
            }
        }
    }
    void SaveUnlockedPart(ModelPartData partData)
    {
        PlayerPrefs.SetInt(partData.partName, 1);
        PlayerPrefs.SetInt(partData.partName + "_purchased", 1);
        PlayerPrefs.Save();
    }

    void LoadUnlockedParts()
    {
        foreach (ModelPartData partData in modelPartsData)
        {
            if (PlayerPrefs.GetInt(partData.partName, 0) == 1)
            {
                UnlockPart(partData);
            }
        }
    }

    private void DisablePurchaseButton(ModelPartData partData)
    {
        foreach (Transform child in shopItemsContainer)
        {
            LocalizedShopItemUI shopItemUI = child.GetComponent<LocalizedShopItemUI>();
            if (shopItemUI != null && shopItemUI.partData == partData)
            {
                shopItemUI.purchaseButton.interactable = false;
                PlayerPrefs.SetInt(partData.partName + "_button", 0);
                PlayerPrefs.Save();
                break;
            }
        }
    }

    private void LoadButtonStates()
    {
        foreach (Transform child in shopItemsContainer)
        {
            LocalizedShopItemUI shopItemUI = child.GetComponent<LocalizedShopItemUI>();
            if (shopItemUI != null)
            {
                string buttonKey = shopItemUI.partData.partName + "_button";
                if (PlayerPrefs.HasKey(buttonKey))
                {
                    shopItemUI.purchaseButton.interactable = PlayerPrefs.GetInt(buttonKey) == 1;
                }
                else
                {
                    PlayerPrefs.SetInt(buttonKey, 1);
                    PlayerPrefs.Save();
                    shopItemUI.purchaseButton.interactable = true;
                }
            }
        }
    }
}