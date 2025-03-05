using System.Collections;
using UnityEngine;

public class EnemyLinearMovement : MonoBehaviour
{
    // Centro della sfera di spawn (da impostare in Inspector o tramite altro script)
    public Vector3 sphereCenter;
    // Raggio della sfera di spawn
    public float sphereRadius = 1.5f;

    // Velocità di movimento del nemico
    public float speed = 5f;
    // Distanza massima dall'origine oltre la quale il nemico viene distrutto
    public float destroyDistance = 20f;

    // Durata dell'animazione di spawn e distruzione
    public float spawnAnimTime = 1f;
    public float destroyAnimTime = 1f;

    // Punto di destinazione calcolato casualmente sulla superficie della sfera
    private Vector3 targetPosition;
    // Direzione di movimento calcolata in Start()
    private Vector3 moveDirection;

    // Riferimento al collider "ZoneDestroy"
    private SphereCollider zoneDestroyCollider;

    // Riferimento al figlio con il MeshRenderer (per animare la scala)
    private Transform meshTransform;

    // Flag per evitare di avviare più volte l'animazione di distruzione
    private bool isDying = false;

    void Start()
    {
        // Ottieni il riferimento all'oggetto con tag "ZoneDestroy"
        GameObject zoneDestroyObject = GameObject.FindGameObjectWithTag("ZoneDestroy");
        if (zoneDestroyObject != null)
        {
            zoneDestroyCollider = zoneDestroyObject.GetComponent<SphereCollider>();
        }
        else
        {
            Debug.LogWarning("ZoneDestroy non trovato nella scena!");
        }

        // Recupera la dimensione dell'enemy dalla SettingsManager
        float enemySize = 0f;
        SettingsManager settingsManager = FindObjectOfType<SettingsManager>();
        if (settingsManager != null)
        {
            enemySize = settingsManager.enemyHitboxSize;
        }
        else
        {
            enemySize = 1f;
        }

        const int maxAttempts = 10;
        int attempts = 0;
        bool validTarget = false;

        do
        {
            // Calcola un punto casuale sulla superficie della sfera di spawn
            targetPosition = sphereCenter + Random.onUnitSphere * sphereRadius;
            validTarget = true;

            if (zoneDestroyCollider != null)
            {
                // Calcola il centro reale del collider (considerando eventuali offset)
                Vector3 zoneCenter = zoneDestroyCollider.transform.position + zoneDestroyCollider.center;
                // Calcola il raggio effettivo considerando lo scale
                float zoneRadius = zoneDestroyCollider.radius * zoneDestroyCollider.transform.lossyScale.x;

                // Definisce A come il punto di partenza e B come il candidato target
                Vector3 A = transform.position;
                Vector3 B = targetPosition;
                Vector3 AB = B - A;

                // Calcola il parametro t per trovare il punto di proiezione su AB
                float t = Vector3.Dot(zoneCenter - A, AB) / AB.sqrMagnitude;
                t = Mathf.Clamp01(t);
                // Calcola il punto più vicino sulla linea
                Vector3 closestPoint = A + t * AB;
                float distance = Vector3.Distance(zoneCenter, closestPoint);

                // Se la distanza è inferiore a (zoneRadius + enemySize), la traiettoria interseca la zona
                if (distance < (zoneRadius + enemySize))
                {
                    validTarget = false;
                }
            }

            attempts++;
        } while (!validTarget && attempts < maxAttempts);

        // Se non viene trovato un target valido, il nemico non viene spawnato
        if (!validTarget)
        {
            Destroy(gameObject);
            return;
        }

        // Calcola la direzione normalizzata verso il target scelto
        moveDirection = (targetPosition - transform.position).normalized;

        // Trova il figlio con il MeshRenderer per animare la scala
        MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
        if (mr != null)
        {
            meshTransform = mr.transform;
            // Inizia con scala zero per l'animazione di spawn
            meshTransform.localScale = Vector3.zero;
            StartCoroutine(AnimateSpawn());
        }
    }

    void Update()
    {
        // Se l'animazione di distruzione è in corso, non aggiorniamo il movimento
        if (!isDying)
        {
            // Muove il nemico lungo la direzione calcolata
            transform.position += moveDirection * speed * Time.deltaTime;

            // Se la distanza dall'origine supera la soglia, avvia l'animazione di distruzione
            if (transform.position.magnitude > destroyDistance)
            {
                if (!isDying)
                    StartCoroutine(AnimateDestroy());
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ZoneDestroy"))
        {
            if (!isDying)
                StartCoroutine(AnimateDestroy());
        }
    }

    IEnumerator AnimateSpawn()
    {
        float elapsed = 0f;
        while (elapsed < spawnAnimTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / spawnAnimTime;
            float scale = Mathf.Lerp(0f, 0.5f, t);
            if (meshTransform != null)
                meshTransform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        // Assicura la scala finale esatta
        if (meshTransform != null)
            meshTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    IEnumerator AnimateDestroy()
    {
        isDying = true;
        float elapsed = 0f;
        while (elapsed < destroyAnimTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / destroyAnimTime;
            float scale = Mathf.Lerp(0.5f, 0f, t);
            if (meshTransform != null)
                meshTransform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        Destroy(gameObject);
    }
}
