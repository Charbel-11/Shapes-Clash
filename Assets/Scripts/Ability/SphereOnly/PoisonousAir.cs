using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoisonousAir : Shape_Abilities
{
    private GameMaster GM;
    private Button But;
    public override void Awake()
    {
        ID = 44;
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
        if (transform.root.name == "Canvas1" || transform.root.name == "Canvas2" || transform.root.name == "Bot")
        {
            But = gameObject.GetComponent<Button>();
        }
    }

    public override void updateState()
    {
        if (transform.root.name == "Canvas1")
        {
            if (GM.PoisonAir1)
                But.interactable = false;
        }
        else if (transform.root.name == "Canvas2" || transform.root.name == "Bot")
        {
            if (GM.PoisonAir2)
                But.interactable = false;
        }
    }

    public override void UseAbility()
    {
        if ((P1 && choiceDoneP1) || (!P1 && choiceDoneP2)) { return; }

        if (P1)
        {
            GM.PoisonAir1 = true;
            GM.PoisonPow1 = AttPow;
        }
        else
        {
            GM.PoisonAir2 = true;
            GM.PoisonPow2 = AttPow;
        }
        base.UseAbility();
    }
}
