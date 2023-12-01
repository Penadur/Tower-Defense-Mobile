using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    private LevelManager levelManager;
    public int level;

    private void Start()
    {
        StartCoroutine(FindLevelManager());
    }
    public void SelectLevel()
    {
        if (level <= levelManager.maxLevel)
        {
            SceneManager.LoadScene(level);
        }

    }

    private IEnumerator FindLevelManager()
    {
        yield return new WaitForSeconds(0.1f);
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        if (level > levelManager.maxLevel)
        {
            Image image = gameObject.GetComponent<Image>();
            image.color = Color.grey;
        }
    }
}
