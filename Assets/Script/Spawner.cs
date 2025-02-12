using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;         // Lista di Prefab dei nemici (A e B)
    public Collider spawnArea;                // Area di spawn (usiamo un Collider Trigger)
    public float minSpawnTime = 2f;            // Tempo minimo tra uno spawn e l'altro
    public float maxSpawnTime = 5f;            // Tempo massimo tra uno spawn e l'altro
    public float spawnRadius = 1.5f;           // Raggio per controllare la sovrapposizione

    void Start()
    {
        // Inizia il ciclo di spawn
        StartCoroutine(SpawnEnemy());
    }

    System.Collections.IEnumerator SpawnEnemy()
    {
        while (true)
        {
            // Attende un tempo casuale tra minSpawnTime e maxSpawnTime
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            // Calcola una posizione casuale all'interno dell'area di spawn
            Vector3 spawnPosition = GetRandomPointInArea();

            // Controlla se c'è spazio per spawnare un nuovo nemico
            if (!Physics.CheckSphere(spawnPosition, spawnRadius))
            {
                // Sceglie casualmente un nemico dalla lista dei Prefab
                int randomIndex = Random.Range(0, enemyPrefabs.Length);
                GameObject enemyPrefab = enemyPrefabs[randomIndex];

                // Instanzia il nemico selezionato
                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    // Ottiene un punto casuale all'interno dell'area di spawn
    Vector3 GetRandomPointInArea()
    {
        Vector3 point = new Vector3(
            Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
            Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
            Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z)
        );

        // Assicura che il punto sia all'interno del Collider (nel caso sia irregolare)
        if (spawnArea.bounds.Contains(point))
        {
            return point;
        }
        else
        {
            // Se il punto è fuori dal Collider, riprova
            return GetRandomPointInArea();
        }
    }
}
