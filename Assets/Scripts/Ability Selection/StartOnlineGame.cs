using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartOnlineGame : MonoBehaviour {
    public bool v2;
    public GameObject YouNeedToChooseMore;
    public GameObject SameShape;

    private SelectionManager SM;

    public GameObject Cancel;
    public static Button[] Butts;

    private void OnEnable()
    {
        SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
        GameObject Canvas1 = GameObject.Find("CanvasP1");
        Butts = Canvas1.transform.Find("Main Screen").Find("Top Bar").GetComponentsInChildren<Button>(false);
        if (SM.OfflineCanvas)
            gameObject.SetActive(false);
    }

    public void StartGame()
    {
        int[] IDs = new int[6];
        IDs = SM.finalIDs;

        int chosen = 0;
        for (int i = 0; i < 6; i++)
        {
            chosen += (IDs[i] != -1 ? 1 : 0);
        }

        if (chosen < 4)
        {
            YouNeedToChooseMore.SetActive(true);
        }
        else
        {
            SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();

            PlayerPrefs.SetInt("bot", 0);       //Change if we want to allow bots online
            PlayerPrefs.Save();

            if (v2)
            {
                int s1 = PlayerPrefs.GetInt("ShapeSelectedID"), s2 = PlayerPrefs.GetInt("2ShapeSelectedID");

                if (s1 == s2)
                {
                    SameShape.SetActive(true);
                    return;
                }

                TempOpponent.Opponent.ShapeIDUser = s1;
                TempOpponent.Opponent.ShapeID2User = s2;
            }
            else
            {
                TempOpponent.Opponent.ShapeIDUser = SM.finalShapeIndex;
                TempOpponent.Opponent.ShapeID2User = -1;
            }

            int Choice = 0;
            if (gameObject.name == "FriendlyBattle") { Choice = 1; }
            else if (gameObject.name == "AcceptBattle") { Choice = 2; }

            int GameMode = v2 ? 2 : 1;

            PlayerPrefs.SetInt("GameMode", GameMode);
            PlayerPrefs.Save();

            try
            {
                ClientTCP.PACKAGE_BattleOnline(PlayerPrefs.GetString("Username"), Choice, GameMode, TempOpponent.Opponent.Username);
                DeactivateButtons();
            }
            catch (Exception e)
            {
                SM.showError();
                print(e);
            }
        }
    }
    private void DeactivateButtons()
    {
        foreach(Button c in Butts)
        {
            c.interactable = false;
        }

        Cancel.SetActive(true);
        if (gameObject.name == "FriendlyBattle" || gameObject.name == "AcceptBattle")
        {
            Cancel.transform.Find("Text").GetComponent<Text>().text = "Waiting for your friend...";
        }
        else
        {
            Cancel.transform.Find("Text").GetComponent<Text>().text = "Searching for an opponent...";
        }
        if (gameObject.name == "AcceptBattle") { Cancel.GetComponentInChildren<Button>().interactable = false; }
        else { Cancel.GetComponentInChildren<Button>().interactable = true; }
        gameObject.SetActive(false);
    }
}
