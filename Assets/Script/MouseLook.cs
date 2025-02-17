using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Sensibilità del mouse
    public float gyroSensitivity = 50f;   // Sensibilità del giroscopio
    public Transform playerBody;          // Riferimento al corpo del giocatore (per ruotare l'intero oggetto)

    private PlayerInputActions inputActions;
    private Vector2 mouseInput;
    private Vector3 gyroInput;
    private float xRotation = 0f;

    void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.CameraControls.Look.performed += ctx =>
        {
            // Verifica il tipo di dato ricevuto
            if (ctx.control.device is Pointer) // Se è un mouse
            {
                mouseInput = ctx.ReadValue<Vector2>();
            }
            else if (ctx.control.device is UnityEngine.InputSystem.Gyroscope) // Specifica esplicita per evitare ambiguità
            {
                gyroInput = ctx.ReadValue<Vector3>();
            }
        };

        inputActions.CameraControls.Look.canceled += ctx =>
        {
            if (ctx.control.device is Pointer)
            {
                mouseInput = Vector2.zero;
            }
            else if (ctx.control.device is UnityEngine.InputSystem.Gyroscope) // Specifica esplicita per evitare ambiguità
            {
                gyroInput = Vector3.zero;
            }
        };

        inputActions.Enable();

        // Abilita il giroscopio su Android con la specifica esplicita
        if (SystemInfo.supportsGyroscope && UnityEngine.InputSystem.Gyroscope.current != null)
        {
            InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
        }
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Start()
    {
        // Blocca il cursore al centro dello schermo e lo rende invisibile (solo su PC)
        if (Application.platform != RuntimePlatform.Android)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        // --- Controllo con il Mouse (PC) ---
        if (Application.platform != RuntimePlatform.Android)
        {
            float mouseX = mouseInput.x * mouseSensitivity * Time.deltaTime;
            float mouseY = mouseInput.y * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }

        // --- Controllo con il Giroscopio (solo su Android) ---
        if (Application.platform == RuntimePlatform.Android && UnityEngine.InputSystem.Gyroscope.current != null)
        {
            // Applica la rotazione angolare del giroscopio
            float gyroX = -gyroInput.x * gyroSensitivity * Time.deltaTime;
            float gyroY = gyroInput.y * gyroSensitivity * Time.deltaTime;

            xRotation -= gyroX;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * gyroY);
        }
    }
}
