using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyBackground : MonoBehaviour {
    public int Index;  
    
    private int state;      // 0 level not enough, 1 to buy, 2 to choose, 3 chosen
    private int price;
    private int coin;

    private ShopManager SM;

    private void Start()
    {
        SM = GameObject.Find("Shop Manager").GetComponent<ShopManager>();
        price = ShopManager.BckgdPrices[Index];
        coin = ShopManager.coins;
    }

    void updateState()
    {
        if (Index == ShopManager.selectedBckgd) { state = 3; }
        else if (ShopManager.unlockedBckgd[Index] == 1) { state = 2; }
        else
        {
            state = 1;  //May be modified if we want to unlock bckgrds at a certain level
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
            ShopManager.selectedBckgd = Index;
            PlayerPrefs.SetInt("BckgdID", Index);
            PlayerPrefs.Save();

            SM.updateAll();
        }
        else if (state == 1)
        {
            price = ShopManager.BckgdPrices[Index];
            coin = ShopManager.coins;

            if (coin >= price)
            {
                int prevBck = ShopManager.selectedBckgd;

                coin -= price;
                PlayerPrefs.SetInt("Gold", coin);
                ShopManager.selectedBckgd = Index;
                PlayerPrefs.SetInt("BckgdID", Index);

                ShopManager.unlockedBckgd[Index] = 1;
                PlayerPrefsX.SetIntArray("Backgrounds", ShopManager.unlockedBckgd);

                PlayerPrefs.Save();
                SM.updateAll();

                try
                {
                    ClientTCP.PACKAGE_ChestOpening();
                }
                catch (Exception)
                {
                    coin += price;
                    ShopManager.selectedBckgd = prevBck;
                    ShopManager.unlockedBckgd[Index] = 0;
                    PlayerPrefs.SetInt("Gold", coin);
                    PlayerPrefs.SetInt("BckgdID", prevBck);
                    PlayerPrefsX.SetIntArray("Backgrounds", ShopManager.unlockedBckgd);
                    PlayerPrefs.Save();

                    SM.updateAll();
                    SM.showError();
                }
            }
            else
            {
                SM.showWarning();
            }
        }
    }
}