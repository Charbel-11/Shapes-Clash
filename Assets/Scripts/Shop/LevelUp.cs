using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUp : MonoBehaviour {
    public int curIdx;

    ShopManager SM;
    GameMaster GM;

    //Only displays values, doesn't change player prefs or server values
    public void UpdateInfo()
    {
        int curLevel = 0;
        Sprite curShape = null;
        int gainedHP = 0, gainedAtck = 0, gainedDef = 0;
        int gainedCoins = 0, gainedDiamonds = 0;

        //curLvl-1 represents the entry in levelStats for curLvl, it has add atck/def/health as well as needed xp to levelup
        if (transform.root.name == "Shop") {
            SM = GameObject.Find("Shop Manager").GetComponent<ShopManager>();
            curLevel = ShopManager.shapeLvls[curIdx];
            curShape = SM.ShapeXP[curIdx];

            gainedAtck = ShopManager.levelStats[curLevel - 1][curIdx][0];
            gainedDef = ShopManager.levelStats[curLevel - 1][curIdx][1];
            gainedHP = ShopManager.levelStats[curLevel - 1][curIdx][2]; 

            gainedCoins = ShopManager.levelStats[curLevel - 1][4][1];
            gainedDiamonds = ShopManager.levelStats[curLevel - 1][4][2];
        }
        else
        {
            GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
            curLevel = PlayerPrefsX.GetIntArray("Level")[curIdx];
            curShape = GM.ShapeXP[curIdx];

            gainedAtck = GameMaster.levelStats[curLevel - 1][curIdx][0];
            gainedDef = GameMaster.levelStats[curLevel - 1][curIdx][1];
            gainedHP = GameMaster.levelStats[curLevel - 1][curIdx][2];

            gainedCoins = GameMaster.levelStats[curLevel - 1][4][1];
            gainedDiamonds = GameMaster.levelStats[curLevel - 1][4][2];
        }

        transform.Find("Shape").GetComponent<Image>().sprite = curShape;
        transform.Find("Shape").Find("Level").GetComponent<Text>().text = curLevel.ToString();

        transform.Find("AttackGained").GetComponent<Text>().text = "+" + gainedAtck.ToString();
        transform.Find("DefenseGained").GetComponent<Text>().text = "+" + gainedDef.ToString();
        transform.Find("HPGained").GetComponent<Text>().text = (100 + gainedHP).ToString();

        transform.Find("CoinsGained").GetComponent<Text>().text = "+" + gainedCoins.ToString();
        transform.Find("Diamonds Gained").GetComponent<Text>().text =  "+" + gainedDiamonds.ToString();

        transform.parent.SetAsLastSibling();
    }
}