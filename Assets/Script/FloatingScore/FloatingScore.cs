using UnityEngine;
using TMPro;

public class FloatingScore : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    public void Initialize(int score)
    {
        scoreText.text = $"+{score}";
    }

    // Metodo da chiamare alla fine dell'animazione
    public void DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }
    public void EndAnimation()
    {
        // Disattiva l'oggetto invece di distruggerlo, pronto per il prossimo utilizzo
        gameObject.SetActive(false);
    }

}
