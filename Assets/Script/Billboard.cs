using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update()
    {
        // Fa sì che il Canvas guardi sempre verso la telecamera
        transform.LookAt(Camera.main.transform);

        // Controlla l'angolo sull'asse X per evitare che sia capovolto
        Vector3 eulerAngles = transform.rotation.eulerAngles;

        // Se l'angolo sull'asse X è fuori dall'intervallo accettabile, lo corregge
        if (eulerAngles.x > 180f)
        {
            eulerAngles.x -= 360f;
        }

        // Limita l'inclinazione per evitare il capovolgimento
        eulerAngles.x = Mathf.Clamp(eulerAngles.x, -45f, 45f);

        // Applica la rotazione corretta
        transform.rotation = Quaternion.Euler(eulerAngles);
    }
}
