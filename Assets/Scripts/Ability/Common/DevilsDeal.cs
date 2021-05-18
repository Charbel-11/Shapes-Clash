using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DevilsDeal : Shape_Abilities {
    private GameMaster GM;

    public override void Awake()
    {
        ID = 8;
        if (transform.root.gameObject.name == "Abilities") { return; }

        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
        base.Awake();
    }

    public override void SetSpecs()
    {
        attack = new int[3] { 0, 0, 0 };
        defense = new int[3] { 0, 0, 0 };
        escapeFrom = new bool[3] { false, false, false };
    }

    //Can be used every 4 round only
    public override void updateState()
    {
        if (!onlineP2)
        {
            if (P1 && transform.root.name == "Canvas1")
            {
                if (GM.StartRoundCountP1 != 0)
                    thisButt.interactable = false;
            }
            else if (player.gameObject.name == "Player2" && (transform.root.name == "Canvas2" || transform.root.name == "Bot"))
            {
                if (GM.StartRoundCountP2 != 0)
                    thisButt.interactable = false;
            }
        }
    }
}