using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MiniCubesBarrage : Shape_Abilities {
    public override void Awake()
    {
        ID = 5;
        base.Awake();
    }

    public override void SetSpecs()
    {
        attack = new int[3] { 0, 0, AttPow};
        defense = new int[3] { 0, 0, 0 };
        escapeFrom = new bool[3] { false, false, false };
    }

    public override void UseAbility()
    {
        if ((P1 && choiceDoneP1) || (!P1 && choiceDoneP2)) { return; }

        base.UseAbility();
        player.SetBulletPower((AttPow / 4), 4);
    }
}