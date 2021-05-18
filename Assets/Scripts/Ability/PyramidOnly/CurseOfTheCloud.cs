using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CurseOfTheCloud : Shape_Abilities {
    private GameMaster GM;

    public override void Awake()
    {
        ID = 18;
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
        if (player.gameObject.name == "Player1" && !GameMaster.Spectate)
        {
            if (GM.StartRoundCountCurseP2 != 0)
                thisButt.interactable = false;
        }
        else
        {
            if (GM.StartRoundCountCurseP1 != 0)
            {
                if (!GameMaster.Online)
                    thisButt.interactable = false;
            }
        }
    }
}
