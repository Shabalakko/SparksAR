using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadComboMulti : MonoBehaviour
{
    public void PlayGameGyro()
    {
        // Carica la scena di gioco
        SceneManager.LoadScene("GyroLevel");
    }

    public void PlayGameCC()
    {
        SceneManager.LoadScene("GyroLevelCombination");
    }
    public void OpenShop()
    {
        // Carica la scena del negozio
        SceneManager.LoadScene("ModelShop");
    }
}
