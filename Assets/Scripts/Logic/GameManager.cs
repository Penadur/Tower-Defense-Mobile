using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using System.Linq;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{   
    public static GameManager Instance { get; private set; }
    
    //LOGIC//
    public TextAsset waveDataTextAsset;
    public List<Wave> waves;
    public Transform spawnPoint;
    private LevelManager levelManager;
    private SaveLoadData saveLoadData;

    //PANELS//
    public GameObject levelPassedPanel;
    public GameObject levelFailedPanel;

    //UI//
    public TextMeshProUGUI levelTMPro;
    public TextMeshProUGUI timeTMPro;
    public TextMeshProUGUI enemiesTMPro;
    public TextMeshProUGUI waveTMPro;
    public TextMeshProUGUI moneyTMPro;
    public TextMeshProUGUI healthTMPro;
    public TextMeshProUGUI level2TMPro;

    //VARIABLES//
    public GameObject[] buildings;
    public float spawnRate;
    public float time;
    public int level;
    public int money;
    public int enemiesSpawned;
    public int wave;
    public int health;

    //DEBUGGER//
    public bool timerEnabled = false;
    public bool globalTimer = true;
    public bool levelPassed = false;
    public bool levelPassable = true;

    private void Awake()
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

    void Start()
    {
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        saveLoadData = GameObject.Find("DataLoader").GetComponent<SaveLoadData>();
        level = SceneManager.GetActiveScene().buildIndex;
        WaveData waveData = JsonUtility.FromJson<WaveData>(waveDataTextAsset.text);
        waves = waveData.waves;
        Invoke("ActivateInventory", 0.1f);
    }

    void Update()
    {
        CheckEnemyCount();
        CheckDeck();

        levelTMPro.text = "Level: " + level;
        timeTMPro.text = "T: " + (int)time;

        Wave currentWave = waves[wave - 1];
        enemiesTMPro.text = "E: " + enemiesSpawned + "/" + currentWave.enemyQuantities.Sum();
        waveTMPro.text = "Wave: " + (wave - 1) + "/" + (waves.Count - 1);
        moneyTMPro.text = "Money: " + money;
        healthTMPro.text = "H: " + health;
        level2TMPro.text = "LEVEL " + level;

        if (globalTimer && timerEnabled && time > 0 && enemiesSpawned == 0 && wave < (waves.Count))
        {
            time = time - Time.deltaTime;
        }

        Timer();

        if (health > 0 && enemiesSpawned == 0 && wave == waves.Count)
        {
            StartCoroutine(CheckWinningCondition());
        }

        if (health <= 0)
        {
            levelFailedPanel.SetActive(true);

            GameObject[] allEnemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject obj in allEnemyObjects)
            {
                Destroy(obj);
            }
            time = 0.01f;
            wave = waves.Capacity;
        }
        
    }
    private void Timer()
    {
        if (globalTimer && timerEnabled)
        {
            if (time <= 0)
            {
                StartCoroutine(SpawnWave());
                if (enemiesSpawned == 0)
                {
                    time = 5f;
                    wave++;
                }
            }
        }
    }
    private IEnumerator CheckWinningCondition()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            if (enemiesSpawned == 0 && wave == (waves.Count) && !levelPassed && levelPassable)
            {
                levelPassedPanel.SetActive(true);
                if (levelManager.maxLevel == SceneManager.GetActiveScene().buildIndex)
                {
                    levelManager.maxLevel++;
                    saveLoadData.SaveData();
                }
                levelPassed = true;
            }
        }
    }

    private IEnumerator SpawnWave()
    {
        Wave currentWave = waves[wave];

        for (int i = 0; i < currentWave.enemyTypes.Length; i++)
        {
            for (int j = 0; j < currentWave.enemyQuantities[i]; j++)
            {
                string enemyPrefabName = "Enemies/" + currentWave.enemyTypes[i];
                GameObject enemyType = Resources.Load<GameObject>(enemyPrefabName);

                if (enemyType != null)
                {
                    SpawnEnemy(enemyType);
                    yield return new WaitForSeconds(spawnRate);
                }
                else
                {
                    Debug.LogError("Prefab not found: " + enemyPrefabName);
                }
            }
        }
    }
    private void SpawnEnemy(GameObject enemyType)
    {
        GameObject newEnemy = Instantiate(enemyType);
        newEnemy.transform.position = spawnPoint.transform.position;
    }
    private void CheckEnemyCount()
    {
        GameObject[] allEnemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        enemiesSpawned = allEnemyObjects.Length;
    }
    public void ChangeTime(float amount)
    {
        Time.timeScale = amount;
    }
    public void ResearchMK1() 
    { 
        levelManager.mk1Researched = true;
        ActivateInventory();
        saveLoadData.SaveData();
    }
    public void ResearchMK2() 
    { 
        levelManager.mk2Researched = true;
        ActivateInventory();
        saveLoadData.SaveData();
    }
    public void ResearchMK3() 
    { 
        levelManager.mk3Researched = true;
        ActivateInventory();
        saveLoadData.SaveData();
    }
    public void ResearchMK4()
    {
        levelManager.mk4Researched = true;
        ActivateInventory();
        saveLoadData.SaveData();
    }

    private void ActivateInventory()
    {
        if (levelManager.mk1Researched) buildings[0].SetActive(true);
        if (levelManager.mk2Researched) buildings[1].SetActive(true);
        if (levelManager.mk3Researched) buildings[2].SetActive(true);
        if (levelManager.mk4Researched) buildings[3].SetActive(true);
    }

    public void ActivateDeck()
    {
        GameObject deck = GameObject.Find("Deck");

        Transform[] childs = deck.GetComponentsInChildren<Transform>();

        foreach (Transform child in childs)
        {
            if (child != null)
            {
                if (child.name == "MK1d")
                {
                    GameObject[] buildingSlots = GameObject.FindGameObjectsWithTag("BuildingSlot");

                    foreach (GameObject buildingSlot in buildingSlots)
                    {
                        BuildingSlot bs = buildingSlot.GetComponent<BuildingSlot>();

                        bs.MK1_B.SetActive(true);
                    }
                }

                if (child.name == "MK2d")
                {
                    GameObject[] buildingSlots = GameObject.FindGameObjectsWithTag("BuildingSlot");

                    foreach (GameObject buildingSlot in buildingSlots)
                    {
                        BuildingSlot bs = buildingSlot.GetComponent<BuildingSlot>();

                        bs.MK2_B.SetActive(true);
                    }
                }

                if (child.name == "MK3d")
                {
                    GameObject[] buildingSlots = GameObject.FindGameObjectsWithTag("BuildingSlot");

                    foreach (GameObject buildingSlot in buildingSlots)
                    {
                        BuildingSlot bs = buildingSlot.GetComponent<BuildingSlot>();

                        bs.MK3_B.SetActive(true);
                    }
                }

                if (child.name == "MK4d")
                {
                    GameObject[] buildingSlots = GameObject.FindGameObjectsWithTag("BuildingSlot");

                    foreach (GameObject buildingSlot in buildingSlots)
                    {
                        BuildingSlot bs = buildingSlot.GetComponent<BuildingSlot>();

                        bs.MK4_B.SetActive(true);
                    }
                }
            }
        }
    }

    private void CheckDeck()
    {
        GameObject[] childs = GameObject.FindGameObjectsWithTag("DeckTag");
        if (childs.Length > 5)
        {
            if (childs[4] != null)
            {
                childs[4].gameObject.SetActive(false);
            }
        }
    }
}
