using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyIndicatorManager : MonoBehaviour
{
    [Header("Impostazioni Indicatori")]
    public RectTransform indicatorPrefab; // Prefab dell'indicatore (UI Element con RectTransform)
    public Canvas canvas;                 // Canvas dove istanziare gli indicatori
    public float margin = 50f;            // Margine dal bordo dello schermo

    [Header("Impostazioni Scala")]
    [Tooltip("Scala massima dell'indicatore (quando il nemico è vicino)")]
    public float maxScale = 1.5f;
    [Tooltip("Scala minima dell'indicatore (quando il nemico è lontano)")]
    public float minScale = 0.5f;
    [Tooltip("Distanza alla quale l'indicatore raggiunge la scala minima")]
    public float distanceScaleThreshold = 30f;

    private Camera mainCamera;
    private Vector2 screenCenter;
    // Dizionario per associare ogni nemico al suo indicatore
    private Dictionary<GameObject, RectTransform> enemyIndicators = new Dictionary<GameObject, RectTransform>();

    void Start()
    {
        if (canvas == null)
        {
            Debug.LogError("Canvas non assegnata nello script EnemyIndicatorManager.");
        }
        if (indicatorPrefab == null)
        {
            Debug.LogError("Prefab dell'indicatore non assegnato nello script EnemyIndicatorManager.");
        }

        mainCamera = Camera.main;
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    void Update()
    {
        // Trova tutti i nemici con tag "EnemyChase"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("EnemyChase");
        HashSet<GameObject> currentEnemies = new HashSet<GameObject>(enemies);

        // Crea un indicatore per ogni nuovo nemico
        foreach (GameObject enemy in enemies)
        {
            if (!enemyIndicators.ContainsKey(enemy))
            {
                RectTransform newIndicator = Instantiate(indicatorPrefab, canvas.transform);
                newIndicator.gameObject.SetActive(true); // l'indicatore rimane sempre attivo
                enemyIndicators.Add(enemy, newIndicator);
            }
        }

        // Aggiorna ciascun indicatore
        foreach (var kvp in enemyIndicators)
        {
            GameObject enemy = kvp.Key;
            RectTransform indicator = kvp.Value;

            if (enemy == null)
                continue;

            // Ottieni la posizione in schermata del nemico
            Vector3 enemyScreenPos3D = mainCamera.WorldToScreenPoint(enemy.transform.position);
            bool isBehind = enemyScreenPos3D.z < 0;
            Vector2 enemyScreenPos;

            if (!isBehind)
            {
                enemyScreenPos = new Vector2(enemyScreenPos3D.x, enemyScreenPos3D.y);
            }
            else
            {
                // Se il nemico è dietro, calcola una direzione basata sulla posizione relativa in world space
                Vector3 dirWorld = (enemy.transform.position - mainCamera.transform.position).normalized;
                float dx = Vector3.Dot(dirWorld, mainCamera.transform.right);
                float dy = Vector3.Dot(dirWorld, mainCamera.transform.up);
                // Genera un punto virtuale lontano per definire la direzione
                enemyScreenPos = screenCenter + new Vector2(dx, dy) * 1000f;
            }

            // Se il nemico è on screen (rispettando il margine) posiziona l'indicatore esattamente alla sua posizione,
            // altrimenti calcola l'intersezione con il bordo dello schermo
            Vector2 indicatorPos;
            if (enemyScreenPos.x >= margin && enemyScreenPos.x <= Screen.width - margin &&
                enemyScreenPos.y >= margin && enemyScreenPos.y <= Screen.height - margin)
            {
                indicatorPos = enemyScreenPos;
            }
            else
            {
                indicatorPos = GetIntersectionWithScreenBounds(screenCenter, enemyScreenPos, margin);
            }

            indicator.position = indicatorPos;

            // Ruota l'indicatore in modo che punti dal centro verso il nemico
            Vector2 dirIndicator = (indicatorPos - screenCenter).normalized;
            float angle = Mathf.Atan2(dirIndicator.y, dirIndicator.x) * Mathf.Rad2Deg;
            indicator.rotation = Quaternion.Euler(0, 0, angle);

            // Calcola la distanza dal nemico alla camera e regola la scala in base a questa distanza
            float distance = Vector3.Distance(enemy.transform.position, mainCamera.transform.position);
            // Quando la distanza è 0 -> maxScale, quando è >= distanceScaleThreshold -> minScale
            float t = Mathf.Clamp01(distance / distanceScaleThreshold);
            float scale = Mathf.Lerp(maxScale, minScale, t);
            indicator.localScale = new Vector3(scale, scale, scale);
        }

        // Rimuove dalla mappa i nemici che non sono più presenti
        List<GameObject> enemiesToRemove = new List<GameObject>();
        foreach (var kvp in enemyIndicators)
        {
            if (!currentEnemies.Contains(kvp.Key))
            {
                Destroy(kvp.Value.gameObject);
                enemiesToRemove.Add(kvp.Key);
            }
        }
        foreach (GameObject enemy in enemiesToRemove)
        {
            enemyIndicators.Remove(enemy);
        }
    }

    /// <summary>
    /// Calcola l'intersezione tra il raggio che parte da start verso target 
    /// e il rettangolo definito dai bordi dello schermo con un margine.
    /// </summary>
    /// <param name="start">Di solito il centro dello schermo</param>
    /// <param name="target">Il punto "grezzo" (potenzialmente fuori schermo)</param>
    /// <param name="margin">Margine da lasciare dai bordi</param>
    /// <returns>Punto di intersezione sul bordo</returns>
    private Vector2 GetIntersectionWithScreenBounds(Vector2 start, Vector2 target, float margin)
    {
        Vector2 d = target - start;
        float left = margin;
        float right = Screen.width - margin;
        float bottom = margin;
        float top = Screen.height - margin;
        float t = float.MaxValue;

        if (Mathf.Abs(d.x) > 0.001f)
        {
            float tCandidate = d.x > 0 ? (right - start.x) / d.x : (left - start.x) / d.x;
            Vector2 candidatePoint = start + d * tCandidate;
            if (tCandidate > 0 && candidatePoint.y >= bottom && candidatePoint.y <= top)
                t = Mathf.Min(t, tCandidate);
        }

        if (Mathf.Abs(d.y) > 0.001f)
        {
            float tCandidate = d.y > 0 ? (top - start.y) / d.y : (bottom - start.y) / d.y;
            Vector2 candidatePoint = start + d * tCandidate;
            if (tCandidate > 0 && candidatePoint.x >= left && candidatePoint.x <= right)
                t = Mathf.Min(t, tCandidate);
        }

        if (t == float.MaxValue)
            t = 1f;
        return start + d * t;
    }
}
