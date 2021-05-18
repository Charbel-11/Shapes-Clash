using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyEmote : MonoBehaviour
{
    public int ID;
    public int coins;
    public int diamonds;

    private int curCoins;
    private int curDiamonds;

    private ShopManager SM;

    private void Start()
    {
        SM = GameObject.Find("Shop Manager").GetComponent<ShopManager>();
    }

    void buy()
    {
        coins = 0;
        diamonds = ShopManager.EmotesPrice[ID];

        curCoins = ShopManager.coins;
        curDiamonds = ShopManager.diamonds;

        if (curCoins >= coins && curDiamonds >= diamonds)
        {
            curCoins -= coins; curDiamonds -= diamonds;
            ShopManager.coins = curCoins;
            ShopManager.diamonds = curDiamonds;
            ShopManager.EmotesUnlock[ID] = 1;

            PlayerPrefs.SetInt("Gold", curCoins);
            PlayerPrefs.SetInt("Diamonds", curDiamonds);
            PlayerPrefsX.SetIntArray("EmotesUnlockedAr", ShopManager.EmotesUnlock);
            PlayerPrefs.Save();

            updateText();
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
        curCoins += coins; curDiamonds += diamonds;
        ShopManager.EmotesUnlock[ID] = 0;

        PlayerPrefs.SetInt("Gold", curCoins);
        PlayerPrefs.SetInt("Diamonds", curDiamonds);
        PlayerPrefsX.SetIntArray("XP", ShopManager.shapeExp);
        PlayerPrefsX.SetIntArray("EmotesUnlockedAr", ShopManager.EmotesUnlock);
        PlayerPrefs.Save();
        updateText();
        SM.goOfSelectedEmote.GetComponent<EmoteBut>().updateState();
    }

    public void updateText()
    {
        coins = 0;
        diamonds = ShopManager.EmotesPrice[ID];

        if (ShopManager.EmotesUnlock[ID] == 1)
        {
            gameObject.transform.Find("BText").gameObject.SetActive(false);
            gameObject.transform.Find("Buy").gameObject.SetActive(false);
            gameObject.transform.Find("Image").gameObject.SetActive(false);
            gameObject.transform.Find("Bought").gameObject.SetActive(true);

            return;
        }

        gameObject.transform.Find("BText").gameObject.SetActive(true);
        gameObject.transform.Find("Buy").gameObject.SetActive(true);
        gameObject.transform.Find("Image").gameObject.SetActive(true);
        gameObject.transform.Find("Bought").gameObject.SetActive(false);

        gameObject.transform.Find("Buy").GetComponent<Text>().text = diamonds.ToString();
    }
}
