using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ToxicWorm : Shape_Abilities
{
    public override void Awake()
    {
        ID = 204;
        base.Awake();
    }

    public override void SetSpecs()
    {
        attack = new int[3] { 0, 0, AttPow };
        defense = new int[3] { 0, 0, 0 };
        escapeFrom = new bool[3] { true, true, true };
    }
}
