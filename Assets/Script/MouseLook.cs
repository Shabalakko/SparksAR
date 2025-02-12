using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Sensibilità del mouse
    public Transform playerBody; // Riferimento al corpo del giocatore (per ruotare l'intero oggetto)

    private float xRotation = 0f;

    void Start()
    {
        // Blocca il cursore al centro dello schermo e lo rende invisibile
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Ottieni l'input del mouse
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;


        // Calcola la rotazione verticale (su e giù)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limita la rotazione verticale

        // Applica la rotazione alla telecamera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Ruota il corpo del giocatore orizzontalmente (sinistra e destra)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}