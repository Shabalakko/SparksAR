using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider hitboxSlider; // Riferimento allo slider
    public float minHitboxSize = 0.5f;
    public float maxHitboxSize = 3.0f;

    private SettingsManager settingsManager;

    void Start()
    {
        settingsManager = FindObjectOfType<SettingsManager>();

        // Imposta i limiti dello slider
        hitboxSlider.minValue = minHitboxSize;
        hitboxSlider.maxValue = maxHitboxSize;

        // Imposta il valore iniziale dallo SettingsManager
        hitboxSlider.value = settingsManager.enemyHitboxSize;

        // Salva il valore ogni volta che lo slider viene modificato
        hitboxSlider.onValueChanged.AddListener(UpdateHitboxSize);
    }

    public void UpdateHitboxSize(float newSize)
    {
        settingsManager.SetEnemyHitboxSize(newSize);
    }
}
