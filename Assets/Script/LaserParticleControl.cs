using UnityEngine;
using UnityEngine.UI;

public class LaserParticleController : MonoBehaviour
{
    [Header("Laser Particles")]
    public ParticleSystem redLaserParticles;
    public ParticleSystem blueLaserParticles;

    [Header("UI Sprites")]
    public Image redLaserWarningSprite;
    public Image blueLaserWarningSprite;

    [Header("Emission Settings")]
    [Range(0f, 1f)]
    public float activationThreshold = 0.5f; // Percentuale di energia sotto cui il particellare si attiva
    public float minEmissionRate = 5f;
    public float maxEmissionRate = 50f;

    [Header("UI Sprite Settings")]
    public float minAlphaFadeSpeed = 1f;
    public float maxAlphaFadeSpeed = 5f;

    private LaserEnergyManager energyManager;
    private ParticleSystem.EmissionModule redEmission;
    private ParticleSystem.EmissionModule blueEmission;

    private float redSpriteAlpha = 1f; // Inizializzato a 1
    private float blueSpriteAlpha = 1f; // Inizializzato a 1
    private bool redFadeIn = false; // Inizia a fare il fade out se l'energia scende
    private bool blueFadeIn = false; // Inizia a fare il fade out se l'energia scende

    void Start()
    {
        energyManager = FindObjectOfType<LaserEnergyManager>();
        if (redLaserParticles != null) redEmission = redLaserParticles.emission;
        if (blueLaserParticles != null) blueEmission = blueLaserParticles.emission;

        // Ensure sprites are initially opaque
        if (redLaserWarningSprite != null)
        {
            Color color = redLaserWarningSprite.color;
            color.a = 1f;
            redLaserWarningSprite.color = color;
        }
        if (blueLaserWarningSprite != null)
        {
            Color color = blueLaserWarningSprite.color;
            color.a = 1f;
            blueLaserWarningSprite.color = color;
        }
    }

    void Update()
    {
        UpdateParticleEmission(redLaserParticles, redEmission, energyManager.GetRedEnergy(), energyManager.redMaxEnergy);
        UpdateParticleEmission(blueLaserParticles, blueEmission, energyManager.GetBlueEnergy(), energyManager.blueMaxEnergy);

        UpdateSpriteAlpha(redLaserWarningSprite, energyManager.GetRedEnergy(), energyManager.redMaxEnergy, ref redSpriteAlpha, ref redFadeIn);
        UpdateSpriteAlpha(blueLaserWarningSprite, energyManager.GetBlueEnergy(), energyManager.blueMaxEnergy, ref blueSpriteAlpha, ref blueFadeIn);
    }

    void UpdateParticleEmission(ParticleSystem particleSystem, ParticleSystem.EmissionModule emission, float currentEnergy, float maxEnergy)
    {
        if (particleSystem == null) return;

        float energyPercentage = currentEnergy / maxEnergy;

        emission.enabled = energyPercentage <= activationThreshold;
        float emissionRate = energyPercentage <= activationThreshold
            ? Mathf.Lerp(minEmissionRate, maxEmissionRate, 1f - (energyPercentage / activationThreshold))
            : minEmissionRate;
        emission.rateOverTime = emissionRate;
    }

    void UpdateSpriteAlpha(Image sprite, float currentEnergy, float maxEnergy, ref float spriteAlpha, ref bool fadeIn)
    {
        if (sprite == null) return;

        float energyPercentage = currentEnergy / maxEnergy;

        if (energyPercentage <= activationThreshold)
        {
            float normalizedEnergyDeficit = 1f - (energyPercentage / activationThreshold);
            float alphaFadeSpeed = Mathf.Lerp(minAlphaFadeSpeed, maxAlphaFadeSpeed, normalizedEnergyDeficit);

            if (fadeIn)
            {
                spriteAlpha += alphaFadeSpeed * Time.deltaTime;
                if (spriteAlpha >= 1f)
                {
                    spriteAlpha = 1f;
                    fadeIn = false;
                }
            }
            else
            {
                spriteAlpha -= alphaFadeSpeed * Time.deltaTime;
                if (spriteAlpha <= 0f)
                {
                    spriteAlpha = 0f;
                    fadeIn = true;
                }
            }

            Color color = sprite.color;
            color.a = spriteAlpha;
            sprite.color = color;
        }
        else
        {
            // If above the threshold, ensure the sprite is fully visible and reset fade direction
            Color color = sprite.color;
            color.a = 1f;
            sprite.color = color;
            spriteAlpha = 1f;
            fadeIn = false;
        }
    }
}