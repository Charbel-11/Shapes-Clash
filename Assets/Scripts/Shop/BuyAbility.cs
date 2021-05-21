using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BuyAbility : MonoBehaviour
{
    public bool direct3;
    private bool passive;
    private int ID;
    private int level;
    private int coinCost;
    private int redBoltCost;
    private Button but;

    private int curCoins;
    private int curRedBolts;
    private int curXP;
    private ShopManager SM;

    private void Start()
    {
        but = GetComponent<Button>();
        but.onClick.AddListener(onClick);
    }

    public void updateCost(bool b = false)
    {
        direct3 = b;
        SM = GameObject.Find("Shop Manager").GetComponent<ShopManager>();
        but = GetComponent<Button>();

        bool levelEnough;
        int levelNeeded;

        passive = SM.goOfSelectedAbBut.GetComponentInParent<Ability>().passive;
        ID = SM.goOfSelectedAbBut.GetComponentInParent<Ability>().ID;
        levelEnough = SM.goOfSelectedAbBut.GetComponentInParent<Ability>().levelEnough;
        levelNeeded = SM.goOfSelectedAbBut.GetComponentInParent<Ability>().levelNeeded;

        if (!levelEnough)
        {
            gameObject.transform.Find("Costs").gameObject.SetActive(false);
            gameObject.transform.Find("Text").GetComponent<Text>().text = "Level Needed: " + levelNeeded.ToString();
            gameObject.transform.Find("Text").GetComponent<Text>().color = new Color(1f, 1f, 1f);
            gameObject.GetComponent<Image>().color = new Color(.5f, .5f, .5f);
            gameObject.transform.Find("Text").GetComponent<RectTransform>().anchorMax = new Vector2(0.9f, 0.8f);
            but.interactable = false;

            return;
        }

        if (passive)
            level = ShopManager.PassivesLevel[ID];
        else if (ID < 100)
            Int32.TryParse(ShopManager.AbLevelArray[ID], out level);
        else if (ID < 200)
            Int32.TryParse(ShopManager.Super100[ID - 101], out level);
        else if (ID < 300)
            Int32.TryParse(ShopManager.Super200[ID - 201], out level);

        if (level == 3)
        {
            gameObject.transform.Find("Costs").gameObject.SetActive(false);
            gameObject.transform.Find("Text").GetComponent<Text>().text = "MAXED";
            gameObject.transform.Find("Text").GetComponent<Text>().color = new Color(1f, 1f, 1f);
            gameObject.GetComponent<Image>().color = new Color(1f, 0f, 0f);
            gameObject.transform.Find("Text").GetComponent<RectTransform>().anchorMax = new Vector2(0.9f, 0.8f);
            but.interactable = false;

            return;
        }

        gameObject.transform.Find("Costs").gameObject.SetActive(true);
        gameObject.transform.Find("Text").GetComponent<Text>().text = "BUY";
        gameObject.transform.Find("Text").GetComponent<Text>().color = new Color(1f, 1f, 1f);
        gameObject.GetComponent<Image>().color = new Color(0f, .53f, .71f);
        gameObject.transform.Find("Text").GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.8f);
        but.interactable = true;

        if (passive)
        {
            coinCost = ShopManager.PassivesShop[ID][level][0];
            redBoltCost = ShopManager.PassivesShop[ID][level][1];
        }
        else if (ID < 100)
        {
            coinCost = ShopManager.ShopArray[ID][level][0];
            redBoltCost = ShopManager.ShopArray[ID][level][1];
        }
        else if (ID < 200)
        {
            coinCost = ShopManager.Super100Shop[ID - 101][level][0];
            redBoltCost = ShopManager.Super100Shop[ID - 101][level][1];
        }
        else
        {
            coinCost = ShopManager.Super200Shop[ID - 201][level][0];
            redBoltCost = ShopManager.Super200Shop[ID - 201][level][1];
        }

        gameObject.transform.Find("Costs").transform.Find("CoinsNum").GetComponent<Text>().text = coinCost.ToString();
        gameObject.transform.Find("Costs").transform.Find("RedBoltNum").GetComponent<Text>().text = redBoltCost.ToString();
    }

    private void onClick()
    {
        passive = SM.goOfSelectedAbBut.GetComponentInParent<Ability>().passive;

        if (ShopManager.getCoins() >= coinCost && ShopManager.getRedBolts() >= redBoltCost && level < 3)
        {
            ShopManager.addCoins(-coinCost);
            ShopManager.addRedBolts(-redBoltCost);
            if (direct3) { level = 3; }
            else { level++; }

            int toAdd = 0;
            if (level == 1) { toAdd = 50; }
            else if (level == 2) { toAdd = 50; }
            else if (level == 3) { toAdd = 100; }

            if (SM.goOfSelectedAbBut.GetComponentInParent<Ability>().common)
            {
                ShopManager.shapeExp[0] += toAdd;
                ShopManager.shapeExp[1] += toAdd;
                ShopManager.shapeExp[2] += toAdd;
                ShopManager.shapeExp[3] += toAdd;
            }
            else
            {
                ShopManager.shapeExp[ShopManager.selectedShapeIndex] += toAdd;
            }

            PlayerPrefsX.SetIntArray("XP", ShopManager.shapeExp);

            if (passive)
            {
                ShopManager.PassivesLevel[ID] = level;
                PlayerPrefsX.SetIntArray("PassivesArray", ShopManager.PassivesLevel);
            }
            else if (ID < 100)
            {
                ShopManager.AbLevelArray[ID] = level.ToString();
                PlayerPrefsX.SetStringArray("AbilitiesArray", ShopManager.AbLevelArray);
            }
            else if (ID < 200)
            {
                ShopManager.Super100[ID - 101] = level.ToString();
                PlayerPrefsX.SetStringArray("Super100Array", ShopManager.Super100);
            }
            else if (ID < 300)
            {
                ShopManager.Super200[ID - 201] = level.ToString();
                PlayerPrefsX.SetStringArray("Super200Array", ShopManager.Super200);
            }

            PlayerPrefs.Save();

            Button temp = SM.goOfSelectedAbBut;

            bool b = true;
            try { b = SM.shapeLevelUp(); }
            catch (Exception) {
                revertChanges();
                SM.showError();
                b = true;
                return;
            }
            if (!b)
            {
                try { ClientTCP.PACKAGE_ChestOpening(); }
                catch (Exception) {
                    revertChanges();
                    SM.showError();
                    return;
                }
            }

            SM.goOfSelectedAbBut = temp;
            SM.levelUp(level, passive);
            updateCost(direct3);
        }
        else
        {
            SM.showWarning();
        }
    }

    public void revertChanges()
    {
        ShopManager.addCoins(coinCost);
        ShopManager.addRedBolts(redBoltCost);

        int toAdd = 0;
        if (level == 1) { toAdd = 50; }
        else if (level == 2) { toAdd = 50; }
        else if (level == 3) { toAdd = 100; }

        if (direct3) { level = 0; }
        else{ level--;}

        if (SM.goOfSelectedAbBut.GetComponentInParent<Ability>().common)
        {
            ShopManager.shapeExp[0] -= toAdd;
            ShopManager.shapeExp[1] -= toAdd;
            ShopManager.shapeExp[2] -= toAdd;
            ShopManager.shapeExp[3] -= toAdd;
        }
        else
        {
            ShopManager.shapeExp[ShopManager.selectedShapeIndex] -= toAdd;
        }
        PlayerPrefsX.SetIntArray("XP", ShopManager.shapeExp);

        if (passive)
        {
            ShopManager.PassivesLevel[ID] = level;
            PlayerPrefsX.SetIntArray("PassivesArray", ShopManager.PassivesLevel);
        }
        else if (ID < 100)
        {
            ShopManager.AbLevelArray[ID] = level.ToString();
            PlayerPrefsX.SetStringArray("AbilitiesArray", ShopManager.AbLevelArray);
        }
        else if (ID < 200)
        {
            ShopManager.Super100[ID - 101] = level.ToString();
            PlayerPrefsX.SetStringArray("Super100Array", ShopManager.Super100);
        }
        else if (ID < 300)
        {
            ShopManager.Super200[ID - 201] = level.ToString();
            PlayerPrefsX.SetStringArray("Super200Array", ShopManager.Super200);
        }

        PlayerPrefs.Save();
    }
}
