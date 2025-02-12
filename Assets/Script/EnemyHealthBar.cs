using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image imageBackground;  // Cerchio bianco per il danno
    public Image imageFill;        // Cerchio rosso per gli HP attuali
    public Canvas canvas;          // Canvas dell'anello HP
    public Enemy enemyScript;      // Riferimento allo script del nemico

    private bool isDamaged = false;

    void Start()
    {
        // Nasconde la barra degli HP all'inizio
        canvas.enabled = false;
    }

    void Update()
    {
        // Aggiorna la barra degli HP in base alla salute attuale
        float hpFraction = enemyScript.GetCurrentHP() / enemyScript.maxHP;
        imageFill.fillAmount = hpFraction;

        // Se è stato inflitto danno, aggiorna l'anello bianco
        if (isDamaged)
        {
            imageBackground.fillAmount = Mathf.Lerp(imageBackground.fillAmount, imageFill.fillAmount, Time.deltaTime * 2);
            if (Mathf.Abs(imageBackground.fillAmount - imageFill.fillAmount) < 0.01f)
            {
                isDamaged = false;
            }
        }

        // Verifica se il nemico è danneggiato o mirato
        if (hpFraction < 1f || IsTargeted())
        {
            canvas.enabled = true;
        }
        else
        {
            canvas.enabled = false;
        }
    }

    // Chiamato quando il nemico subisce danno
    public void TakeDamage()
    {
        isDamaged = true;
        imageBackground.fillAmount = imageFill.fillAmount;
    }

    // Verifica se il nemico è mirato
    private bool IsTargeted()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            return hit.collider.gameObject == enemyScript.gameObject;
        }

        return false;
    }

    public void UpdateHealthBar()
    {
        // Aggiorna la barra degli HP in base alla salute attuale
        float hpFraction = enemyScript.GetCurrentHP() / enemyScript.maxHP;
        imageFill.fillAmount = hpFraction;
    }

}
