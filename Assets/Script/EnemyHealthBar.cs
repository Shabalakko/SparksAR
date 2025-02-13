using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image imageBackground;
    public Image imageFill;
    public Canvas canvas;
    private IEnemy enemyScript;  // Cambiato da Enemy a IEnemy
    private bool isDamaged = false;

    void Start()
    {
        // Trova automaticamente lo script corretto che implementa IEnemy
        enemyScript = GetComponentInParent<IEnemy>();

        // Nasconde la barra degli HP all'inizio
        canvas.enabled = false;
    }

    void Update()
    {
        if (enemyScript == null) return;

        float hpFraction = enemyScript.GetCurrentHP() / enemyScript.maxHP;
        imageFill.fillAmount = hpFraction;

        if (isDamaged)
        {
            imageBackground.fillAmount = Mathf.Lerp(imageBackground.fillAmount, imageFill.fillAmount, Time.deltaTime * 2);
            if (Mathf.Abs(imageBackground.fillAmount - imageFill.fillAmount) < 0.01f)
            {
                isDamaged = false;
            }
        }

        if (hpFraction < 1f || IsTargeted())
        {
            canvas.enabled = true;
        }
        else
        {
            canvas.enabled = false;
        }
    }

    public void TakeDamage()
    {
        isDamaged = true;
        imageBackground.fillAmount = imageFill.fillAmount;
    }

    private bool IsTargeted()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            return hit.collider.gameObject == ((MonoBehaviour)enemyScript).gameObject;
        }
        return false;
    }
}
