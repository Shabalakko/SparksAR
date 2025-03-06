using UnityEngine;

public class LaserParticleController : MonoBehaviour
{
    [Header("Laser Particles")]
    public ParticleSystem redLaserParticles;
    public ParticleSystem blueLaserParticles;

    [Header("Emission Settings")]
    [Range(0f, 1f)]
    public float activationThreshold = 0.3f; // Percentuale di energia sotto cui il particellare si attiva
    public float minEmissionRate = 5f;
    public float maxEmissionRate = 50f;

    private LaserEnergyManager energyManager;
    private ParticleSystem.EmissionModule redEmission;
    private ParticleSystem.EmissionModule blueEmission;

    void Start()
    {
        energyManager = FindObjectOfType<LaserEnergyManager>();
        if (redLaserParticles != null) redEmission = redLaserParticles.emission;
        if (blueLaserParticles != null) blueEmission = blueLaserParticles.emission;
    }

    void Update()
    {
        UpdateParticleEmission(redLaserParticles, redEmission, energyManager.GetRedEnergy(), energyManager.redMaxEnergy);
        UpdateParticleEmission(blueLaserParticles, blueEmission, energyManager.GetBlueEnergy(), energyManager.blueMaxEnergy);
    }

    void UpdateParticleEmission(ParticleSystem particleSystem, ParticleSystem.EmissionModule emission, float currentEnergy, float maxEnergy)
    {
        if (particleSystem == null) return;

        float energyPercentage = currentEnergy / maxEnergy;

        emission.enabled = true;
        float emissionRate = energyPercentage <= activationThreshold
            ? Mathf.Lerp(minEmissionRate, maxEmissionRate, 1f - (energyPercentage / activationThreshold))
            : minEmissionRate;
        emission.rateOverTime = emissionRate;
    }
}
