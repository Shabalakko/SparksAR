using System.Collections;
using UnityEngine;

public class EnemyChaseMovement : MonoBehaviour
{
    // Velocità di movimento del nemico
    public float speed = 5f;
    // Distanza massima dall'origine oltre la quale il nemico viene distrutto
    public float destroyDistance = 20f;

    // Durata dell'animazione di spawn e distruzione
    public float spawnAnimTime = 1f;
    public float destroyAnimTime = 1f;

    // Direzione di movimento calcolata in Start() (verso il giocatore)
    private Vector3 moveDirection;

    // Riferimento al figlio con il MeshRenderer per animare la scala
    private Transform meshTransform;

    // Flag per evitare di avviare più volte l'animazione di distruzione
    private bool isDying = false;

    // Riferimento al giocatore
    private GameObject player;

    // Riferimento al collider "ZoneDestroy"
    private SphereCollider zoneDestroyCollider;

    void Start()
    {
        // Cerca il giocatore tramite il tag "Player"
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Calcola la direzione normalizzata verso il giocatore
            moveDirection = (player.transform.position - transform.position).normalized;
        }
        else
        {
            Debug.LogWarning("Player non trovato nella scena!");
            // Se il giocatore non viene trovato, muovi l'enemy in avanti come fallback
            moveDirection = transform.forward;
        }

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

        // Trova il figlio con il MeshRenderer per animare la scala (animazione di spawn)
        MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
        if (mr != null)
        {
            meshTransform = mr.transform;
            meshTransform.localScale = Vector3.zero;
            StartCoroutine(AnimateSpawn());
        }
    }

    void Update()
    {
        // Se non è in corso l'animazione di distruzione, aggiorna il movimento
        if (!isDying)
        {
            transform.position += moveDirection * speed * Time.deltaTime;

            // Se l'enemy si allontana troppo dall'origine, avvia l'animazione di distruzione
            if (transform.position.magnitude > destroyDistance)
            {
                if (!isDying)
                    StartCoroutine(AnimateDestroy());
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Se l'enemy entra in collisione con l'oggetto "ZoneDestroy", avvia la distruzione
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

