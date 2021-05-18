using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Show_EP : MonoBehaviour
{
    private Text textToShow;

    private GameMaster GM;

    public void UpdateEP()
    {
        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
        textToShow = this.GetComponent<Text>();

        if (!GameMaster.Online && transform.root.name == "Canvas2")
            textToShow.text = GM.player2.GetEP().ToString();
        else
            textToShow.text = GM.player1.GetEP().ToString();
    }
}
