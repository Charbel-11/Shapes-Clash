using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Checkpoint : MonoBehaviour {
    public int ID;
    public int road;
    public int PPNeeded;    //Only for main road

    private int coinToGain;
    private int redBoltsToGain;
    private int diamondsToGain;

    private Button myBut;
    private int state;      //0: unavailable    1: to claim     2: claimed
    private int prevS;
    private int prevAbLvl;
    private int abID;
    private ValuesChange MM;

    private void Start()
    {
        MM = GameObject.Find("Menu Manager").GetComponent<ValuesChange>();

        Int32.TryParse(transform.name[transform.name.Length - 1].ToString(), out ID);
        ID--;

        if (road == 4)
            transform.Find("Text").GetComponent<Text>().text = PPNeeded.ToString();
        else
            transform.Find("Text").GetComponent<Text>().text = (ID + 1).ToString();

        myBut = transform.Find("Fill").GetComponent<Button>();
        myBut.onClick.AddListener(TaskOnClick);
    }
    
    void TaskOnClick()
    {
        if (state == 1) { Claim(); }
    }

    //TODO: get PPneeded from server
    public int updateState()
    {
        Int32.TryParse(transform.name[transform.name.Length - 1].ToString(), out ID);
        ID--;
        state = ValuesChange.trophyRoadUnlocked[road][ID];
        prevS = state;

        if (state == 0 && ((road == 4 && PPNeeded <= ValuesChange.maxPP) || (road != 4 && (ID + 1) <= ValuesChange.shapeLvls[road]))) {
            state = 1;
            ValuesChange.trophyRoadUnlocked[road][ID] = 1;  
            PlayerPrefsX.SetStringArray("TrophyRoadUnlocked", ClientHandleData.TransformToString(ValuesChange.trophyRoadUnlocked).Split('-'));
            PlayerPrefs.Save();
            try
            {
                ClientTCP.PACKAGE_ChestOpening();
            }
            catch (Exception)
            {
                state = prevS; ValuesChange.trophyRoadUnlocked[road][ID] = state;
                PlayerPrefsX.SetStringArray("TrophyRoadUnlocked", ClientHandleData.TransformToString(ValuesChange.trophyRoadUnlocked).Split('-'));
                PlayerPrefs.Save();
                return 0;
            }
        }

        transform.Find("Fill").GetComponent<Image>().color = ShapeConstants.checkPointsStates[state];
        transform.Find("Claim").gameObject.SetActive(state == 1);
        transform.Find("Image").gameObject.SetActive(state != 1);
        transform.Find("Text").gameObject.SetActive(state != 1);

        if (ID > 0)
            transform.parent.parent.Find("Links").Find("Link" + ID.ToString()).GetChild(0).GetComponent<Image>().color = ShapeConstants.checkPointsStates[state];

        transform.Find("Fill").GetComponent<Button>().enabled = (state == 1);

        return (state == 1) ? 1 : 0;
    }

    public void Claim()
    {
        Int32.TryParse(transform.name[transform.name.Length - 1].ToString(), out ID);
        ID--;

        abID = -1;
        if (ValuesChange.trophyRoadStats[road][ID][3] != -1) abID = ValuesChange.trophyRoadStats[road][ID][3];
        else if (ValuesChange.trophyRoadStats[road][ID][4] != -1) abID = ValuesChange.trophyRoadStats[road][ID][4];
        else if (ValuesChange.trophyRoadStats[road][ID][5] != -1) abID = ValuesChange.trophyRoadStats[road][ID][5];

        coinToGain = ValuesChange.trophyRoadStats[road][ID][0];
        redBoltsToGain = ValuesChange.trophyRoadStats[road][ID][1];
        diamondsToGain = ValuesChange.trophyRoadStats[road][ID][2];

        ValuesChange.addCoins(coinToGain);
        ValuesChange.addRedBolts(redBoltsToGain);
        ValuesChange.addDiamonds(diamondsToGain);

        if (abID != -1)
        {
            Int32.TryParse(ValuesChange.AbLevelArray[abID], out int temp);
            ValuesChange.AbLevelArray[abID] = Math.Max(1, temp).ToString();
            prevAbLvl = temp;
        }

        prevS = state;
        state = 2; ValuesChange.trophyRoadUnlocked[road][ID] = state; 

        PlayerPrefsX.SetStringArray("AbilitiesArray", ValuesChange.AbLevelArray);
        PlayerPrefsX.SetStringArray("TrophyRoadUnlocked", ClientHandleData.TransformToString(ValuesChange.trophyRoadUnlocked).Split('-'));
        ValuesChange.saveChanges();

        try { ClientTCP.PACKAGE_ChestOpening(); }
        catch (Exception) { RevertClaim(); MM.showError(); return; }

        transform.Find("Fill").GetComponent<Image>().color = ShapeConstants.checkPointsStates[state];
        transform.Find("Claim").gameObject.SetActive(false);
        transform.Find("Image").gameObject.SetActive(true);
        transform.Find("Text").gameObject.SetActive(true);

        if (ID > 0)
            transform.parent.parent.Find("Links").Find("Link" + ID.ToString()).GetChild(0).GetComponent<Image>().color = ShapeConstants.checkPointsStates[state];

        transform.Find("Fill").GetComponent<Button>().enabled = false;

        if (coinToGain > 0)
            MM.playResourcesAnim(0);
        if (redBoltsToGain > 0)
            MM.playResourcesAnim(1);
        if (diamondsToGain > 0)
            MM.playResourcesAnim(2);

        MM.changeRoadNotification(-1);
    }

    public void RevertClaim()
    {
        ValuesChange.addCoins(-coinToGain);
        ValuesChange.addRedBolts(-redBoltsToGain);
        ValuesChange.addDiamonds(-diamondsToGain);
        if (abID != -1)
            ValuesChange.AbLevelArray[abID] = prevAbLvl.ToString();
        state = prevS; ValuesChange.trophyRoadUnlocked[road][ID] = state;

        PlayerPrefsX.SetStringArray("AbilitiesArray", ValuesChange.AbLevelArray);
        PlayerPrefsX.SetStringArray("TrophyRoadUnlocked", ClientHandleData.TransformToString(ValuesChange.trophyRoadUnlocked).Split('-'));
        ValuesChange.saveChanges();
    }
}