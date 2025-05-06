using UnityEngine;
using TMPro;

public class CoinDisplay : MonoBehaviour
{
    public TextMeshProUGUI coinsTextUI; // Assegna qui l'elemento Text UI nell'Inspector

    private void OnEnable()
    {
        // Iscriviti all'evento OnCoinsChanged per aggiornare il testo quando le monete cambiano
        WalletManager.OnCoinsChanged += UpdateCoinsText;
        // Aggiorna il testo immediatamente all'abilitazione dell'oggetto
        UpdateCoinsText(WalletManager.Instance.GetTotalCoins());
    }

    private void OnDisable()
    {
        // Annulla la sottoscrizione all'evento per evitare memory leak
        WalletManager.OnCoinsChanged -= UpdateCoinsText;
    }

    private void Start()
    {
        // Assicurati che la riferimento al testo UI sia valido
        if (coinsTextUI == null)
        {
            Debug.LogError("TextMeshProUGUI non assegnato a CoinDisplay!");
            enabled = false; // Disabilita lo script se manca il riferimento
        }
        else
        {
            // Aggiorna il testo iniziale
            UpdateCoinsText(WalletManager.Instance.GetTotalCoins());
        }
    }

    private void UpdateCoinsText(int currentCoins)
    {
        if (coinsTextUI != null)
        {
            coinsTextUI.text = currentCoins.ToString();
        }
    }
}