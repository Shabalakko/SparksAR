using UnityEngine;
using Unity.Mathematics;
using CesiumForUnity;

public class CoordinateNoiseOscillator : MonoBehaviour
{
    private CesiumGlobeAnchor globeAnchor;

    [Header("Longitudine")]
    [SerializeField] private double minLongitude = -10.0;
    [SerializeField] private double maxLongitude = 10.0;
    [SerializeField] private float longitudeFrequency = 1.0f;
    [SerializeField] private float longitudeNoiseIntensity = 1.0f;

    [Header("Latitudine")]
    [SerializeField] private double minLatitude = -5.0;
    [SerializeField] private double maxLatitude = 5.0;
    [SerializeField] private float latitudeFrequency = 1.5f;
    [SerializeField] private float latitudeNoiseIntensity = 1.0f;

    [Header("Altezza")]
    [SerializeField] private double minHeight = 0.0;
    [SerializeField] private double maxHeight = 100.0;
    [SerializeField] private float heightFrequency = 0.5f;

    private double initialLongitude;
    private double initialLatitude;
    private double initialHeight;

    void Start()
    {
        globeAnchor = GetComponent<CesiumGlobeAnchor>();
        if (globeAnchor == null)
        {
            Debug.LogError("CesiumGlobeAnchor non trovato su " + gameObject.name);
            return;
        }

        // Memorizza le coordinate iniziali
        initialLongitude = globeAnchor.longitudeLatitudeHeight.x;
        initialLatitude = globeAnchor.longitudeLatitudeHeight.y;
        initialHeight = globeAnchor.longitudeLatitudeHeight.z;
    }

    void Update()
    {
        if (globeAnchor != null)
        {
            // Calcolo del tempo normalizzato
            float time = Time.time;

            // Generazione del rumore per longitudine e latitudine
            float longitudeNoise = (Mathf.PerlinNoise(time * longitudeFrequency, 0.0f) - 0.5f) * 2.0f * longitudeNoiseIntensity;
            float latitudeNoise = (Mathf.PerlinNoise(time * latitudeFrequency, 1.0f) - 0.5f) * 2.0f * latitudeNoiseIntensity;

            // Calcolo dei nuovi valori con oscillazione e rumore
            double newLongitude = Mathf.Lerp((float)minLongitude, (float)maxLongitude, (Mathf.Sin(time * longitudeFrequency) + 1.0f) / 2.0f) + longitudeNoise;
            double newLatitude = Mathf.Lerp((float)minLatitude, (float)maxLatitude, (Mathf.Sin(time * latitudeFrequency) + 1.0f) / 2.0f) + latitudeNoise;
            double newHeight = Mathf.Lerp((float)minHeight, (float)maxHeight, (Mathf.Sin(time * heightFrequency) + 1.0f) / 2.0f);

            // Applicazione dei nuovi valori
            globeAnchor.longitudeLatitudeHeight = new double3(newLongitude, newLatitude, newHeight);
        }
    }
}
