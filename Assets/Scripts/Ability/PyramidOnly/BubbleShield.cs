using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleShield : Shape_Abilities { 
    public override void Awake()
    {
        ID = 36;
        base.Awake();
    }

    public override void SetSpecs()
    {
        attack = new int[3] { 0, 0, 0 };
        defense = new int[3] { 0, DefPow, DefPow };
        escapeFrom = new bool[3] { false, false, false };
    }
}
