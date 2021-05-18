using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealingCircle : Shape_Abilities
{
    private GameMaster GM;
    private Button But;

    public override void Awake()
    {
        ID = 41;
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
        if (transform.root.gameObject.name == "Abilities") { return; }

        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
        if (transform.root.name == "Canvas1" || transform.root.name == "Canvas2")
        {
            But = gameObject.GetComponent<Button>();
        }
    }

    public override void updateState()
    {
        if (transform.root.name == "Canvas1")
        {
            if (GM.Healing1 || player.GetLife()>=100)
                But.interactable = false;
        }
        else if (transform.root.name == "Canvas2")
        {
            if (GM.Healing2 || player.GetLife()>=100)
                But.interactable = false;
        }
    }

    public override void UseAbility()
    {
        if ((P1 && choiceDoneP1) || (!P1 && choiceDoneP2)) { return; }

        if (P1)
        {
            GM.Healing1 = true;
            GM.Heal1 = AttPow;
        }
        else
        {
            GM.Healing2 = true;
            GM.Heal2 = AttPow;
        }
        base.UseAbility();
    }
}
