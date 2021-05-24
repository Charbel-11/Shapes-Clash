using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_Player : Shape_Player {
    public GameObject Earthquake;
    GameObject[] Children;
    FallInPieces[] ObjectsToFall;
    int Childcount;
    Animation anim;

    public GameObject MudAttack;
    public GameObject MudEruption;

    public bool CanStuckInPlace;
    public bool CanHaveProtectiveEarth;

    public GameObject ShieldAppearance;
    public GameObject ShieldDisappearance;

    new void OnEnable()
    {
        base.OnEnable();

        Childcount = Earthquake.transform.childCount;
        Children = new GameObject[Earthquake.transform.childCount];
        ObjectsToFall = new FallInPieces[Earthquake.transform.childCount];
        for (int i = 0; i < Childcount; i++)
        {
            Children[i] = Earthquake.transform.GetChild(i).gameObject;
            ObjectsToFall[i] = Children[i].GetComponent<FallInPieces>();
        }
        if (Shape_Abilities.Tutorial)
        {
            CanStuckInPlace = false;
            CanHaveProtectiveEarth = false;
            return;
        }
        if (gameObject.name == "Player1" || GM is GameMasterOffline)
        {
            if (GM.PassivesArray[0] > 0)
                CanStuckInPlace = true;
            else
                CanStuckInPlace = false;

            if (GM.PassivesArray[1] > 0)
                CanHaveProtectiveEarth = true;
            else
                CanHaveProtectiveEarth = false;
        }
        else
        {
            if (TempOpponent.Opponent.Passives[0] > 0)
                CanStuckInPlace = true;
            else
                CanStuckInPlace = false;

            if (TempOpponent.Opponent.Passives[1] > 0)
                CanHaveProtectiveEarth = true;
            else
                CanHaveProtectiveEarth = false;
        }
            
    }

    public void Attack()
    {
        var pos = gameObject.transform.Find("position").GetComponent<Transform>();
        Instantiate(Bullet, pos.position, pos.rotation);
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
        if (ID == 1 && otherPlayerID < 100)
        {
            Invoke("Attack", 0.1f);
        }
        else if (ID == 5 & otherPlayerID < 100)
        {
            //Mini cube barrage attacks with 4 mini cubes
            if(otherPlayerID == 31)
            {
                Invoke("Attack", 0.1f);
                Invoke("Attack", 0.2f);
                Invoke("Attack", 0.35f);
                Invoke("Attack", 0.45f);
            }
            else
            {
                Invoke("Attack", 0.15f);
                Invoke("Attack", 0.25f);
                Invoke("Attack", 0.6f);
                Invoke("Attack", 0.7f);
            }
        }
        else if ((ID == 12 || ID == 202) && otherPlayerID<100)
        {
            EarthquakeOngoing = true;
            anim = GameObject.Find("Ground").GetComponent<Animation>();
            anim.Play();
            for (int i = 0; i < 5; i++)
            {
                Invoke("Earthquake1", i / 2);
            }
            Invoke("Stop", 2f);
        }
        else if(ID == 39 && otherPlayerID < 100)
        {
            MudAttack.SetActive(true);
            Invoke("ResetIt", 2.5f);
        }
        else if(ID == 40 && otherPlayerID < 100)
        {
            MudEruption.SetActive(true);
            Invoke("ResetIt", 2.5f);
        }
    }

    void Earthquake1()
    {
        for (int i = 0; i < Childcount; i++)
        {
            ObjectsToFall[i].Fall();
        }
    }
    void Stop()
    {
        anim.Stop();
    }
    private void ResetIt()
    {
        MudAttack.SetActive(false);
        MudEruption.SetActive(false);
    }
}
