using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameShield : Shape_Abilities { 
    public override void Awake()
    {
        ID = 55;
        base.Awake();
    }

    public override void SetSpecs()
    {
        attack = new int[3] { 0, 0, 0 };
        defense = new int[3] { DefPow, 0, DefPow };
        escapeFrom = new bool[3] { false, false, false };
    }
}
