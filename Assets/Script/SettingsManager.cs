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
    public string skyGameObjectName = "SKY"; // New: Name of the SKY GameObject

    private GameObject buttonReference;
    private GameObject skyGameObjectReference; // New: Reference to the SKY GameObject
    private bool isCesiumActive;
    private string mainMenuSceneName = "MainMenu";
    private bool listenerAdded = false; // Track if the listener has been added
    private float lastClickTime = 0f;
    public float clickDebounceTime = 0.5f; // Adjust as needed to prevent multiple calls

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            // Carica lo stato salvato, se non esiste, usa il valore di default della scena.
            isCesiumActive = PlayerPrefs.GetInt("CesiumGeoreferenceActive", -1) == -1
                ? true // Assumiamo che il valore di default nella scena sia true
                : PlayerPrefs.GetInt("CesiumGeoreferenceActive", 1) == 1; //altrimenti usa il valore salvato
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
        else // This assumes you're in a "game" scene when not in the main menu
        {
            // Find the SKY GameObject when a new scene (not main menu) is loaded
            skyGameObjectReference = GameObject.Find(skyGameObjectName);
            if (skyGameObjectReference == null)
            {
                Debug.LogWarning("SKY GameObject not found: " + skyGameObjectName + " in scene " + scene.name);
            }
            // Update the SKY GameObject's state based on the initial Cesium state
            UpdateSkyGameObject(isCesiumActive);
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
        Debug.Log("PlayButton is active. Attaching listener.");
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

        if (!buttonReference.activeInHierarchy)
        {
            Debug.LogError("ButtonReference is not active in hierarchy: " + buttonName);
            yield break;
        }

        yield return new WaitUntil(() => buttonReference.activeInHierarchy);
        Debug.Log(buttonName + " is active. Attaching listener.");

        UpdateButtonSprite(isCesiumActive);
        Button targetButton = buttonReference.GetComponent<Button>();
        if (targetButton != null)
        {
            if (!listenerAdded)
            {
                targetButton.onClick.AddListener(ToggleCesiumGeoreference);
                listenerAdded = true;
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
        if (Time.time - lastClickTime < clickDebounceTime)
        {
            Debug.LogWarning("Button click debounced.");
            return;
        }

        lastClickTime = Time.time;

        Debug.Log("ToggleCesiumGeoreference called, previous state: " + isCesiumActive);
        isCesiumActive = !isCesiumActive;
        Debug.Log("New state: " + isCesiumActive);
        PlayerPrefs.SetInt("CesiumGeoreferenceActive", isCesiumActive ? 1 : 0);
        PlayerPrefs.Save();
        UpdateButtonSprite(isCesiumActive);

        // New: Toggle SKY GameObject active state
        UpdateSkyGameObject(isCesiumActive);
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

    // New: Method to update the active state of the SKY GameObject
    private void UpdateSkyGameObject(bool isCesiumActiveState)
    {
        // If the SKY GameObject hasn't been found yet, try to find it.
        // This is important if you're transitioning directly into a scene
        // where the SKY object exists without going through the main menu first.
        if (skyGameObjectReference == null)
        {
            skyGameObjectReference = GameObject.Find(skyGameObjectName);
            if (skyGameObjectReference == null)
            {
                Debug.LogWarning("SKY GameObject not found to update its state: " + skyGameObjectName);
                return;
            }
        }

        // SKY is active when Cesium is INACTIVE, and vice-versa
        skyGameObjectReference.SetActive(!isCesiumActiveState);
        Debug.Log("SKY GameObject active state set to: " + !isCesiumActiveState + " (Cesium Active: " + isCesiumActiveState + ")");
    }
}