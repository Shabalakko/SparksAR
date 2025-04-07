using UnityEngine;
using System;

public class WalletManager : MonoBehaviour
{
    private const string COINS_PLAYERPREFS_KEY = "TotalCoins";

    public static event Action<int> OnCoinsChanged;

    public static WalletManager Instance { get; private set; }

    [Header("Testing Options")]
    public int initialCoins = 0; // Monete iniziali per il testing
    public bool resetCoinsOnStart = false; // Resetta le monete all'avvio per il testing

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Inizializza il portafoglio
        if (resetCoinsOnStart)
        {
            ResetTotalCoins(); // Resetta se richiesto
        }
        if (!PlayerPrefs.HasKey(COINS_PLAYERPREFS_KEY))
        {
            PlayerPrefs.SetInt(COINS_PLAYERPREFS_KEY, initialCoins); // Usa initialCoins
            PlayerPrefs.Save();
        }
        else if (resetCoinsOnStart)
        {
            PlayerPrefs.SetInt(COINS_PLAYERPREFS_KEY, initialCoins); // Usa initialCoins
            PlayerPrefs.Save();
        }
    }

    // Metodo per ottenere il totale delle monete
    public int GetTotalCoins()
    {
        return PlayerPrefs.GetInt(COINS_PLAYERPREFS_KEY, 0);
    }

    // Metodo per aggiungere monete al totale
    public void AddCoins(int amount)
    {
        int currentCoins = GetTotalCoins();
        currentCoins += amount;
        PlayerPrefs.SetInt(COINS_PLAYERPREFS_KEY, currentCoins);
        PlayerPrefs.Save();
        OnCoinsChanged?.Invoke(currentCoins);
    }

    // Metodo per sottrarre monete al totale
    public void RemoveCoins(int amount)
    {
        int currentCoins = GetTotalCoins();
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            PlayerPrefs.SetInt(COINS_PLAYERPREFS_KEY, currentCoins);
            PlayerPrefs.Save();
            OnCoinsChanged?.Invoke(currentCoins);
        }
        else
        {
            Debug.LogWarning("Not enough coins!");
        }
    }

    // Metodo per impostare direttamente il totale delle monete (per testing)
    public void SetTotalCoins(int amount) // Nuovo metodo pubblico
    {
        PlayerPrefs.SetInt(COINS_PLAYERPREFS_KEY, amount);
        PlayerPrefs.Save();
        OnCoinsChanged?.Invoke(amount);
    }

    // Metodo per resettare il totale delle monete
    private void ResetTotalCoins()
    {
        PlayerPrefs.SetInt(COINS_PLAYERPREFS_KEY, 0);
        PlayerPrefs.Save();
        OnCoinsChanged?.Invoke(0);
        Debug.Log("Coins reset to 0");
    }
}
