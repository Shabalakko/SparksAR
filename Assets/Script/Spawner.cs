using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;         // Lista di Prefab dei nemici (A e B)
    public float[] spawnChances;              // Percentuali di spawn per ogni prefab (dovrebbero sommare 100 o essere intese come pesi)
    public Collider spawnArea;                // Area di spawn (usiamo un Collider Trigger)
    public float minSpawnTime = 2f;            // Tempo minimo tra uno spawn e l'altro
    public float maxSpawnTime = 5f;            // Tempo massimo tra uno spawn e l'altro
    public float spawnRadius = 1.5f;           // Raggio per controllare la sovrapposizione

    void Start()
    {
        // Verifica che il numero di percentuali corrisponda al numero di prefab
        if (spawnChances.Length != enemyPrefabs.Length)
        {
            Debug.LogWarning("spawnChances deve essere della stessa lunghezza di enemyPrefabs!");
        }
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
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
                // Seleziona un indice in base alle percentuali definite
                int enemyIndex = GetRandomEnemyIndex();
                if (enemyIndex != -1)
                {
                    GameObject enemyPrefab = enemyPrefabs[enemyIndex];
                    Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                }
            }
        }
    }

    // Metodo per selezionare casualmente un prefab basato sui pesi/percentuali
    int GetRandomEnemyIndex()
    {
        // Calcola la somma totale dei pesi
        float totalChance = 0;
        for (int i = 0; i < spawnChances.Length; i++)
        {
            totalChance += spawnChances[i];
        }

        // Se il totale è zero, non è possibile selezionare un nemico
        if (totalChance <= 0) return -1;

        // Genera un numero casuale tra 0 e totalChance
        float randomValue = Random.Range(0, totalChance);

        // Seleziona l'indice basandosi sui pesi
        for (int i = 0; i < spawnChances.Length; i++)
        {
            if (randomValue < spawnChances[i])
            {
                return i;
            }
            randomValue -= spawnChances[i];
        }

        // Fallback (dovrebbe raramente essere raggiunto)
        return spawnChances.Length - 1;
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
