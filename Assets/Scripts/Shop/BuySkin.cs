﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuySkin : MonoBehaviour {
    public int Index;

    private int state;      // 0 level not enough, 1 to buy, 2 to choose, 3 chosen
    private int price;

    private ShopManager SM;

    private void Start()
    {
        SM = GameObject.Find("Shop Manager").GetComponent<ShopManager>();
        price = ShopManager.SkinPrices[Index];
    }

    void updateState()
    {
        if (Index == ShopManager.selectedSkin) { state = 3; }
        else if (ShopManager.unlockedSkins[Index] == 1) { state = 2; }
        else
        {
            state = 1;  //May be modified if we want to unlock skins at a certain level
        }
    }

    void updateButton()
    {
        if (state == 0)
        {
            gameObject.GetComponent<Button>().interactable = false;

            gameObject.transform.Find("Text").gameObject.SetActive(true);
            gameObject.transform.Find("BText").gameObject.SetActive(false);
            gameObject.transform.Find("Price").gameObject.SetActive(false);
            gameObject.transform.Find("Image").gameObject.SetActive(false);

            gameObject.transform.Find("Text").GetComponent<Text>().text = "Level Not High Enough";
        }
        else if (state == 1)
        {
            price = ShopManager.BckgdPrices[Index];

            gameObject.transform.Find("Text").gameObject.SetActive(false);
            gameObject.transform.Find("BText").gameObject.SetActive(true);
            gameObject.transform.Find("Price").gameObject.SetActive(true);
            gameObject.transform.Find("Image").gameObject.SetActive(true);

            gameObject.GetComponent<Button>().interactable = true;
            gameObject.GetComponent<Image>().color = new Color(0f, 0.85f, 0f);
            gameObject.transform.Find("Price").GetComponent<Text>().text = price.ToString();
            gameObject.transform.Find("BText").GetComponent<Text>().color = new Color(1f, 1f, 1f);
            gameObject.transform.Find("Price").GetComponent<Text>().color = new Color(1f, 1f, 1f);
        }
        else if (state == 2)
        {
            gameObject.GetComponent<Button>().interactable = true;

            gameObject.transform.Find("Text").gameObject.SetActive(true);
            gameObject.transform.Find("BText").gameObject.SetActive(false);
            gameObject.transform.Find("Price").gameObject.SetActive(false);
            gameObject.transform.Find("Image").gameObject.SetActive(false);

            gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f);
            gameObject.transform.Find("Text").GetComponent<Text>().text = "Select";
            gameObject.transform.Find("Text").GetComponent<Text>().color = new Color(0f, 0f, 0f);
        }
        else if (state == 3)
        {
            gameObject.GetComponent<Button>().interactable = false;

            gameObject.transform.Find("Text").gameObject.SetActive(true);
            gameObject.transform.Find("BText").gameObject.SetActive(false);
            gameObject.transform.Find("Price").gameObject.SetActive(false);
            gameObject.transform.Find("Image").gameObject.SetActive(false);

            gameObject.GetComponent<Image>().color = ShapeConstants.selectedColor;
            gameObject.transform.Find("Text").GetComponent<Text>().text = "Selected";
            gameObject.transform.Find("Text").GetComponent<Text>().color = new Color(1f, 1f, 1f);
        }
    }

    //Done with the SM first
    public void updateAll()
    {
        updateState();
        updateButton();
    }

    //Here playerprefs is modified
    public void onClick()
    {
        if (state == 2)
        {
            ShopManager.selectedSkin = Index;
            PlayerPrefs.SetInt("SkinID", Index);
            PlayerPrefs.Save();

            SM.updateAll();
        }
        else if (state == 1)
        {
            price = ShopManager.SkinPrices[Index];

            if (ShopManager.getCoins() >= price)
            {
                int prevSkin = ShopManager.selectedSkin;

                ShopManager.addCoins(-price);
                ShopManager.selectedSkin = Index;
                PlayerPrefs.SetInt("SkinID", Index);

                ShopManager.unlockedSkins[Index] = 1;
                PlayerPrefsX.SetIntArray("SkinsUnlockedAr", ShopManager.unlockedSkins);

                PlayerPrefs.Save();

                try
                {
                    ClientTCP.PACKAGE_ChestOpening();
                }
                catch(Exception)
                {
                    ShopManager.addCoins(price);
                    ShopManager.selectedSkin = prevSkin;
                    ShopManager.unlockedSkins[Index] = 0;
                    PlayerPrefs.SetInt("SkinID", prevSkin);
                    PlayerPrefsX.SetIntArray("SkinsUnlockedAr", ShopManager.unlockedSkins);
                    PlayerPrefs.Save();

                    SM.showError();
                }
                SM.updateAll();
            }
            else
            {
                SM.showWarning();
            }
        }
    }
}
