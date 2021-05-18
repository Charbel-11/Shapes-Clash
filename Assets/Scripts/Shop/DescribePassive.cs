using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescribePassive : MonoBehaviour {
    public int ID;

    private int abLvl;
    private Text AbilityName;

    private Text Stat1;
    private Text Chance;

    public void updateInfo()
    {
        ID = transform.GetSiblingIndex();
        abLvl = ShopManager.PassivesLevel[ID];

        Stat1 = transform.Find("Stat1").Find("Info").GetComponent<Text>();
        Chance = transform.Find("Chance").Find("Num").GetComponent<Text>();

        if (abLvl == 0)
        {
            Stat1.text = "<color=black>" + ShopManager.PassiveStatsArr[ID][0].ToString() + " - " + ShopManager.PassiveStatsArr[ID][1].ToString() + " - " + ShopManager.PassiveStatsArr[ID][2].ToString() + "</color>";
            Chance.text = "<color=black>" + ShopManager.PassiveStatsArr[ID][3].ToString() + " - " + ShopManager.PassiveStatsArr[ID][4].ToString() + " - " + ShopManager.PassiveStatsArr[ID][5].ToString() + "</color>";
        }
        else if (abLvl == 1)
        {
            Stat1.text = "<b>" + ShopManager.PassiveStatsArr[ID][0].ToString() + "</b><color=black> - " + ShopManager.PassiveStatsArr[ID][1].ToString() + " - " + ShopManager.PassiveStatsArr[ID][2].ToString() + "</color>";
            Chance.text = "<b>" + ShopManager.PassiveStatsArr[ID][3].ToString() + "</b><color=black> - " + ShopManager.PassiveStatsArr[ID][4].ToString() + " - " + ShopManager.PassiveStatsArr[ID][5].ToString() + "</color>";
        }
        else if (abLvl == 2)
        {
            Stat1.text = "<color=black>" + ShopManager.PassiveStatsArr[ID][0].ToString() + " - </color><b>" + ShopManager.PassiveStatsArr[ID][1].ToString() + "</b><color=black> - " + ShopManager.PassiveStatsArr[ID][2].ToString() + "</color>";
            Chance.text = "<color=black>" + ShopManager.PassiveStatsArr[ID][3].ToString() + " - </color><b>" + ShopManager.PassiveStatsArr[ID][4].ToString() + "</b><color=black> - " + ShopManager.PassiveStatsArr[ID][5].ToString() + "</color>";
        }
        else
        {
            Stat1.text = "<color=black>" + ShopManager.PassiveStatsArr[ID][0].ToString() + " - " + ShopManager.PassiveStatsArr[ID][1].ToString() + " - </color><b>" + ShopManager.PassiveStatsArr[ID][2].ToString() + "</b>";
            Chance.text = "<color=black>" + ShopManager.PassiveStatsArr[ID][3].ToString() + " - " + ShopManager.PassiveStatsArr[ID][4].ToString() + " - </color><b>" + ShopManager.PassiveStatsArr[ID][5].ToString() + "</b>";
        }
        
        AbilityName = transform.Find("Ability Name").transform.Find("Name").GetComponent<Text>();
        AbilityName.text = "Level " + abLvl + " " + transform.name;

        transform.Find("Description").GetComponent<Text>().text = ShapeConstants.PassiveDescription[ID];
    }
}