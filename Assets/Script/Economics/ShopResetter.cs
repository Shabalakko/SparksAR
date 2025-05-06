using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ShopResetter : MonoBehaviour
{
    [Header("Model Parts Data")]
    public List<ModelPartData> modelPartsData = new List<ModelPartData>();

    // Metodo per resettare le parti sbloccate e il wallet in PlayerPrefs
    public void ResetAllData()
    {
        // Resetta le parti sbloccate
        foreach (ModelPartData partData in modelPartsData)
        {
            PlayerPrefs.DeleteKey(partData.partName);
            PlayerPrefs.DeleteKey(partData.partName + "_purchased"); // Resetta anche lo stato di acquisto
            PlayerPrefs.DeleteKey(partData.partName + "_button");
        }

        // Resetta il wallet utilizzando il metodo statico di WalletManager
        PlayerPrefs.DeleteKey("TotalCoins"); // Cancella la chiave delle monete
        PlayerPrefs.Save();

        Debug.Log("Unlocked parts and wallet reset!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Ricarica la scena
    }

    // Metodo per resettare solo le parti sbloccate in PlayerPrefs
    public void ResetUnlockedParts()
    {
        foreach (ModelPartData partData in modelPartsData)
        {
            PlayerPrefs.DeleteKey(partData.partName);
            PlayerPrefs.DeleteKey(partData.partName + "_purchased"); // Resetta anche lo stato di acquisto
            PlayerPrefs.DeleteKey(partData.partName + "_button");
        }
        PlayerPrefs.Save();
        Debug.Log("Unlocked parts reset!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //Ricarica la scena
        PlayerPrefs.DeleteKey("TotalCoins"); // Cancella la chiave delle monete
        PlayerPrefs.Save();
        Debug.Log("Wallet reset!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Ricarica la scena
    }

    // Metodo per resettare solo il wallet
    
}