using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapesButton : MonoBehaviour
{
    public int Index;       // 0 cube, 1 pyr, 2 star, 3 sphere
    public int state;      // 0 level not enough, 1 to buy, 2 to choose, 3 chosen
    public int price;
    private int coin;

    private ShopManager SM;
    private int neededPP;
    private int prevIdx;

    private void Start()
    {
        SM = GameObject.Find("Shop Manager").GetComponent<ShopManager>();
        price = ShopManager.shapePrices[Index];
    }

    //Done with the SM first
    public void updateAll()
    {
        updateState();
        updateButton();
    }

    //Use all-time max PP instead?
    void updateState()
    {
        if (Index == ShopManager.selectedShapeIndex) { state = 3; }
        else if (ShopManager.shapeLvls[Index] != 0) { state = 2; }
        else
        {   
            int secondI = (Index + 1) % 4;
            int thirdI = (Index + 2) % 4;
            int fourthI = (Index + 3) % 4;

            int unlockedShapes = 0, maxPP = ShopManager.shapePPs[Index];
            if (ShopManager.shapeLvls[secondI] > 0) { unlockedShapes++; maxPP = ShopManager.shapePPs[secondI] > maxPP ? ShopManager.shapePPs[secondI] : maxPP; }
            if (ShopManager.shapeLvls[thirdI] > 0) { unlockedShapes++; maxPP = ShopManager.shapePPs[thirdI] > maxPP ? ShopManager.shapePPs[thirdI] : maxPP; }
            if (ShopManager.shapeLvls[fourthI] > 0) { unlockedShapes++; maxPP = ShopManager.shapePPs[fourthI] > maxPP ? ShopManager.shapePPs[fourthI] : maxPP; }

            if (unlockedShapes == 1 && maxPP >= ShapeConstants.PPRange[0]) { state = 1; }
            else if (unlockedShapes == 2 && maxPP >= ShapeConstants.PPRange[1]) { state = 1; }
            else if (unlockedShapes == 3 && maxPP >= ShapeConstants.PPRange[2]) { state = 1; }
            else { state = 0; }

            if (unlockedShapes == 1) { neededPP = ShapeConstants.PPRange[0]; }
            else if (unlockedShapes == 2) { neededPP = ShapeConstants.PPRange[1]; }
            else if (unlockedShapes == 3) { neededPP = ShapeConstants.PPRange[2]; }
        }
    }

    void updateButton()
    {
        if (state == 0)
        {
            gameObject.GetComponent<Button>().interactable = false;
            gameObject.GetComponentInChildren<Text>().text = "Need to reach " + neededPP + " prestige points";
            gameObject.GetComponentInChildren<Text>().rectTransform.anchorMax = new Vector2(0.9f, 0.9f);
            gameObject.transform.Find("Coin").GetComponent<Image>().enabled = false;
        }
        else if (state == 1)
        {
            price = ShopManager.shapePrices[Index];

            gameObject.GetComponent<Button>().interactable = true;
            gameObject.GetComponent<Image>().color = new Color(0f, 0.85f, 0f);
            Text childText = gameObject.GetComponentInChildren<Text>();
            childText.text = "Buy " + price.ToString();
            childText.color = new Color(1f, 1f, 1f);
            childText.rectTransform.anchorMax = new Vector2(0.7f, 0.9f);
            gameObject.transform.Find("Coin").GetComponent<Image>().enabled = true;
        }
        else if (state == 2)
        {
            gameObject.GetComponent<Button>().interactable = true;
            gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f);
            Text childText = gameObject.GetComponentInChildren<Text>();
            childText.text = "Choose";
            childText.color = new Color(0f, 0f, 0f);
            childText.rectTransform.anchorMax = new Vector2(0.9f, 0.9f);
            gameObject.transform.Find("Coin").GetComponent<Image>().enabled = false;
        }
        else if (state == 3)
        {
            gameObject.GetComponent<Button>().interactable = false;
            gameObject.GetComponent<Image>().color = ShapeConstants.selectedColor;
            Text childText = gameObject.GetComponentInChildren<Text>();
            childText.text = "Chosen";
            childText.color = new Color(1f, 1f, 1f);
            childText.rectTransform.anchorMax = new Vector2(0.9f, 0.9f);
            gameObject.transform.Find("Coin").GetComponent<Image>().enabled = false;
        }
    }

    //Here playerprefs is modified
    public void onClick()
    {
        if (state == 2)
        {
            ShopManager.selectedShapeIndex = Index;
            PlayerPrefs.SetInt("ShapeSelectedID", Index);
            PlayerPrefs.Save();

            SM.updateAll();
        }
        else if (state == 1)
        {
            price = ShopManager.shapePrices[Index];

            if (ShopManager.getCoins() >= price)
            {
                prevIdx = ShopManager.selectedShapeIndex;
                ShopManager.addCoins(-price);
                ShopManager.selectedShapeIndex = Index;
                PlayerPrefs.SetInt("ShapeSelectedID", Index);

                ShopManager.shapeLvls[Index] = 1;
                PlayerPrefsX.SetIntArray("Level", ShopManager.shapeLvls);

                PlayerPrefs.Save();

                bool b = true;
                try { b = SM.shapeLevelUp(); }      //In case of accumulated XP
                catch (Exception) { revertChanges(); b = true; SM.showError(); } 
                if (!b)
                {
                    SM.updateAll();
                    try { ClientTCP.PACKAGE_ChestOpening(); }
                    catch (Exception) { revertChanges(); SM.showError(); }
                }
            }
            else
            {
                SM.showWarning();
            }
        }
    }

    public void revertChanges()
    {
        ShopManager.addCoins(price);
        ShopManager.selectedShapeIndex = prevIdx;
        ShopManager.shapeLvls[Index] = 0;

        PlayerPrefs.SetInt("ShapeSelectedID", Index);
        PlayerPrefsX.SetIntArray("Level", ShopManager.shapeLvls);
        PlayerPrefs.Save();

        SM.updateAll();
    }
}