using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnergyDrain : Shape_Abilities {
    GameMaster GM;
    public override void Awake()
    {
        ID = 9;
        base.Awake();
    }

    public override void SetSpecs()
    {
        attack = new int[3] { 0, 0, 0 };
        defense = new int[3] { 0, 0, 0 };
        escapeFrom = new bool[3] { false, false, false };
    }

    public override void Start()
    {
        if(transform.parent.name != "AllAbilities")
            GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
    }
    public override void UseAbility()
    {
        if ((P1 && choiceDoneP1) || (!P1 && choiceDoneP2)) { return; }
        if (P1)
            GM.PPDrain1 = AttPow;
        else
            GM.PPDrain2 = AttPow;
        base.UseAbility();
    }
}
