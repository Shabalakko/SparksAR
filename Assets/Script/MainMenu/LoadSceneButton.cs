using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{
    
    void LoadScene()
    {
        Time.timeScale = 1;
        // Carica la scena specificata
        SceneManager.LoadScene("MainMenu"); 
    }

}
