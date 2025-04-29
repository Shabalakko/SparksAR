using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class AccessibilitaManager : MonoBehaviour
{
    public static AccessibilitaManager Instance { get; private set; }

    // Ora disattivazioneAttiva significa che gli oggetti dovrebbero essere DISATTIVATI
    public bool disattivazioneAttiva { get; private set; } = false;
    public Color coloreAttivo = Color.green; // Il colore "attivo" ora significa oggetti ATTIVI
    public Color coloreDisattivo = Color.red; // Il colore "disattivo" ora significa oggetti DISATTIVATI
    public Image bottoneImageMainMenu;
    private const string prefKeyDisattivazioneAttiva = "DisattivazioneAccessibilitaAttiva";

    [System.Serializable]
    public struct OggettiDaDisattivarePerScena
    {
        public string nomeScena;
        public List<string> nomiOggetti;
    }

    public List<OggettiDaDisattivarePerScena> listaOggettiDaDisattivare;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CaricaStatoDisattivazione();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ApplicaStatoDisattivazioneAlleSceneCaricate();
        AggiornaColoreBottone();
    }

    private void CaricaStatoDisattivazione()
    {
        if (PlayerPrefs.HasKey(prefKeyDisattivazioneAttiva))
        {
            disattivazioneAttiva = PlayerPrefs.GetInt(prefKeyDisattivazioneAttiva) == 1;
        }
        else
        {
            disattivazioneAttiva = false; // Stato predefinito: oggetti attivi (bottone rosso)
        }
        Debug.Log($"Stato disattivazione caricato: {disattivazioneAttiva}");
    }

    private void SalvaStatoDisattivazione()
    {
        PlayerPrefs.SetInt(prefKeyDisattivazioneAttiva, disattivazioneAttiva ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log($"Stato disattivazione salvato: {disattivazioneAttiva}");
    }

    public void ToggleDisattivazione()
    {
        disattivazioneAttiva = !disattivazioneAttiva;
        Debug.Log($"Disattivazione oggetti tra scene: {disattivazioneAttiva}");

        SalvaStatoDisattivazione();
        AggiornaColoreBottone();
        ApplicaStatoDisattivazioneAlleSceneCaricate();
    }

    private void AggiornaColoreBottone()
    {
        if (bottoneImageMainMenu != null)
        {
            // Se disattivazioneAttiva è true (gli oggetti sono disattivati), il bottone è rosso
            bottoneImageMainMenu.color = disattivazioneAttiva ? coloreDisattivo : coloreAttivo;
        }
        else
        {
            Debug.LogWarning("Riferimento all'immagine del bottone nel MainMenu non assegnato.");
        }
    }

    public void ApplicaStatoDisattivazioneAllaScena(Scene scene)
    {
        foreach (var gruppo in listaOggettiDaDisattivare)
        {
            if (gruppo.nomeScena == scene.name)
            {
                // Se disattivazioneAttiva è true, disattiviamo gli oggetti (setActive(false))
                DisattivaOggettiNellaScena(scene, gruppo.nomiOggetti, !disattivazioneAttiva);
            }
        }
    }

    public void ApplicaStatoDisattivazioneAlleSceneCaricate()
    {
        int sceneCount = SceneManager.sceneCount;
        for (int i = 0; i < sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            ApplicaStatoDisattivazioneAllaScena(scene);
        }
    }

    private void DisattivaOggettiNellaScena(Scene scena, List<string> nomiOggetti, bool attiva)
    {
        foreach (string nomeOggetto in nomiOggetti)
        {
            GameObject oggettoTrovato = TrovaOggettoNellaScena(scena, nomeOggetto);
            if (oggettoTrovato != null)
            {
                oggettoTrovato.SetActive(attiva);
                Debug.Log($"{(attiva ? "Riattivato" : "Disattivato")} l'oggetto '{nomeOggetto}' nella scena '{scena.name}'.");
            }
            else
            {
                // Non loggare warning qui
            }
        }
    }

    private GameObject TrovaOggettoNellaScena(Scene scena, string nomeOggetto)
    {
        GameObject[] rootObjects = scena.GetRootGameObjects();
        foreach (GameObject rootObject in rootObjects)
        {
            Transform foundTransform = rootObject.transform.Find(nomeOggetto);
            if (foundTransform != null)
            {
                return foundTransform.gameObject;
            }
        }
        return null;
    }
}