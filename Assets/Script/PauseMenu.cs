using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;
    private MouseLook mouseLook;

    [Header("Laser Management")]
    public GameObject Lasers;
    public GameObject PLaserR, PLaserB;

    void Start()
    {
        mouseLook = FindObjectOfType<MouseLook>(); // Trova lo script MouseLook
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
        DisableLasers(); // Disattiva i laser quando il gioco è in pausa
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        EnableLasers(); // Riattiva i laser quando si riprende il gioco
    }

    public void ResetCamera()
    {
        if (mouseLook != null)
        {
            mouseLook.ResetCameraRotation();
        }
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        EnableLasers(); // Riattiva i laser quando si torna al menù principale (se necessario)
    }

    private void DisableLasers()
    {
        if (Lasers != null)
        {
            LaserGun compR = Lasers.GetComponent<LaserGun>();
            LaserGunBlue compB = Lasers.GetComponent<LaserGunBlue>();
            if (compB != null) compB.enabled = false;
            if (compR != null) compR.enabled = false;
        }
        if (PLaserR != null) PLaserR.SetActive(false);
        if (PLaserB != null) PLaserB.SetActive(false);

        StopParticleSystems();
    }

    private void EnableLasers()
    {
        if (Lasers != null)
        {
            LaserGun compR = Lasers.GetComponent<LaserGun>();
            LaserGunBlue compB = Lasers.GetComponent<LaserGunBlue>();
            if (compB != null) compB.enabled = true;
            if (compR != null) compR.enabled = true;
        }
        if (PLaserR != null) PLaserR.SetActive(true);
        if (PLaserB != null) PLaserB.SetActive(true);
    }

    private void StopParticleSystems()
    {
        if (PLaserR != null)
        {
            ParticleSystem psR = PLaserR.GetComponent<ParticleSystem>();
            if (psR != null) psR.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        if (PLaserB != null)
        {
            ParticleSystem psB = PLaserB.GetComponent<ParticleSystem>();
            if (psB != null) psB.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}