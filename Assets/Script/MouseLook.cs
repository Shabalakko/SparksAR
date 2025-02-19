using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public float gyroSensitivity = 50f;   // Sensibilità del giroscopio
    public Transform playerBody;          // Riferimento al corpo del giocatore (per ruotare l'intero oggetto)

    private PlayerInputActions inputActions;
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
            if (ctx.control.device is UnityEngine.InputSystem.Gyroscope)
            {
                gyroInput = ctx.ReadValue<Vector3>();
            }
        };

        inputActions.CameraControls.Look.canceled += ctx =>
        {
            if (ctx.control.device is UnityEngine.InputSystem.Gyroscope)
            {
                gyroInput = Vector3.zero;
            }
        };

        inputActions.Enable();

        // Abilita il giroscopio su Android
        if (SystemInfo.supportsGyroscope && UnityEngine.InputSystem.Gyroscope.current != null)
        {
            InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
        }
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        if (Application.platform == RuntimePlatform.Android && UnityEngine.InputSystem.Gyroscope.current != null)
        {
            // Applica la rotazione angolare del giroscopio
            float gyroX = gyroInput.x * gyroSensitivity * Time.deltaTime;
            float gyroY = -gyroInput.y * gyroSensitivity * Time.deltaTime;

            xRotation -= gyroX;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * gyroY);
        }
    }
}
