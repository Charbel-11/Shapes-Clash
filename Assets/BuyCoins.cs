using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyCoins : MonoBehaviour
{
    //Set in ShopManager
    public int price;       
    public int reward;      

    private int curDiamonds, curCoins;
    private ShopManager SM;

    public void onClick()
    {
        SM = GameObject.Find("Shop Manager").GetComponent<ShopManager>();
        curDiamonds = ShopManager.diamonds;
        curCoins = ShopManager.coins;

        if (curDiamonds >= price)
        {
            curDiamonds -= price;
            curCoins += reward;

            PlayerPrefs.SetInt("Diamonds", curDiamonds);
            PlayerPrefs.SetInt("Gold", curCoins);
            PlayerPrefs.Save();

            try { ClientTCP.PACKAGE_AdReward(-1); }
            catch (Exception)
            {
                curDiamonds += price;
                curCoins -= reward;
                PlayerPrefs.SetInt("Diamonds", curDiamonds);
                PlayerPrefs.SetInt("Gold", curCoins);
                PlayerPrefs.Save();
                SM.showError();
                return;
            }

            SM.updateBotBar();
        }
    }
}
