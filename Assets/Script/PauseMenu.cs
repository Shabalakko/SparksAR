using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Slider mouseSensitivitySlider; // Riferimento allo slider nel menù di pausa

    private bool isPaused = false;
    private MouseLook mouseLook;
    private PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.CameraControls.Pause.performed += OnPausePerformed;
        inputActions.CameraControls.Enable();
    }

    void OnDisable()
    {
        inputActions.CameraControls.Pause.performed -= OnPausePerformed;
        inputActions.CameraControls.Disable();
    }

    void Start()
    {
        mouseLook = FindObjectOfType<MouseLook>(); // Trova lo script MouseLook

        // Inizializza lo slider con il valore corrente della sensibilità
        if (mouseSensitivitySlider != null && mouseLook != null)
        {
            mouseSensitivitySlider.value = mouseLook.mouseSensitivity;
            // Aggiungi un listener per aggiornare la sensibilità quando lo slider cambia
            mouseSensitivitySlider.onValueChanged.AddListener(UpdateMouseSensitivity);
        }
    }

    void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
        // Mostra il cursore per interagire con il menù
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        // Blocca e nascondi il cursore quando si riprende il gioco
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Questo metodo viene chiamato quando lo slider cambia valore
    void UpdateMouseSensitivity(float newSensitivity)
    {
        if (mouseLook != null)
        {
            mouseLook.mouseSensitivity = newSensitivity;
        }
    }

    public void ResetCamera()
    {
        if (mouseLook != null)
        {
            // mouseLook.ResetCameraRotation();
        }
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
