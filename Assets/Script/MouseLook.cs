using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public float gyroSensitivity = 50f;
    public Transform playerBody;

    private PlayerInputActions inputActions;
    private Vector3 gyroInput;
    private float xRotation = 0f;
    private Quaternion initialRotation;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        initialRotation = transform.localRotation; // Salva la rotazione iniziale della telecamera
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
            float gyroX = gyroInput.x * gyroSensitivity * Time.unscaledDeltaTime; // Usa Time.unscaledDeltaTime per funzionare in pausa
            float gyroY = -gyroInput.y * gyroSensitivity * Time.unscaledDeltaTime;

            xRotation -= gyroX;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * gyroY);
        }
    }

    public void ResetCameraRotation()
    {
        transform.localRotation = initialRotation; // Resetta la telecamera
        playerBody.rotation = Quaternion.Euler(0f, playerBody.rotation.eulerAngles.y, 0f); // Reset Y del player
        xRotation = 0f;
    }
}
