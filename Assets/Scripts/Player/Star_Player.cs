using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star_Player : Shape_Player
{
    public bool CanBurn;
    public bool CanFieryEyes;
    public GameObject FBridge;
    public GameObject FLaser;
    public GameObject FBomb;
    public GameObject FShield;
    public GameObject FBoost;
    public GameObject FlameShield;

    new void OnEnable()
    {
        base.OnEnable();

        if (Shape_Abilities.Tutorial)
        {
            CanBurn = false;
            CanFieryEyes = false;
            return;
        }
        if (gameObject.name == "Player1" || GM is GameMasterOffline)
        {
            if (GM.PassivesArray[4] > 0)
                CanBurn = true;
            else
                CanBurn = false;

            if (GM.PassivesArray[5] > 0)
                CanFieryEyes = true;
            else
                CanFieryEyes = false;
        }
        else
        {
            if (TempOpponent.Opponent.Passives[4] > 0)
                CanBurn = true;
            else
                CanBurn = false;

            if (TempOpponent.Opponent.Passives[5] > 0)
                CanFieryEyes = true;
            else
                CanFieryEyes = false;
        }
        
    }

    public override void Choice(int ID)
    {
        int otherPlayerID;
        if (gameObject.name == "Player1")
        {
            otherPlayerID = GM.player2.GetIdOfAnimUsed();
        }
        else
        {
            otherPlayerID = GM.player1.GetIdOfAnimUsed();
        }
        if (otherPlayerID < 100)
        {
            base.Choice(ID);
        }
        if (ID == 23 && otherPlayerID < 100)
        {
            Invoke("Attack", 0.1f);
        }
        else if(ID == 30 && otherPlayerID < 100)
        {
            FBridge.SetActive(true);
            Invoke("SetFalse", 2.5f);
        }
        else if (ID == 31 && otherPlayerID < 100)
        {
            FLaser.SetActive(true);
            Invoke("SetFalse", 2.5f);
        }
        else if (ID == 32 && otherPlayerID < 100)
        {
            FBomb.SetActive(true);
            Invoke("SetFalse", 2.5f);
        }
        else if (ID == 24 && otherPlayerID<100)
        {
            FShield.SetActive(true);
            Invoke("SetFalse", 2.5f);
        }
        else if (ID == 33)
        {
            FBoost.SetActive(true);
            Invoke("SetFalse", 2.5f);
        }
        else if(ID == 55 && otherPlayerID < 100)
        {
            FlameShield.SetActive(true);
            Invoke("SetFalse", 2.5f);
        }
    }
    public void Attack()
    {
        var pos = gameObject.transform.Find("position").GetComponent<Transform>();
        Instantiate(Bullet, pos.position, pos.rotation);
    }
    public override void SetFalse()
    {
        FBridge.SetActive(false);
        FLaser.SetActive(false);
        FBomb.SetActive(false);
        FShield.SetActive(false);
        FBoost.SetActive(false);
        FlameShield.SetActive(false);
    }
}
