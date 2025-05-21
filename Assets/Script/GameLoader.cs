using UnityEngine;
using HISPlayerAPI; // Assicurati di avere questo namespace per accedere a HISPlayerAPI

public class GameLoader : MonoBehaviour
{
    // Variabile per impostare il Time.timeScale iniziale
    public float initialTimeScale = 0f;

    // Variabile per impostare il Time.timeScale quando l'evento viene riprodotto
    public float playbackTimeScale = 1f;

    void Awake()
    {
        // Imposta il Time.timeScale iniziale quando la scena viene caricata
        // Awake viene chiamato prima di Start, assicurando che sia impostato subito.
        Time.timeScale = initialTimeScale;
        Debug.Log($"Time.timeScale impostato a: {Time.timeScale} all'avvio della scena.");

        // È buona pratica impostare anche Time.fixedDeltaTime per coerenza con la fisica
        // soprattutto quando Time.timeScale è 0, per evitare che la fisica "salti" all'attivazione.
        // Se initialTimeScale è 0, fixedDeltaTime * initialTimeScale risulterà 0,
        // ma lo imposteremo correttamente a playbackTimeScale quando l'evento accade.
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // 0.02f è il Fixed Timestep di default (1/50)
    }

    void OnEnable()
    {
        // Iscriviti all'evento quando lo script è abilitato
        // Questo è il punto chiave per catturare l'evento del player.
        // Dalla traccia dello stack, sembra che l'evento sia EventPlaybackPlayed.
        // Se non funziona, potresti dover controllare la documentazione di HISPlayerAPI
        // per il nome esatto dell'evento o del delegate.
        //HISPlayerManager.EventPlaybackPlayed += OnPlaybackPlayed;
        Debug.Log("Iscritto all'evento HISPlayerManager.EventPlaybackPlayed.");
    }

    void OnDisable()
    {
        // Disiscriviti dall'evento quando lo script è disabilitato o distrutto
        // Questo previene memory leak e chiamate a oggetti inesistenti.
       // HISPlayerManager.EventPlaybackPlayed -= OnPlaybackPlayed;
        Debug.Log("Disiscritto dall'evento HISPlayerManager.EventPlaybackPlayed.");
    }

    // Questo metodo viene chiamato quando l'evento EventPlaybackPlayed si verifica
    private void OnPlaybackPlayed(HISPlayerEventInfo eventInfo)
    {
        // Imposta Time.timeScale a 1 (o al valore desiderato)
        Time.timeScale = playbackTimeScale;
        Debug.Log($"Evento EventPlaybackPlayed ricevuto! Time.timeScale impostato a: {Time.timeScale}");

        // Aggiorna anche Time.fixedDeltaTime per la fisica
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Assumendo 0.02f come Fixed Timestep di default

        // Se desideri che questo script si disabiliti o si distrugga dopo aver impostato il timeScale
        // (ad esempio, se è solo per la fase di caricamento iniziale), puoi farlo qui.
        // Destroyer(gameObject); // O this.enabled = false;
    }

    // Puoi aggiungere qui la logica per il caricamento o l'UI di caricamento
    void Start()
    {
        // Qui potresti attivare la tua schermata di caricamento UI,
        // o mostrare un messaggio "Caricamento in corso..."
        // Ricorda che le animazioni UI con Animator impostato su "Unscaled Time"
        // continueranno a funzionare anche con Time.timeScale a 0.
        Debug.Log("Logica di caricamento (UI, ecc.) avviata. Il gioco è in pausa.");
    }
}