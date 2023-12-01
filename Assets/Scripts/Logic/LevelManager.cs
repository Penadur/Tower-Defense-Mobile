using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject continueButton;
    public int maxLevel;
    public bool mk1Researched;
    public bool mk2Researched;
    public bool mk3Researched;
    public bool mk4Researched;

    void Start()
    {
        CheckLevel();
    }
    private void CheckLevel()
    {
        if (maxLevel > 1 && continueButton != null)
        {
            continueButton.SetActive(true);
            TextMeshProUGUI textObject = continueButton.GetComponentInChildren<TextMeshProUGUI>();
            textObject.text = "Continue Level " + maxLevel;
        }
    }

    public void NewGame()
    {
        maxLevel = 1;
        SceneManager.LoadScene(1);
    }
    public void ContinueLevel()
    {
        SceneManager.LoadScene(maxLevel);
    }
    public void LoadNextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex == maxLevel)
        {
            maxLevel++;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
