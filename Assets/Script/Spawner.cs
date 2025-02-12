using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn; // Prefab del nemico
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 5f;
    public float spawnRadius = 1.5f; // Raggio per il controllo delle sovrapposizioni

    private Collider spawnArea;

    void Start()
    {
        // Ottiene il Collider dell'area di spawn
        spawnArea = GetComponent<Collider>();

        // Inizia lo spawning
        StartCoroutine(SpawnRoutine());
    }

    private System.Collections.IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Attende un tempo casuale prima di spawnare
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            // Ottiene una posizione valida per lo spawn
            Vector3 spawnPosition = GetValidSpawnPosition();

            // Se è stata trovata una posizione valida, istanzia l'oggetto
            if (spawnPosition != Vector3.zero)
            {
                GameObject newEnemy = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);

                // Assicurati che il collider per il controllo di spawn sia attivo
                Collider[] colliders = newEnemy.GetComponentsInChildren<Collider>();
                foreach (Collider col in colliders)
                {
                    if (!col.isTrigger)
                    {
                        col.enabled = true;
                    }
                }
            }
        }
    }

    // Ottiene una posizione casuale valida all'interno dell'area di spawn
    private Vector3 GetValidSpawnPosition()
    {
        Vector3 randomPosition;
        int maxAttempts = 20; // Numero massimo di tentativi per trovare una posizione libera
        int attempts = 0;

        do
        {
            Vector3 center = spawnArea.bounds.center;
            Vector3 size = spawnArea.bounds.size;

            // Calcolo delle posizioni casuali su X, Y e Z
            float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
            float y = Random.Range(center.y - size.y / 2, center.y + size.y / 2);
            float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

            randomPosition = new Vector3(x, y, z);

            // Controlla se l'area è libera usando OverlapSphere, ma ignora i Trigger
            Collider[] colliders = Physics.OverlapSphere(randomPosition, spawnRadius);

            bool isPositionValid = true;

            foreach (Collider col in colliders)
            {
                // Se il collider non è un Trigger, la posizione non è valida
                if (!col.isTrigger)
                {
                    isPositionValid = false;
                    break;
                }
            }

            // Se non ci sono altri oggetti vicini (non Trigger), la posizione è valida
            if (isPositionValid)
            {
                return randomPosition;
            }

            attempts++;
        }
        while (attempts < maxAttempts);

        // Se non trova una posizione valida, ritorna Vector3.zero
        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        // Mostra l'area di spawn nell'Editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(GetComponent<Collider>().bounds.center, GetComponent<Collider>().bounds.size);
    }
}
