using UnityEngine;

public class EnemyEllipticalMovement : MonoBehaviour
{
    public float orbitSpeed = 1f;             // Velocità dell'orbita
    public float ellipseWidth = 5f;           // Ampiezza dell'ellisse sull'asse X
    public float ellipseHeight = 3f;          // Ampiezza dell'ellisse sull'asse Z
    public float verticalAmplitude = 2f;      // Ampiezza verticale dell'ellisse (asse Y)
    public float rotationSpeed = 20f;         // Velocità di rotazione dell'ellisse
    public float transitionSpeed = 0.5f;      // Velocità di transizione tra le orbite
    public Transform meshTransform;

    private Vector3 startPosition;
    private Vector3 currentEllipse;
    private Vector3 nextEllipse;
    private float angle = 0f;
    private float rotationAngle = 0f;
    private Vector3 previousVelocity;


    void Start()
    {
        // Memorizza la posizione iniziale
        startPosition = transform.position;

        // Imposta un'ellisse di partenza
        currentEllipse = CalculateEllipse();
        nextEllipse = currentEllipse;
        previousVelocity = transform.forward;
    }

    void Update()
    {
        // Aggiorna l'angolo per il movimento ellittico
        angle += orbitSpeed * Time.deltaTime;
        if (angle > 360f) angle -= 360f;

        // Aggiorna l'angolo di rotazione dell'ellisse
        rotationAngle += rotationSpeed * Time.deltaTime;
        if (rotationAngle > 360f) rotationAngle -= 360f;
        Quaternion rotation = Quaternion.Euler(0, rotationAngle, 0);

        // Calcola la posizione ellittica
        Vector3 targetPosition = startPosition;
        targetPosition.x += Mathf.Cos(angle) * currentEllipse.x;
        targetPosition.y += Mathf.Sin(angle * 2) * currentEllipse.y;
        targetPosition.z += Mathf.Sin(angle) * currentEllipse.z;
        targetPosition = startPosition + rotation * (targetPosition - startPosition);

        // Memorizza la posizione precedente per il calcolo della direzione di movimento
        Vector3 previousPosition = transform.position;

        // Muove il GameObject padre in modo fluido verso la posizione target
        transform.position = Vector3.Lerp(transform.position, targetPosition, transitionSpeed * Time.deltaTime);

        // Calcola la direzione effettiva del movimento
        Vector3 movementDirection = (transform.position - previousPosition).normalized;
        if (movementDirection.sqrMagnitude > 0.001f)
        {
            // Ruota il figlio in modo che il suo asse Z punti verso la direzione di movimento
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            meshTransform.rotation = Quaternion.Slerp(meshTransform.rotation, targetRotation, transitionSpeed * Time.deltaTime);
        }

        // Cambia dinamicamente l'ellisse per il prossimo loop se l'angolo è quasi completato
        if (angle >= 359f)
        {
            currentEllipse = nextEllipse;
            nextEllipse = CalculateEllipse();
        }
    }


    // Calcola un'ellisse casuale per variare il movimento
    Vector3 CalculateEllipse()
    {
        return new Vector3(
            Random.Range(ellipseWidth * 0.8f, ellipseWidth * 1.2f),    // Asse X
            Random.Range(verticalAmplitude * 0.5f, verticalAmplitude), // Asse Y (fluttuazione verticale)
            Random.Range(ellipseHeight * 0.8f, ellipseHeight * 1.2f)   // Asse Z
        );
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ZoneDestroy"))
        {
            Destroy(gameObject);
        }
    }
}
