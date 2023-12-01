using System.IO;
using UnityEngine;

public class SaveLoadData : MonoBehaviour
{
    private LevelManager levelManager;
    private void Awake()
    {
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        LoadData();
    }
    public void SaveData()
    {
        GameData gameData = new GameData();
        
        gameData.maxLevel = levelManager.maxLevel;
        gameData.mk1Researched = levelManager.mk1Researched;
        gameData.mk2Researched = levelManager.mk2Researched;
        gameData.mk3Researched = levelManager.mk3Researched;
        gameData.mk4Researched = levelManager.mk3Researched;

        string json = JsonUtility.ToJson(gameData, true);

        File.WriteAllText(Application.dataPath + "/GameDataFile.json", json);
    }

    public void LoadData()
    {
        string json = File.ReadAllText(Application.dataPath + "/GameDataFile.json");

        GameData loadedManager = JsonUtility.FromJson<GameData>(json);

        levelManager.maxLevel = loadedManager.maxLevel;
        levelManager.mk1Researched = loadedManager.mk1Researched;
        levelManager.mk2Researched = loadedManager.mk2Researched;
        levelManager.mk3Researched = loadedManager.mk3Researched;
        levelManager.mk4Researched = loadedManager.mk3Researched;
    }
}
