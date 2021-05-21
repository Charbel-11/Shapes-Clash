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
    private ShopManager SM;

    public void onClick()
    {
        SM = GameObject.Find("Shop Manager").GetComponent<ShopManager>();

        if (ShopManager.getDiamonds() >= price)
        {
            ShopManager.addCoins(reward);
            ShopManager.addDiamonds(-price);
            ShopManager.saveChanges();

            try { ClientTCP.PACKAGE_AdReward(-1); }
            catch (Exception)
            {
                ShopManager.addCoins(-reward);
                ShopManager.addDiamonds(+price);
                ShopManager.saveChanges();
                SM.showError();
                return;
            }
        }
    }
}
