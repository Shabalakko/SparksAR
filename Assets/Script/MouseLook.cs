using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private PlayerInputActions inputActions;
    private Vector2 mouseInput;
    private float xRotation = 0f;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        // Blocca e nasconde il cursore al gioco
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        inputActions.CameraControls.Look.performed += ctx =>
        {
            mouseInput = ctx.ReadValue<Vector2>();
        };

        inputActions.CameraControls.Look.canceled += ctx =>
        {
            mouseInput = Vector2.zero;
        };

        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
        // Sblocca il cursore quando il componente viene disabilitato
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        float mouseX = mouseInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Ruota la telecamera in verticale
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // Ruota il corpo del player in orizzontale
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
