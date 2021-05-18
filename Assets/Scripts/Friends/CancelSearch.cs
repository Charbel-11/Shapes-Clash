using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CancelSearch : MonoBehaviour
{
    public GameObject Battle;
    private SelectionManager SM;

    private void OnEnable()
    {
        gameObject.GetComponent<Button>().interactable = true;
        SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
    }
    public void Cancel(bool b = true)  //b is false in the special case when we cancelled a search to get into a friendly game
    {
        try { ClientTCP.PACKAGE_MainMenu(true, PlayerPrefs.GetInt("GameMode")); }
        catch (Exception) { SM.showError(); }

        foreach (Button c in StartOnlineGame.Butts)
        {
            if (c.name != "FriendlyBattle" && c.name != "AcceptBattle")
                c.interactable = true;
        }
        if (b)  
        {
            Battle.SetActive(true);
            TempOpponent.Opponent.Accepting = false;
            TempOpponent.Opponent.FriendlyBattle = false;
        }
        transform.parent.gameObject.SetActive(false);
    }
}

