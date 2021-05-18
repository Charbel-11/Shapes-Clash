using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyramid_Player : Shape_Player {

    public bool CanFreeze;
    public bool CanSnow;
    public GameObject Bubble;
    public GameObject BubbleShield;
    public GameObject BluePlanet;
    public GameObject WaterCage;
    public GameObject IceShards;
    public GameObject Snow;
    public GameObject WaterTransfer;

    new void OnEnable()
    {
        base.OnEnable();

        if (Shape_Abilities.Tutorial)
        {
            CanFreeze = false;
            CanSnow = false;
            return;
        }

        if (gameObject.name == "Player1" || GM is GameMasterOffline)
        {
            if (GM.PassivesArray[2] > 0)
                CanFreeze = true;
            else
                CanFreeze = false;

            if (GM.PassivesArray[3] > 0)
                CanSnow = true;
            else
                CanSnow = false;
        }
        else
        {
            if (TempOpponent.Opponent.Passives[2] > 0)
                CanFreeze = true;
            else
                CanFreeze = false;

            if (TempOpponent.Opponent.Passives[3] > 0)
                CanSnow = true;
            else
                CanSnow = false;
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
        if(ID == 37)
        {
            BluePlanet.SetActive(true);
            Invoke("SetFalse", 2.5f);
        }
        if(ID == 203 && otherPlayerID < 100)
        {
            WaterTransfer.SetActive(true);
            Invoke("SetFalse", 2.5f);
        }
        else if(ID == 203 && otherPlayerID > 100)
        {
            var main = WaterTransfer.GetComponent<ParticleSystem>().main;
            main.startLifetime = 1.4f;
            WaterTransfer.SetActive(true);
            Invoke("SetFalse", 2.5f);
        }
        if (otherPlayerID < 100)
        {
            base.Choice(ID);
            if (ID == 22)
            {
                Invoke("Attack", 0.1f);
            }
            else if(ID == 35)
            {
                Bubble.SetActive(true);
                Invoke("SetFalse", 2.5f);
            }
            else if(ID == 36)
            {
                BubbleShield.SetActive(true);
                Invoke("SetFalse", 2.5f);
            }
            else if(ID == 38)
            {
                WaterCage.SetActive(true);
                Invoke("SetFalse", 2.5f);
            }
            else if(ID == 28)
            {
                IceShards.SetActive(true);
                Invoke("SetFalse", 2.5f);
            }
        }        
    }
    public void Attack()
    {
        var pos = gameObject.transform.Find("position").GetComponent<Transform>();
        Instantiate(Bullet, pos.position, pos.rotation);
    }
    public override void SetFalse()
    {
        if (Bubble.activeSelf) Bubble.SetActive(false);
        if (BubbleShield.activeSelf) BubbleShield.SetActive(false);
        if (BluePlanet.activeSelf) BluePlanet.SetActive(false);
        if (WaterCage.activeSelf) WaterCage.SetActive(false);
        if (IceShards.activeSelf) IceShards.SetActive(false);
        if (WaterTransfer.activeSelf) WaterTransfer.SetActive(false);
        var main = WaterTransfer.GetComponent<ParticleSystem>().main;
        main.startLifetime = 2.8f;
    }
}
