using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Return : MonoBehaviour
{
    public void OnClick()
    {

        if (!GameMaster.Replay)
        {
            TempOpponent.Opponent.Reset();
            GameMaster.Spectate = false;
            ClientTCP.PACKAGE_DEBUG("SpectatorOut");
        }
        else
        {
            TempOpponent.Opponent.Reset(true);
        }
        SceneManager.LoadScene("MainMenuScene");
    }

}
