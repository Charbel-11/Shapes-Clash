using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ShadowDissolve : Shape_Abilities {

    private GameMaster GM;

    public override void Awake()
    {
        ID = 54;
        if (transform.root.gameObject.name == "Abilities") { return; }

        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
        base.Awake();
    }

    public override void SetSpecs()
    {
        attack = new int[3] { 0, 0, 0 };
        defense = new int[3] { 0, 0, 0 };
        escapeFrom = new bool[3] { true, false, true };
    }

    public override void updateState()
    {
        if (transform.root.name == "Canvas1")
        {
            if (GM.DoitSmoke1)
                thisButt.interactable = false;
        }
        else if (transform.root.name == "Canvas2")
        {
            if (GM.DoitSmoke2)
                thisButt.interactable = false;
        }
    }
}
