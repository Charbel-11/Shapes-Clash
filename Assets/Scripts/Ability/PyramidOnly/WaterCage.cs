using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCage : Shape_Abilities{
    public override void Awake()
    {
        ID = 38;
        base.Awake();
    }

    public override void SetSpecs()
    {
        attack = new int[3] { 0, AttPow, 0 };
        defense = new int[3] { 0, 0, DefPow };
        escapeFrom = new bool[3] { false, false, false };
    }
}
