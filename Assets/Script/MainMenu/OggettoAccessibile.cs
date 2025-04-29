using UnityEngine;

public class OggettoAccessibile : MonoBehaviour
{
    private void OnEnable()
    {
        // Questo metodo viene chiamato quando l'oggetto viene attivato (anche all'istanza)
        VerificaStatoAccessibilita();
    }

    private void VerificaStatoAccessibilita()
    {
        if (AccessibilitaManager.Instance != null && AccessibilitaManager.Instance.disattivazioneAttiva)
        {
            gameObject.SetActive(false);
        }
    }
}