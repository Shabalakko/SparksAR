using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    private static SettingsManager instance;
    public string buttonName = "MyButton";
    public string playButtonName = "PlayButton";
    public Sprite activeSprite; // Sprite da mostrare quando Cesium è attivo
    public Sprite inactiveSprite; // Sprite da mostrare quando Cesium è inattivo
    private GameObject buttonReference;
    private bool isCesiumActive;
    private string mainMenuSceneName = "MainMenu";
    private bool listenerAdded = false; // Track if the listener has been added
    private bool isToggling = false;    // Track if ToggleCesiumGeoreference is already running
    private float lastClickTime = 0f;
    public float clickDebounceTime = 0.5f; // Adjust as needed to prevent multiple calls

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            // Carica lo stato salvato, se non esiste, usa il valore di default della scena.
            // MODIFICA QUI: Imposta il valore di default a false (0) se non è salvato.
            isCesiumActive = PlayerPrefs.GetInt("CesiumGeoreferenceActive", 0) == 1;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StopAllCoroutines();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mainMenuSceneName)
        {
            StartCoroutine(AssignPlayButton());
            listenerAdded = false; // Reset the listenerAdded flag when returning to the main menu
        }
    }

    private IEnumerator AssignPlayButton()
    {
        GameObject playButton = GameObject.Find(playButtonName);
        if (playButton == null)
        {
            Debug.LogError("PlayButton not found: " + playButtonName + " in scene " + mainMenuSceneName);
            yield break;
        }
        Button playButtonComponent = playButton.GetComponent<Button>();
        if (playButtonComponent == null)
        {
            Debug.LogError("PlayButton Component is null on: " + playButtonName);
            yield break;
        }

        yield return new WaitUntil(() => playButton.activeInHierarchy);
        Debug.Log("PlayButton is active.  Attaching listener.");
        playButtonComponent.onClick.RemoveAllListeners();
        playButtonComponent.onClick.AddListener(FindButton);
    }

    public void FindButton()
    {
        StartCoroutine(FindAndAssignButton());
    }

    private IEnumerator FindAndAssignButton()
    {
        buttonReference = GameObject.Find(buttonName);
        if (buttonReference == null)
        {
            Debug.LogError("ButtonReference not found: " + buttonName);
            yield break;
        }

        // Add this check.
        if (!buttonReference.activeInHierarchy)
        {
            Debug.LogError("ButtonReference is not active in hierarchy: " + buttonName);
            yield break;
        }

        yield return new WaitUntil(() => buttonReference.activeInHierarchy);
        Debug.Log(buttonName + " is active.  Attaching listener.");

        UpdateButtonSprite(isCesiumActive);
        Button targetButton = buttonReference.GetComponent<Button>();
        if (targetButton != null)
        {
            // Check if the listener has already been added
            if (!listenerAdded)
            {
                targetButton.onClick.AddListener(ToggleCesiumGeoreference);
                listenerAdded = true; // Set the flag to true after adding the listener
            }
            else
            {
                Debug.LogWarning("Listener for ToggleCesiumGeoreference already exists on " + buttonName);
            }
        }
        else
        {
            Debug.LogError("TargetButton component is null on: " + buttonName);
        }
    }

    public void ToggleCesiumGeoreference()
    {
        // Debounce the button click
        if (Time.time - lastClickTime < clickDebounceTime)
        {
            Debug.LogWarning("Button click debounced.");
            return;
        }

        lastClickTime = Time.time; // Update the last click time

        Debug.Log("ToggleCesiumGeoreference called, previous state: " + isCesiumActive);
        isCesiumActive = !isCesiumActive;
        Debug.Log("New state: " + isCesiumActive);
        PlayerPrefs.SetInt("CesiumGeoreferenceActive", isCesiumActive ? 1 : 0);
        PlayerPrefs.Save();
        UpdateButtonSprite(isCesiumActive);
    }

    private void UpdateButtonSprite(bool isActive)
    {
        if (buttonReference == null)
        {
            return;
        }
        Image buttonImage = buttonReference.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.sprite = isActive ? activeSprite : inactiveSprite;
        }
        else
        {
            Debug.LogError("Image component is null on: " + buttonName);
        }
    }
}