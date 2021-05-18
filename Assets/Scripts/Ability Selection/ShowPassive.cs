using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowPassive : MonoBehaviour {
    public bool endGame;
    public bool mainMenu;

    private int ID;

    private int abLvl;
    private Text AbilityName;

    private Text Stat1;
    private Text Chance;

    private bool init = false;

    private string[] PassiveStatsArrStr;
    private int[][] PassiveStatsArr;
    private int[] PassivesArray;

    void setup()
    {
        mainMenu = transform.root.name == "Main Menu";
        if (mainMenu)   //Changing profile
        {
            OpStat OS = transform.GetComponentInParent<OpStat>();
            PassivesArray = OS.passivesLevel;
        }

        if (init) { return; }
        init = true;

        if (mainMenu)
        {
            PassiveStatsArrStr = PlayerPrefsX.GetStringArray("PassiveStatsArray");
            PassiveStatsArr = ClientHandleData.TransformStringArray(PassiveStatsArrStr);
        }
        else if (!endGame)
        {
            PassiveStatsArr = SelectionManager.PassiveStatsArr;
            PassivesArray = SelectionManager.PassivesArray;
        }
        else
        {
            GameMaster GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
            PassiveStatsArr = GM.PassiveStats;
            PassivesArray = GM.PassivesArray;
        }
    }

    public void updateInfo()
    {
        setup();

        ID = transform.GetSiblingIndex();
        abLvl = PassivesArray[ID];

        Stat1 = transform.Find("Stat1").Find("Info").GetComponent<Text>();
        Chance = transform.Find("Chance").Find("Num").GetComponent<Text>();

        if (abLvl == 0)
        {
            Stat1.text = "<color=black>" + PassiveStatsArr[ID][0].ToString() + " - " + PassiveStatsArr[ID][1].ToString() + " - " + PassiveStatsArr[ID][2].ToString() + "</color>";
            Chance.text = "<color=black>" + PassiveStatsArr[ID][3].ToString() + " - " + PassiveStatsArr[ID][4].ToString() + " - " + PassiveStatsArr[ID][5].ToString() + "</color>";
        }
        else if (abLvl == 1)
        {
            Stat1.text = "<b>" + PassiveStatsArr[ID][0].ToString() + "</b><color=black> - " + PassiveStatsArr[ID][1].ToString() + " - " + PassiveStatsArr[ID][2].ToString() + "</color>";
            Chance.text = "<b>" + PassiveStatsArr[ID][3].ToString() + "</b><color=black> - " + PassiveStatsArr[ID][4].ToString() + " - " + PassiveStatsArr[ID][5].ToString() + "</color>";
        }
        else if (abLvl == 2)
        {
            Stat1.text = "<color=black>" + PassiveStatsArr[ID][0].ToString() + " - </color><b>" + PassiveStatsArr[ID][1].ToString() + "</b><color=black> - " + PassiveStatsArr[ID][2].ToString() + "</color>";
            Chance.text = "<color=black>" + PassiveStatsArr[ID][3].ToString() + " - </color><b>" + PassiveStatsArr[ID][4].ToString() + "</b><color=black> - " + PassiveStatsArr[ID][5].ToString() + "</color>";
        }
        else
        {
            Stat1.text = "<color=black>" + PassiveStatsArr[ID][0].ToString() + " - " + PassiveStatsArr[ID][1].ToString() + " - </color><b>" + PassiveStatsArr[ID][2].ToString() + "</b>";
            Chance.text = "<color=black>" + PassiveStatsArr[ID][3].ToString() + " - " + PassiveStatsArr[ID][4].ToString() + " - </color><b>" + PassiveStatsArr[ID][5].ToString() + "</b>";
        }

        AbilityName = transform.Find("Ability Name").transform.Find("Name").GetComponent<Text>();
        AbilityName.text = "Level " + abLvl + " " + transform.name;

        transform.Find("Description").GetComponent<Text>().text = ShapeConstants.PassiveDescription[ID];
    }
}