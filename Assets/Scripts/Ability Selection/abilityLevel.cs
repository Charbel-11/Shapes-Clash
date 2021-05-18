using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class abilityLevel : MonoBehaviour {

    public int level;
    private int ID;
    private AbilityToSelect friend;
    private Image im;
    private bool inShop;

    private void Start()
    {
        inShop = this.transform.root.name == "Shop";

        if (inShop)
        {
            ID = GetComponentInParent<Ability>().ID;
            if (GetComponentInParent<Ability>().passive) { level = ShopManager.PassivesLevel[ID]; }
            else if (ID < 100) { Int32.TryParse(ShopManager.AbLevelArray[ID], out level); }
            else if (ID < 200) { Int32.TryParse(ShopManager.Super100[ID - 100 - 1], out level); }
            else if (ID < 300) { Int32.TryParse(ShopManager.Super200[ID - 200 - 1], out level); }
        }
        else
        {
            friend = transform.parent.GetComponentInChildren<AbilityToSelect>();
            ID = friend.ID;
            
            Int32.TryParse(SelectionManager.AbLevelArray[ID], out level);
        }

        updateLevel(level);
        updateTransparency();
    }

    public void updateTransparency()
    {
        if (friend == null) { friend = transform.parent.GetComponentInChildren<AbilityToSelect>(); }

        if (!inShop && (friend.GetAlreadySelected() == true || level == 0))
        {
            im = GetComponent<Image>();
            var temp = im.color;
            temp.a = 0.3f;
            im.color = temp;

            foreach (Transform child in transform)
            {
                im = child.GetComponent<Image>();
                temp = im.color;
                temp.a = 0.5f;
                im.color = temp;
            }
        }
        else
        {
            im = GetComponent<Image>();
            var temp = im.color;
            temp.a = 1f;
            im.color = temp;

            foreach (Transform child in transform)
            {
                im = child.GetComponent<Image>();
                temp = im.color;
                temp.a = 1f;
                im.color = temp;
            }
        }
    }

    public void updateColor()
    {
        int curIdx;
        if (this.transform.root.name == "Shop") { curIdx = ShopManager.selectedShapeIndex; }
        else {
            curIdx = GameObject.Find("SelectionManager").GetComponent<SelectionManager>().finalShapeIndex;
            Int32.TryParse(SelectionManager.AbLevelArray[ID], out level);   //surely ID<100 here
        }

        Color tempC = (level == 3) ? ShapeConstants.levelMaxColors[curIdx] : ShapeConstants.levelColors[curIdx];
        for (int i = 0; i < level; i++)
            transform.GetChild(i).GetComponent<Image>().color = tempC;
        for (int i = level; i < 3; i++)
            transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1);
    }

    public void updateLevel(int newL)
    {
        level = newL;
        updateColor(); 
    }

    public void inShopHighlight()
    {
        if (level == 3) { return; }     //Maxed level
       
        transform.GetChild(level).GetComponent<Image>().color = new Color(0f, 1f, 0f);
    }

    public void stopInShopHighlight()
    {
        if (level == 3) { return; }     //Maxed level

        transform.GetChild(level).GetComponent<Image>().color = new Color(1f, 1f, 1f);
    }
}
