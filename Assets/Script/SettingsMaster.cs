using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private static SettingsManager instance;
    public float enemyHitboxSize = 1f; // Valore di default


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persiste tra le scene
        }
        else
        {
            Destroy(gameObject);
        }

        LoadSettings();
    }

    public void SetEnemyHitboxSize(float newSize)
    {
        enemyHitboxSize = newSize;
        PlayerPrefs.SetFloat("EnemyHitboxSize", enemyHitboxSize);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("EnemyHitboxSize"))
        {
            enemyHitboxSize = PlayerPrefs.GetFloat("EnemyHitboxSize");
        }
    }
}
