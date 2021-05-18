using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MirrorDefense : Shape_Abilities {
    public override void Awake()
    {
        ID = 11;
        base.Awake();
    }

    public override void SetSpecs()
    {
        attack = new int[3] { 0, 0, 0 };
        defense = new int[3] { 0, 0, 0 };
        escapeFrom = new bool[3] { false, false, false };
    }
}
