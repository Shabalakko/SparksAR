using UnityEngine;
using System.Collections.Generic;

public class LaserManager : MonoBehaviour
{
    public static LaserManager Instance;

    public GameObject redLaserPrefab;
    public GameObject blueLaserPrefab;
    public Transform redLaserOrigin; // Punto di origine per il laser rosso
    public Transform blueLaserOrigin; // Punto di origine per il laser blu

    private Dictionary<IEnemy, GameObject> activeLasers = new Dictionary<IEnemy, GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ActivateLaser(IEnemy enemy, string color)
    {
        if (!activeLasers.ContainsKey(enemy))
        {
            GameObject laserPrefab = color == "Red" ? redLaserPrefab : blueLaserPrefab;
            Transform laserOrigin = color == "Red" ? redLaserOrigin : blueLaserOrigin;

            GameObject laserInstance = Instantiate(laserPrefab, laserOrigin.position, Quaternion.identity);
            laserInstance.transform.SetParent(laserOrigin);
            activeLasers[enemy] = laserInstance;
        }
        UpdateLaserPosition(enemy);
    }

    public void UpdateLaserPosition(IEnemy enemy)
    {
        if (activeLasers.ContainsKey(enemy))
        {
            GameObject laser = activeLasers[enemy];

            if (enemy is MonoBehaviour enemyObj)
            {
                laser.transform.position = laser.transform.parent.position;
                laser.transform.LookAt(enemyObj.transform.position);
            }
        }
    }

    public void DeactivateLaser(IEnemy enemy)
    {
        if (activeLasers.ContainsKey(enemy))
        {
            Destroy(activeLasers[enemy]);
            activeLasers.Remove(enemy);
        }
    }
}
