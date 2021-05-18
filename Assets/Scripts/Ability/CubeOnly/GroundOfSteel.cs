using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GroundOfSteel : Shape_Abilities {
    public override void Awake()
    {
        ID = 14;
        base.Awake();
    }

    public override void SetSpecs()
    {
        attack = new int[3] { 0, 0, 0 };
        defense = new int[3] { 0, DefPow, DefPow};
        escapeFrom = new bool[3] { false, false, false };
    }
}
