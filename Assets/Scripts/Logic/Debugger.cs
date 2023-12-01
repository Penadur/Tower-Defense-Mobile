using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debugger : MonoBehaviour
{
    private void Update()
    {
        
    }
    public void KillAllSpawnedEnemies()
    {
        GameObject[] allEnemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject obj in allEnemyObjects)
        {
            Destroy(obj);
        }
    }

    public void SendEnemies()
    {
        GameManager.Instance.timerEnabled = true;
        GameManager.Instance.time = 0.01f;
    }

    public void ChangeMoney(int money)
    {
        GameManager.Instance.money += money;
    }
    
    public void ChangeWave(int wave)
    {
        GameManager.Instance.wave += wave;
    }
    public void TimerEnabled()
    {
        if (GameManager.Instance.globalTimer) GameManager.Instance.globalTimer = false;
        else GameManager.Instance.globalTimer = true;
    }

    public void LevelPassable()
    {
        if (GameManager.Instance.levelPassable) GameManager.Instance.levelPassable = false;
        else GameManager.Instance.levelPassable = true;
    }

    public void FinishLevel()
    {
        KillAllSpawnedEnemies();
        GameManager.Instance.timerEnabled = true;
        GameManager.Instance.time = 0.01f;

        GameManager.Instance.wave = GameManager.Instance.waves.Capacity;
        GameManager.Instance.levelPassed = false;
        GameManager.Instance.levelPassable = true;
    }
}
