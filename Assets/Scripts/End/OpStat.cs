using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class OpStat : MonoBehaviour {
    public Sprite[] Shapes;     //Must be in the order Cube/pyramid/...
    public GameObject describer;
    public GameObject PassivesInfo;
    public GameObject allAbilities;
    public GameObject allSpecials;

    public string[] AbilitiesLevel;
    public string[] super100Level;
    public string[] super200Level;

    public GameMaster GM;
    public Replay curReplay;

    private string username;
    public int[] Abilities;
    public int superID;
    public int[] passivesLevel;
    public string specialAb;
    public int opLevel;
    public int opPP;
    private Transform topBar;
    public int opShapeID;
    private int[][] passiveStats;

    public void UpdateOp() {
        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
        topBar = transform.Find("Top Bar");

        if (!GameMaster.Online && !GameMaster.botOnline) { return; }

        Abilities = TempOpponent.Opponent.AbIDs;
        superID = TempOpponent.Opponent.SuperID;
        AbilitiesLevel = TempOpponent.Opponent.AbLevelArray;
        super100Level = TempOpponent.Opponent.Super100;
        super200Level = TempOpponent.Opponent.Super200;
        opPP = TempOpponent.Opponent.OpPP;
        opLevel = TempOpponent.Opponent.OpLvl;
        passivesLevel = TempOpponent.Opponent.Passives;
        username = TempOpponent.Opponent.Username;
        opShapeID = TempOpponent.Opponent.ShapeID;

        passiveStats = GM.PassiveStats;

        updatePanel();
    }

    public void UpdateOp2() {
        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
        topBar = transform.Find("Top Bar");

        if (!GameMaster.Online) { return; }

        Abilities = TempOpponent.Opponent.AbIDs2;
        superID = TempOpponent.Opponent.SuperID2;
        AbilitiesLevel = TempOpponent.Opponent.AbLevelArray;
        super100Level = TempOpponent.Opponent.Super100;
        super200Level = TempOpponent.Opponent.Super200;
        opPP = TempOpponent.Opponent.OpPP2;
        opLevel = TempOpponent.Opponent.OpLvl2;
        passivesLevel = TempOpponent.Opponent.Passives;
        username = TempOpponent.Opponent.Username;
        opShapeID = TempOpponent.Opponent.ShapeID12;

        passiveStats = GM.PassiveStats;

        updatePanel();
    }

    public void updateProfilePanel(string[] s) {
        topBar = transform.Find("Top Bar");

        string[] PassiveStatsArray = PlayerPrefsX.GetStringArray("PassiveStatsArray");
        passiveStats = ClientHandleData.TransformStringArray(PassiveStatsArray); // 0 = StuckinPlace, 1 = Hardening, 2 = Freeze, 3 = Snowing, 4 = Burn, 5 = FieryEyes, 6 = HelperSpheres, 7 =Fog

        if (s[3] == "N") { opShapeID = 0; Abilities = new int[] { 2, 2, 2, 2, 2, 2 }; superID = 101; }
        else {
            string lastDeck = s[3];
            string[] sep = lastDeck.Split(',');
            opShapeID = Int32.Parse(sep[0]);
            Abilities = new int[6];
            for (int i = 0; i < 6; i++)
                Abilities[i] = Int32.Parse(sep[i + 1]);
            superID = Int32.Parse(sep[7]);
        }

        username = s[1];
        opPP = Int32.Parse(s[2].Split(',')[opShapeID]);
        AbilitiesLevel = s[5].Split(',');
        super100Level = s[6].Split(',');
        super200Level = s[7].Split(',');
        string[] passivesLevelStr = s[8].Split(',');
        passivesLevel = new int[8];
        for (int i = 0; i < 8; i++)
            passivesLevel[i] = Int32.Parse(passivesLevelStr[i]);
        opLevel = Int32.Parse(s[9].Split(',')[opShapeID]);

        updatePanel();
    }

    public void updatePanel() {
        Transform selectedShape = transform.Find("Selected Shape");
        topBar.Find("Player Name").GetComponent<Text>().text = username;
        selectedShape.transform.Find("Image").GetComponent<Image>().sprite = Shapes[opShapeID];
        selectedShape.transform.Find("PPs").GetComponentInChildren<Text>().text = opPP.ToString();

        int z = 0;
        while (z < ShapeConstants.PPRange.Length && ShapeConstants.PPRange[z] <= opPP) { z++; }
        selectedShape.transform.Find("PPs").GetComponentInChildren<Text>().color = ShapeConstants.PPColors[z];

        for (int i = 0; i < 4; i++) {
            Transform curChild = selectedShape.transform.Find("Levels").GetChild(i);
            if (i == opShapeID) {
                curChild.gameObject.SetActive(true);
                curChild.GetChild(0).GetChild(0).GetComponent<Text>().text = opLevel.ToString();
            }
            else {
                curChild.gameObject.SetActive(false);
            }
        }

        //PASSIVES
        int lvlP1 = passivesLevel[opShapeID * 2];
        int lvlP2 = passivesLevel[1 + opShapeID * 2];
        Transform passive = transform.Find("Selected Passives").Find("All");

        passive.Find("P1").Find("ActiveP1").GetComponentInChildren<Text>().text = ShapeConstants.PassiveNames[opShapeID * 2];
        passive.Find("P2").Find("ActiveP2").GetComponentInChildren<Text>().text = ShapeConstants.PassiveNames[1 + opShapeID * 2];

        if (lvlP1 == 0)
            passive.Find("P1").Find("Percentage").GetComponentInChildren<Text>().text = "0 %";
        else
            passive.Find("P1").Find("Percentage").GetComponentInChildren<Text>().text = (100 / passiveStats[opShapeID * 2][2 + lvlP1]).ToString() + "%";

        if (lvlP2 == 0)
            passive.Find("P2").Find("Percentage").GetComponentInChildren<Text>().text = "0 %";
        else
            passive.Find("P2").Find("Percentage").GetComponentInChildren<Text>().text = (100 / passiveStats[1 + opShapeID * 2][2 + lvlP2]).ToString() + "%";

        Color tempC = (lvlP1 == 3) ? ShapeConstants.levelMaxColors[opShapeID] : ShapeConstants.levelColors[opShapeID];
        for (int i = 0; i < lvlP1; i++)
            passive.Find("LevelP1").transform.GetChild(i).GetComponent<Image>().color = tempC;
        for (int i = lvlP1; i < 3; i++)
            passive.Find("LevelP1").transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1);

        tempC = (lvlP2 == 3) ? ShapeConstants.levelMaxColors[opShapeID] : ShapeConstants.levelColors[opShapeID];
        for (int i = 0; i < lvlP1; i++)
            passive.Find("LevelP2").transform.GetChild(i).GetComponent<Image>().color = tempC;
        for (int i = lvlP1; i < 3; i++)
            passive.Find("LevelP2").transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1);

        //ABILITIES
        GameObject SelectedActive = transform.Find("Selected Actives").gameObject;
        GameObject levelCont;
        int level, curIdx = opShapeID;
        Shape_Abilities cur;
        for (int i = 0; i < 6; i++) {
            GameObject Ab = SelectedActive.transform.Find("A" + (i + 1).ToString()).gameObject;

            cur = null;
            foreach (Transform a in allAbilities.transform) {
                if (a.GetComponent<Shape_Abilities>().ID == Abilities[i]) {
                    cur = a.GetComponent<Shape_Abilities>();
                    break;
                }
            }
            Ab.GetComponentInChildren<Text>().text = (cur != null ? cur.name : "");
            if (cur == null) {
                Ab.transform.GetChild(0).GetChild(0).GetComponent<Image>().enabled = false;
                Ab.GetComponent<Button>().interactable = false;
                level = 0;
            }
            else {
                Ab.transform.GetChild(0).GetChild(0).GetComponent<Image>().enabled = true;
                Ab.GetComponent<Button>().interactable = true;
                if (!cur.common)
                    Ab.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = cur.transform.Find("Image").GetComponent<Image>().sprite;
                else
                    Ab.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = cur.transform.GetChild(opShapeID).GetComponent<Image>().sprite;

                Int32.TryParse(AbilitiesLevel[Abilities[i]], out level);
            }

            Ab.transform.Find("Name").GetComponent<Image>().color = ShapeConstants.bckgNameColor[curIdx];
            levelCont = SelectedActive.transform.Find("LevelA" + (i + 1).ToString()).gameObject;

            tempC = (level == 3) ? ShapeConstants.levelMaxColors[curIdx] : ShapeConstants.levelColors[curIdx];
            for (int j = 0; j < level; j++) {
                levelCont.transform.GetChild(j).GetComponent<Image>().color = tempC;
            }
            for (int j = level; j < 3; j++) {
                levelCont.transform.GetChild(j).GetComponent<Image>().color = new Color(1, 1, 1);
            }
        }

        //For the special ability 
        Transform specAb = transform.Find("Special Ab").Find("Special Ability");

        level = 0;
        if (superID > 200)
            Int32.TryParse(super200Level[superID - 201], out level);
        else if (superID > 100)
            Int32.TryParse(super100Level[superID - 101], out level);

        cur = null;
        if (level > 0) {
            foreach (Transform a in allSpecials.transform) {
                if (a.GetComponent<Shape_Abilities>().ID == superID) {
                    cur = a.GetComponent<Shape_Abilities>();
                    break;
                }
            }
        }
        specAb.GetComponentInChildren<Text>().text = (cur ? cur.name : "");
        if (cur != null) {
            specAb.transform.Find("Image").GetComponent<Image>().enabled = true;
            specAb.GetComponent<Button>().interactable = true;
            specAb.transform.Find("Image").GetComponent<Image>().sprite = cur.transform.Find("Image").GetComponent<Image>().sprite;
        }
        else {
            specAb.transform.Find("Image").GetComponent<Image>().enabled = false;
            specAb.GetComponent<Button>().interactable = false;
        }
        levelCont = transform.Find("Special Ab").Find("Level").gameObject;

        specAb.transform.Find("Name").GetComponent<Image>().color = ShapeConstants.bckgNameColor[curIdx];
        tempC = (level == 3) ? ShapeConstants.levelMaxColors[curIdx] : ShapeConstants.levelColors[curIdx];
        for (int j = 0; j < level; j++)
            levelCont.transform.GetChild(j).GetComponent<Image>().color = tempC;
        for (int j = level; j < 3; j++)
            levelCont.transform.GetChild(j).GetComponent<Image>().color = new Color(1, 1, 1);
    }

    public void OpenExplanatoryPanel(int i) {
        describer.SetActive(true);
        if (i == 6)
            describer.GetComponent<DescribeMe>().showMe(superID);
        else
            describer.GetComponent<DescribeMe>().showMe(Abilities[i]);
    }

    public void OpenPassivePanel(int ID) {
        PassivesInfo.gameObject.SetActive(true);
        for (int i = 0; i < PassivesInfo.transform.childCount; i++) {
            PassivesInfo.transform.GetChild(i).gameObject.SetActive(i == ID);
            if (i == ID)
                PassivesInfo.transform.GetChild(i).GetComponent<ShowPassive>().updateInfo();
        }
    }
}