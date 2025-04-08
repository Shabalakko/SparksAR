using UnityEngine;
using System.Collections.Generic;

public class EnemyTypeB : EnemyBase
{
    // Specifica il colore di questo nemico
    public override string EnemyColor => "Blue";

    [Tooltip("Lista di prefabs da istanziare casualmente")]
    public List<GameObject> prefabVarianti;

    [Tooltip("Riferimento all'oggetto vuoto figlio che determina la posizione")]
    public Transform spawnPoint;

    private GameObject istanzaAttuale; //Riferimento all'istanza creata

    public override void TakeDamage(float damage, string color)
    {
        if (color == EnemyColor)
        {
            currentHP -= damage;
            lastDamageTime = Time.time;
            if (currentHP <= 0)
            {
                // Se il nemico blu muore, ricarica l'energia rossa
                if (energyManager != null)
                {
                    energyManager.RechargeRed();
                }
                Die();
            }
        }
    }

    protected override void Start()
    {
        base.Start(); // Chiama l'override di Start nella classe EnemyBase

        // Istanzia un prefab casuale all'avvio
        //InstanziaPrefabCasuale();
    }

    /*void InstanziaPrefabCasuale()
    {
        if (prefabVarianti != null && prefabVarianti.Count > 0 && spawnPoint != null)
        {
            int indiceCasuale = Random.Range(0, prefabVarianti.Count);
            GameObject prefabSelezionato = prefabVarianti[indiceCasuale];

            // Istanzia il prefab selezionato nella posizione e rotazione dello spawnPoint
            istanzaAttuale = Instantiate(prefabSelezionato, spawnPoint.position, spawnPoint.rotation);
            istanzaAttuale.transform.SetParent(spawnPoint, true); //imposta come figlio
        }
        else
        {
            Debug.LogError("Manca la lista di prefabs o lo spawn point!", this);
            // Puoi anche istanziare un prefab di default o distruggere questo oggetto
            //Destroy(gameObject);
        }
    }*/

    public GameObject particellePrefab; // Aggiungi questo campo e assegnagli il prefab tramite Inspector
    protected override void Die()
    {
        // Istanzia l'effetto particellare nella posizione del nemico
        if (particellePrefab != null)
        {
            GameObject effetto = Instantiate(particellePrefab, transform.position, Quaternion.identity);
            ParticleSystem ps = effetto.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                // Distruggi l'effetto dopo la sua durata
                Destroy(effetto, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(effetto, 2f);
            }
        }
        if (istanzaAttuale != null)
        {
            Destroy(istanzaAttuale);
        }
        // Chiama la logica di morte originale (punteggio, popup, etc.) e distruggi il nemico
        base.Die();
    }
}
