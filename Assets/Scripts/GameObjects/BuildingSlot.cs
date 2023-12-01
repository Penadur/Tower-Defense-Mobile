using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class BuildingSlot : MonoBehaviour
{
    [SerializeField] private GameObject buildPanel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject anchor;
    [SerializeField] private GameObject selectionButton;
    [SerializeField] private GameObject levelText;

    [SerializeField] private GameObject MK1;
    [SerializeField] private GameObject MK2;
    [SerializeField] private GameObject MK3;
    [SerializeField] private GameObject MK4;
    [SerializeField] private GameObject HP1;

    [SerializeField] private GameObject MK1p;
    [SerializeField] private GameObject MK2p;
    [SerializeField] private GameObject MK3p;
    [SerializeField] private GameObject MK4p;
    [SerializeField] private GameObject HP1p;


    public bool placeHolderMK1 = false, placeHolderMK2 = false, placeHolderMK3 = false, placeHolderMK4 = false, 
        placeHolderHP1 = false;

    public GameObject MK1_B;
    public GameObject MK2_B;
    public GameObject MK3_B;
    public GameObject MK4_B;
    public GameObject HP1_B;

    public bool MK1isActive = false;
    public bool MK2isActive = false;
    public bool MK3isActive = false;
    public bool MK4isActive = false;
    public bool HP1isActive = false;

    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI fireRateText;
    [SerializeField] private TextMeshProUGUI upgradePriceText;
    [SerializeField] private TextMeshProUGUI sellPriceText;
    [SerializeField] private TextMeshProUGUI buildingLevelText;

    public GameObject building;
    public Turret turret;
    public Helper helper;

    private bool isBuildPanelDown = false;
    private bool isUpgradePanelDown = false;
    public bool buildingIsPresent = false;

    void Start()
    {
        anchor = GameObject.Find("BuildPanelAnchor");
    }

    // Update is called once per frame
    void Update()
    {
        if (turret != null)
        {
            buildingLevelText.text = turret.level.ToString();
        }
        if (helper != null)
        {
            buildingLevelText.text = helper.level.ToString();
        }
    }

    public void OnMouseClick()
    {
        if (buildingIsPresent)
        {
            DisablePanels();
            DisablePlaceHolders();
            DisableSelectedTiles();
            canvas.sortingOrder = 2;
            upgradePanel.SetActive(true);
            UpdateBuildingUIInfo();

            if (transform.position.y >= 4 && !isUpgradePanelDown)
            {
                upgradePanel.transform.Translate(Vector2.down * 2.5f);
                isUpgradePanelDown = true;
            }
        }
        else
        {
            DisablePanels();
            DisablePlaceHolders();
            canvas.sortingOrder = 2;
            buildPanel.SetActive(true);
            buildPanel.transform.position = anchor.transform.position;
            DisableSelectedTiles();

            Image buttonImage = selectionButton.GetComponent<Image>();
            Color buttonColor = buttonImage.color;
            buttonColor.a = 0.4f;
            buttonImage.color = buttonColor;

            if (transform.position.y >= 4 && !isBuildPanelDown)
            {
                buildPanel.transform.Translate(Vector2.down * 2.5f);
                isBuildPanelDown = true;
            }
        }
    }

    private static void DisableSelectedTiles()
    {
        GameObject[] allButtonObjects = GameObject.FindGameObjectsWithTag("SelectionButton");
        foreach (GameObject buttonObject in allButtonObjects)
        {
            Image image = buttonObject.GetComponent<Image>();
            Color color = image.color;
            color.a = 0;
            image.color = color;
        }
    }

    private void UpdateBuildingUIInfo()
    {
        if (turret != null)
        {
            damageText.text = "D: " + turret.damage[turret.level - 1];
            fireRateText.text = "FR: " + turret.fireRate[turret.level - 1];
            upgradePriceText.text = turret.upgradePrice[turret.level - 1].ToString();
            sellPriceText.text = turret.sellingPrice[turret.level - 1].ToString();
        }
        if (helper != null)
        {
            damageText.text = "D + " + helper.assistAmount[helper.level - 1] + " %";
            upgradePriceText.text = helper.upgradePrice[helper.level - 1].ToString();
            sellPriceText.text = helper.sellingPrice[helper.level - 1].ToString();
        }
    }
    private void DisablePanels()
    {
        GameObject[] allPanels = GameObject.FindGameObjectsWithTag("Panel");

        foreach (GameObject panel in allPanels)
        {
            if (panel != null)
            {
                panel.gameObject.SetActive(false);
            }
        }

        GameObject[] allCanvas = GameObject.FindGameObjectsWithTag("Canvas");

        foreach (GameObject canvas in allCanvas)
        {
            if (canvas != null)
            {
                Canvas canva = canvas.GetComponent<Canvas>();
                canva.sortingOrder = 0;
            }
        }

        GameObject[] BSs = GameObject.FindGameObjectsWithTag("BuildingSlot");

        foreach (GameObject BS in BSs)
        {
            if (BS != null)
            {
                BuildingSlot bss = BS.GetComponent<BuildingSlot>();
                bss.placeHolderMK1 = false;
                bss.placeHolderMK2 = false; 
                bss.placeHolderMK3 = false;
                bss.placeHolderMK4 = false;
                bss.placeHolderHP1 = false;
            }
        }
    }
    private void DisablePlaceHolders()
    {
        GameObject[] PlaceHolders = GameObject.FindGameObjectsWithTag("PlaceHolder");

        foreach (GameObject PlaceHolder in PlaceHolders)
        {
            if (PlaceHolder != null)
            {
                Destroy(PlaceHolder.gameObject);
            }
        }

        placeHolderMK1 = false;
        placeHolderMK2 = false;
        placeHolderMK3 = false;
        placeHolderMK4 = false;
        placeHolderHP1 = false;
    }


    public void BuildMK1(int price)
    {
        if (GameManager.Instance.money >= price && placeHolderMK1)
        {
            GameManager.Instance.timerEnabled = true;
            DisablePanels();
            DisablePlaceHolders();
            DisableSelectedTiles();
            levelText.SetActive(true);
            building = Instantiate(MK1, transform.position, Quaternion.identity);
            building.transform.SetParent(transform);
            turret = building.GetComponent<Turret>();
            buildingIsPresent = true;
            GameManager.Instance.money -= price;
            placeHolderMK1 = false;
        }

        else if (!placeHolderMK1)
        {
            DisablePlaceHolders();
            building = Instantiate(MK1p, transform.position, Quaternion.identity);
            placeHolderMK1 = true; 
        }      
    }
    public void BuildMK2(int price)
    {
        if (GameManager.Instance.money >= price && placeHolderMK2)
        {
            GameManager.Instance.timerEnabled = true;
            DisablePanels();
            DisablePlaceHolders();
            DisableSelectedTiles();
            levelText.SetActive(true);
            building = Instantiate(MK2, transform.position, Quaternion.identity);
            building.transform.SetParent(transform);
            turret = building.GetComponent<Turret>();
            buildingIsPresent = true;
            GameManager.Instance.money -= price;
            placeHolderMK2 = false;
        }

        else if (!placeHolderMK2)
        {
            DisablePlaceHolders();
            building = Instantiate(MK2p, transform.position, Quaternion.identity);
            placeHolderMK2 = true;
        }
    }

    public void BuildMK3(int price)
    {
        if (GameManager.Instance.money >= price && placeHolderMK3)
        {
            GameManager.Instance.timerEnabled = true;
            DisablePanels();
            DisablePlaceHolders();
            DisableSelectedTiles();
            levelText.SetActive(true);
            building = Instantiate(MK3, transform.position, Quaternion.identity);
            building.transform.SetParent(transform);
            turret = building.GetComponent<Turret>();
            buildingIsPresent = true;
            GameManager.Instance.money -= price;
            placeHolderMK3 = false;
        }

        else if (!placeHolderMK3)
        {
            DisablePlaceHolders();
            building = Instantiate(MK3p, transform.position, Quaternion.identity);
            placeHolderMK3 = true;
        }
    }
    public void BuildMK4(int price)
    {
        if (GameManager.Instance.money >= price && placeHolderMK4)
        {
            GameManager.Instance.timerEnabled = true;
            DisablePanels();
            DisablePlaceHolders();
            DisableSelectedTiles();
            levelText.SetActive(true);
            building = Instantiate(MK4, transform.position, Quaternion.identity);
            building.transform.SetParent(transform);
            turret = building.GetComponent<Turret>();
            buildingIsPresent = true;
            GameManager.Instance.money -= price;
            placeHolderMK4 = false;
        }

        else if (!placeHolderMK4)
        {
            DisablePlaceHolders();
            building = Instantiate(MK4p, transform.position, Quaternion.identity);
            placeHolderMK4 = true;
        }
    }

    public void BuildHP1(int price)
    {
        if (GameManager.Instance.money >= price && placeHolderHP1)
        {
            GameManager.Instance.timerEnabled = true;
            DisablePanels();
            DisablePlaceHolders();
            DisableSelectedTiles();
            levelText.SetActive(true);
            building = Instantiate(HP1, transform.position, Quaternion.identity);
            building.transform.SetParent(transform);
            helper = building.GetComponent<Helper>();
            buildingIsPresent = true;
            GameManager.Instance.money -= price;
            placeHolderHP1 = false;
        }

        else if (!placeHolderHP1)
        {
            DisablePlaceHolders();
            building = Instantiate(HP1p, transform.position, Quaternion.identity);
            placeHolderHP1 = true;
        }
    }


    public void Upgrade()
    {
        if (turret != null)
        {
            if (GameManager.Instance.money >= turret.upgradePrice[turret.level - 1] && turret.level < 5)
            {
                GameManager.Instance.money -= turret.upgradePrice[turret.level - 1];
                turret.level++;
                UpdateBuildingUIInfo();
            }
        }
        if (helper != null)
        {
            if (GameManager.Instance.money >= helper.upgradePrice[helper.level - 1] && helper.level < 5)
            {
                GameManager.Instance.money -= helper.upgradePrice[helper.level - 1];
                helper.level++;
                UpdateBuildingUIInfo();
            }
        }
    }

    public void Sell()
    {
        if (turret != null)
        {
            DisablePanels();
            DisableSelectedTiles();
            GameManager.Instance.money += turret.sellingPrice[turret.level - 1];
            Destroy(building);
            buildingIsPresent = false;
            levelText.SetActive(false);
        }

        if (helper != null)
        {
            DisablePanels();
            DisableSelectedTiles();
            GameManager.Instance.money += helper.sellingPrice[helper.level - 1];
            Destroy(building);
            buildingIsPresent = false;
            levelText.SetActive(false);
        }
    }
}
