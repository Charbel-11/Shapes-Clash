using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour {

    public void TaskOnClick()
    {        
        if (!SelectionManager.online) { return; }
        if (TempOpponent.Opponent.FriendlyBattle && TempOpponent.Opponent.Accepting)
        {
            ClientTCP.PACKAGE_DEBUG("No", TempOpponent.Opponent.Username);
            TempOpponent.Opponent.FriendlyBattle = false;
            TempOpponent.Opponent.Accepting = false;
        }
        else if (TempOpponent.Opponent.FriendlyBattle)
        {
            TempOpponent.Opponent.FriendlyBattle = false;
        }

        //No need as I made it such that when we have a cancel panel, we can't touch anything else
        //ClientTCP.PACKAGE_MainMenu(false, PlayerPrefs.GetInt("GameMode"));
    }
}