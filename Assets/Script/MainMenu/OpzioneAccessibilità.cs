using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class AccessibilitaManager : MonoBehaviour
{
    public static AccessibilitaManager Instance { get; private set; }

    public bool disattivazioneAttiva { get; private set; } = false;
    public Color coloreAttivo = Color.green;
    public Color coloreDisattivo = Color.red;
    public Image bottoneImageMainMenu; // Assicurati che sia assegnato nell'Inspector
    private const string prefKeyDisattivazioneAttiva = "DisattivazioneAccessibilitaAttiva";
    public string nomeBottoneMainMenu = "NomeDelTuoBottone"; // Sostituisci "NomeDelTuoBottone" con il nome effettivo del GameObject del tuo bottone nel MainMenu
    private bool isClicking = false;
    private float clickDelay = 0.2f; // Impedisci altri click per 0.2 secondi

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        AggiornaColoreBottone(); // Aggiorna il colore iniziale
        ApplicaStatoDisattivazioneAlleSceneCaricate(); // Applica lo stato alla prima scena caricata
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scena caricata: {scene.name}");
        ApplicaStatoDisattivazioneAllaScena(scene);

        GameObject bottoneGO = null;
        if (!string.IsNullOrEmpty(nomeBottoneMainMenu))
        {
            bottoneGO = TrovaOggettoPerNomeNellaScena(scene, nomeBottoneMainMenu);
        }
        else
        {
            bottoneGO = GameObject.FindGameObjectWithTag("BottoneMainMenuTag");
        }

        if (bottoneGO != null)
        {
            bottoneImageMainMenu = bottoneGO.GetComponent<Image>();
            Button bottoneComponent = bottoneGO.GetComponent<Button>(); // Ottieni il componente Button

            if (bottoneImageMainMenu != null)
            {
                AggiornaColoreBottone();
            }
            else
            {
                Debug.LogError($"Trovato l'oggetto '{bottoneGO.name}' nella scena '{scene.name}', ma non ha un componente Image.");
            }

            if (bottoneComponent != null)
            {
                // Rimuovi eventuali listener precedenti per evitare chiamate multiple
                bottoneComponent.onClick.RemoveAllListeners();
                // Aggiungi un nuovo listener che chiama il metodo ToggleDisattivazione di questa istanza di AccessibilitaManager
                bottoneComponent.onClick.AddListener(ToggleDisattivazione);
            }
            else
            {
                Debug.LogError($"Trovato l'oggetto '{bottoneGO.name}' nella scena '{scene.name}', ma non ha un componente Button.");
            }
        }
        else
        {
            Debug.LogWarning($"Bottone con nome '{nomeBottoneMainMenu}' o tag 'BottoneMainMenuTag' non trovato nella scena '{scene.name}'.");
            bottoneImageMainMenu = null;
        }
    }


    private void CaricaStatoDisattivazione()
    {
        if (PlayerPrefs.HasKey(prefKeyDisattivazioneAttiva))
        {
            disattivazioneAttiva = PlayerPrefs.GetInt(prefKeyDisattivazioneAttiva) == 1;
        }
        else
        {
            disattivazioneAttiva = false;
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
        if (!isClicking)
        {
            isClicking = true;
            disattivazioneAttiva = !disattivazioneAttiva;
            Debug.Log($"Disattivazione oggetti tra scene: {disattivazioneAttiva}");

            SalvaStatoDisattivazione();
            AggiornaColoreBottone();
            ApplicaStatoDisattivazioneAlleSceneCaricate();

            // Riabilita i click dopo un breve ritardo
            Invoke("ResetClicking", clickDelay);
        }
    }

    private void ResetClicking()
    {
        isClicking = false;
    }

    private void AggiornaColoreBottone()
    {
        if (bottoneImageMainMenu != null)
        {
            bottoneImageMainMenu.color = disattivazioneAttiva ? coloreDisattivo : coloreAttivo;
        }
        else
        {
            Debug.LogWarning("Riferimento all'immagine del bottone nel MainMenu non ancora disponibile.");
        }
    }

    public void ApplicaStatoDisattivazioneAllaScena(Scene scene)
    {
        foreach (var gruppo in listaOggettiDaDisattivare)
        {
            if (gruppo.nomeScena == scene.name)
            {
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
            GameObject oggettoTrovato = TrovaOggettoPerNomeNellaScena(scena, nomeOggetto);
            if (oggettoTrovato != null)
            {
                oggettoTrovato.SetActive(attiva);
                Debug.Log($"{(attiva ? "Riattivato" : "Disattivato")} l'oggetto '{nomeOggetto}' nella scena '{scena.name}'.");
            }
            else
            {
                Debug.LogWarning($"Oggetto '{nomeOggetto}' non trovato nella scena '{scena.name}' per {(attiva ? "riattivazione" : "disattivazione")}.");
            }
        }
    }

    private GameObject TrovaOggettoPerNomeNellaScena(Scene scene, string nomeOggetto)
    {
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject rootObject in rootObjects)
        {
            Transform foundTransform = rootObject.transform.Find(nomeOggetto);
            if (foundTransform != null)
            {
                return foundTransform.gameObject;
            }
            // Ricerca ricorsiva (opzionale, ma più completa)
            Transform RecursiveFindChild(Transform parent, string name)
            {
                foreach (Transform child in parent)
                {
                    if (child.name == name)
                    {
                        return child;
                    }
                    Transform found = RecursiveFindChild(child, name);
                    if (found != null)
                    {
                        return found;
                    }
                }
                return null;
            }

            Transform foundRecursive = RecursiveFindChild(rootObject.transform, nomeOggetto);
            if (foundRecursive != null)
            {
                return foundRecursive.gameObject;
            }
        }
        return null;
    }
}