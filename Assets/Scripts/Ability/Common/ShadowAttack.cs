using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ShadowAttack : Shape_Abilities{
    public override void Awake()
    {
        ID = 4;
        base.Awake();
    }

    public override void SetSpecs()
    {
        attack = new int[3] { 0, AttPow, 0 };
        defense = new int[3] { 0, 0, 0 };
        escapeFrom = new bool[3] { true, false, true };
    }
}
