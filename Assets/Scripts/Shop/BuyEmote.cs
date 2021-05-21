using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyEmote : MonoBehaviour
{
    public int ID;
    private int coins;
    private int diamonds;
    private Button but;

    private ShopManager SM;

    private void Start()
    {
        SM = GameObject.Find("Shop Manager").GetComponent<ShopManager>();

        but = gameObject.GetComponent<Button>();
        but.onClick.AddListener(buy);
    }

    //First 4 emotes cost coins, the other cost diamonds
    void buy()
    {
        if (ID < 4) { coins = ShopManager.EmotesPrice[ID]; diamonds = 0; }
        else { diamonds = ShopManager.EmotesPrice[ID]; coins = 0; }

        if (ShopManager.getCoins() >= coins && ShopManager.getDiamonds() >= diamonds)
        {
            ShopManager.addCoins(-coins); ShopManager.addDiamonds(-diamonds);
            ShopManager.EmotesUnlock[ID] = 1;

            PlayerPrefsX.SetIntArray("EmotesUnlockedAr", ShopManager.EmotesUnlock);
            PlayerPrefs.Save();

            updateText(ID);
            SM.goOfSelectedEmote.GetComponent<EmoteBut>().updateState();

            if (SM.goOfSelectedEmote.GetComponent<EmoteBut>().common)
            {
                ShopManager.shapeExp[0] += 50;
                ShopManager.shapeExp[1] += 50;
                ShopManager.shapeExp[2] += 50;
                ShopManager.shapeExp[3] += 50;
            }
            else
            {
                ShopManager.shapeExp[ShopManager.selectedShapeIndex] += 50;
            }

            bool b = true;
            try { b = SM.shapeLevelUp(); }
            catch(Exception){ revertChanges(); b = true; }
            if (!b)
            {
                try { ClientTCP.PACKAGE_ChestOpening(); }
                catch(Exception) { revertChanges(); return; }
                PlayerPrefsX.SetIntArray("XP", ShopManager.shapeExp);
                PlayerPrefs.Save();
            }
        }
        else
        {
            SM.showWarning();
        }
    }

    public void revertChanges()
    {
        if (SM.goOfSelectedEmote.GetComponent<EmoteBut>().common)
        {
            ShopManager.shapeExp[0] -= 50;
            ShopManager.shapeExp[1] -= 50;
            ShopManager.shapeExp[2] -= 50;
            ShopManager.shapeExp[3] -= 50;
        }
        else
        {
            ShopManager.shapeExp[ShopManager.selectedShapeIndex] -= 50;
        }
        ShopManager.addCoins(coins); ShopManager.addDiamonds(diamonds);
        ShopManager.EmotesUnlock[ID] = 0;

        PlayerPrefsX.SetIntArray("XP", ShopManager.shapeExp);
        PlayerPrefsX.SetIntArray("EmotesUnlockedAr", ShopManager.EmotesUnlock);
        PlayerPrefs.Save();
        updateText(ID);
        SM.goOfSelectedEmote.GetComponent<EmoteBut>().updateState();
    }

    public void updateText(int newID)
    {
        ID = newID;
        if (ShopManager.EmotesUnlock[ID] == 1)
        {
            gameObject.transform.Find("BText").gameObject.SetActive(false);
            gameObject.transform.Find("Buy").gameObject.SetActive(false);
            gameObject.transform.Find("ImageD").gameObject.SetActive(false);
            gameObject.transform.Find("ImageC").gameObject.SetActive(false);
            gameObject.transform.Find("Bought").gameObject.SetActive(true);

            return;
        }

        if (ID < 4) { coins = ShopManager.EmotesPrice[ID]; diamonds = 0; }
        else { diamonds = ShopManager.EmotesPrice[ID]; coins = 0; }

        gameObject.transform.Find("BText").gameObject.SetActive(true);
        gameObject.transform.Find("Buy").gameObject.SetActive(true);
        gameObject.transform.Find("ImageD").gameObject.SetActive(ID >= 4);
        gameObject.transform.Find("ImageC").gameObject.SetActive(ID < 4);
        gameObject.transform.Find("Bought").gameObject.SetActive(false);

        gameObject.transform.Find("Buy").GetComponent<Text>().text = ShopManager.EmotesPrice[ID].ToString();
        gameObject.transform.Find("Buy").GetComponent<Text>().color = (ID >= 4 ? new Color(0.36f, 0f, 0.462f) : new Color(1f, 1f, 0f));
    }
}
