using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Ability : MonoBehaviour
{
    public int ID;
    public bool passive;
    public bool common;
    public bool direct3;

    public bool levelEnough;
    public int levelNeeded;
    private string Text;

    private Button but;
    private Button[] buts;
    private ShopManager SM;

    private int level;
    private int coinCost;
    private int redBoltCost;

    private bool init = false;

    private void Awake()
    {
        buts = gameObject.GetComponentsInChildren<Button>();
        foreach (Button b in buts)
            b.onClick.AddListener(TaskOnClick);
    }

    private void Start()
    {
        if (init) { return; }
        SM = GameObject.Find("Shop Manager").GetComponent<ShopManager>();
        
        gameObject.transform.Find("Panel").gameObject.SetActive(true);
        gameObject.transform.Find("Panel2").gameObject.SetActive(true);
        gameObject.transform.Find("Maxed").gameObject.SetActive(true);

        if (!passive)
            but = transform.Find("Button8").GetComponent<Button>();
        else
            but = transform.Find("Button8").GetChild(0).GetComponent<Button>();

        init = true;
    }

    void TaskOnClick()
    {
        if (!SM.canChange) { return; }

        but.gameObject.transform.GetComponent<Image>().color = new Color(0f, 1f, 0f);
        but.gameObject.transform.GetComponent<RectTransform>().offsetMax = new Vector2(0f, 10f);
        but.gameObject.transform.GetComponent<RectTransform>().offsetMin = new Vector2(-2.5f, 0f);
        but.interactable = false;

        buts = gameObject.GetComponentsInChildren<Button>();
        foreach (Button b in buts)
        {
            b.interactable = false;
        }

        gameObject.GetComponentInChildren<abilityLevel>().inShopHighlight();

        if (SM.goOfSelectedAbBut != but) SM.resetSelected(false);
        SM.goOfSelectedAbBut = but;
        if (passive) SM.openPassive(ID);
        else SM.openInfo(ID, direct3);
    }

    public void mimicClick()
    {
        TaskOnClick();
    }

    public void updatePanels()
    {
        if (!init) { Start(); }
        updateLevel();

        if (levelNeeded > ShopManager.shapeLvls[ShopManager.selectedShapeIndex] || levelNeeded == -1)
        {
            gameObject.transform.Find("Panel").gameObject.SetActive(false);
            gameObject.transform.Find("Panel2").gameObject.SetActive(false);

            gameObject.transform.Find("Maxed").gameObject.SetActive(true);
            gameObject.transform.Find("Maxed").GetComponent<Image>().color = new Color(0.5f, .5f, .5f);
            if (levelNeeded == -1)
                gameObject.transform.Find("Maxed").GetComponentInChildren<Text>().text = "Locked";
            else
                gameObject.transform.Find("Maxed").GetComponentInChildren<Text>().text = "Level Needed: " + levelNeeded.ToString();

            levelEnough = false;
            return;
        }

        levelEnough = true;

        if (level == 3)
        {
            gameObject.transform.Find("Panel").gameObject.SetActive(false);
            gameObject.transform.Find("Panel2").gameObject.SetActive(false);

            gameObject.transform.Find("Maxed").gameObject.SetActive(true);
            gameObject.transform.Find("Maxed").GetComponent<Image>().color = new Color(1f, 0f, 0f);
            gameObject.transform.Find("Maxed").GetComponentInChildren<Text>().text = "Maxed";
            return;
        }
        else
        {
            gameObject.transform.Find("Panel").gameObject.SetActive(true);
            gameObject.transform.Find("Panel2").gameObject.SetActive(true);
            gameObject.transform.Find("Maxed").gameObject.SetActive(false);
        }

        if (passive)
        {
            coinCost = ShopManager.PassivesShop[ID][level][0];
            redBoltCost = ShopManager.PassivesShop[ID][level][1];
        }
        else
        {
            if (ID < 100)
            {
                coinCost = ShopManager.ShopArray[ID][level][0];
                redBoltCost = ShopManager.ShopArray[ID][level][1];
            }
            else if (ID < 200)
            {
                coinCost = ShopManager.Super100Shop[ID - 101][level][0];
                redBoltCost = ShopManager.Super100Shop[ID - 101][level][1];
            }
            else if (ID < 300)
            {
                coinCost = ShopManager.Super200Shop[ID - 201][level][0];
                redBoltCost = ShopManager.Super200Shop[ID - 201][level][1];
            }
        }

        gameObject.transform.Find("Panel").GetComponentInChildren<Text>().text = coinCost.ToString();
        gameObject.transform.Find("Panel2").GetComponentInChildren<Text>().text = redBoltCost.ToString();
    }

    public void updateButton()
    {
        if (!init) { Start(); }

        if (!passive)
            but = transform.Find("Button8").GetComponent<Button>();
        else
            but = transform.Find("Button8").GetChild(0).GetComponent<Button>();

        updateLevel();

        if (common && !passive)
        {
            for (int i = 0; i < 4; i++)
                but.transform.GetChild(i).gameObject.SetActive(ShopManager.selectedShapeIndex == i);
        }

        if (level == 0 && (levelNeeded > ShopManager.shapeLvls[ShopManager.selectedShapeIndex] || levelNeeded == -1))
        {
            but.transform.Find("Lock").GetComponent<Image>().enabled = true;
            but.interactable = false;
        }
        else
        {
            but.transform.Find("Lock").GetComponent<Image>().enabled = false;
            but.interactable = true;
        }

        if (passive)
            but.transform.parent.Find("Percentage").GetChild(0).GetComponent<Text>().text = ShopManager.PassiveStatsArr[ID][3 + (level == 0 ? 0 : level - 1)].ToString() + "%";
    }

    public void setOrder()
    {
        if (level == 0 && (levelNeeded > ShopManager.shapeLvls[ShopManager.selectedShapeIndex] || levelNeeded == -1))
            transform.SetAsLastSibling();
    }

    void updateLevel()
    {
        if (passive)
        {
            level = ShopManager.PassivesLevel[ID];
            if (level == 3) { levelNeeded = 0; return; }
            levelNeeded = ShopManager.PassivesShop[ID][level][2];
            return;
        }
        if (ID < 100)
        {
            Int32.TryParse(ShopManager.AbLevelArray[ID], out level);
            if (level == 3) { levelNeeded = 0; return; }
            levelNeeded = ShopManager.ShopArray[ID][level][2];
        }
        else if (ID < 200)
        {
            Int32.TryParse(ShopManager.Super100[ID - 101], out level);
            if (level == 3) { levelNeeded = 0; return; }
            levelNeeded = ShopManager.Super100Shop[ID - 101][level][2];
        }
        else if (ID < 300)
        {
            Int32.TryParse(ShopManager.Super200[ID - 201], out level);
            if (level == 3) { levelNeeded = 0; return; }
            levelNeeded = ShopManager.Super200Shop[ID - 201][level][2];
        }
    }
}