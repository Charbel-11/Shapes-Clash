using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere_Player : Shape_Player
{
    public bool CanHelperSpheres;
    public bool CanSmoke;

    public GameObject ToxicShot;
    public GameObject PoisonousAir;
    public GameObject PoisonousBubble;
    public GameObject ToxicRing;
    public GameObject AirCannon;
    public GameObject PoisonCloud;
    public GameObject SpiralShield;
    public GameObject WindArrow;
    public GameObject MiniNado;
    public GameObject HelperSpheres;
    public GameObject Smoke;
    public GameObject ToxicWorm;
    public GameObject Tornado;
    public GameObject SpiralShieldBelow;
    new void OnEnable()
    {
        base.OnEnable();

        ToxicRing.SetActive(true);

        if (Shape_Abilities.Tutorial)
        {
            CanHelperSpheres = false;
            CanSmoke = false;
            return;
        }

        if (gameObject.name == "Player1" || GM is GameMasterOffline)
        {
            if (GM.PassivesArray[6] > 0)
                CanHelperSpheres = true;
            else
                CanHelperSpheres = false;

            if (GM.PassivesArray[7] > 0)
                CanSmoke = true;
            else
                CanSmoke = false;
        }
        else
        {
            if (TempOpponent.Opponent.Passives[6] > 0)
                CanHelperSpheres = true;
            else
                CanHelperSpheres = false;

            if (TempOpponent.Opponent.Passives[7] > 0)
                CanSmoke = true;
            else
                CanSmoke = false;
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
        base.Choice(ID);
        if (ID == 104 && otherPlayerID > 100)
        {
            gameObject.GetComponent<Animator>().SetInteger("ID", -1);
            gameObject.GetComponent<Animator>().SetBool("Scared", true);
            Tornado.SetActive(true);
            Invoke("SetFalse", 2.5f);
        }
        else if (ID == 104)
        {
            Tornado.SetActive(true);
            Invoke("SetFalse", 2.5f);
        }       
        else if (ID == 204 && otherPlayerID > 100)
        {
            var main = ToxicWorm.GetComponent<ParticleSystem>().main;
            main.startLifetime = 1.4f;
            ToxicWorm.transform.GetChild(0).gameObject.SetActive(false);
            ToxicWorm.SetActive(true);
            Invoke("SetFalse", 2.5f);
        }
        else if (ID == 204)
        {
            ToxicWorm.SetActive(true);
            Invoke("SetFalse", 2.5f);
        }
        else if (ID == 44)
        {
            PoisonousAir.SetActive(true);
            Invoke("SetFalse", 2.5f);
        }
        else if (otherPlayerID < 100)
        {
            if (ID == 42)
            {
                Invoke("Attack", 0.1f);
            }
            else if (ID == 43)
            {
                ToxicShot.SetActive(true);
                Invoke("SetFalse", 2.5f);
            }          
            else if (ID == 45)
            {
                PoisonousBubble.SetActive(true);
                Invoke("SetFalse", 2.5f);
            }
            else if(ID == 46)
            {
                var Enabled = ToxicRing.GetComponent<ParticleSystem>().collision;
                BoxCollider Col = ToxicRing.GetComponent<BoxCollider>();
                Col.enabled = true;
                Enabled.enabled = true;
                Invoke("SetFalse", 2.5f);
            }
            else if (ID == 47)
            {
                AirCannon.SetActive(true);
                Invoke("SetFalse", 2.5f);
            }
            else if(ID == 48)
            {
                PoisonCloud.SetActive(true);
                Invoke("SetFalse", 2.5f);
            }
            else if(ID == 49)
            {
                SpiralShield.SetActive(true);
                Invoke("SetFalse", 2.5f);
            }
            else if(ID == 50)
            {
                WindArrow.SetActive(true);
                Invoke("SetFalse", 2.5f);
            }
            else if(ID == 51)
            {
                MiniNado.SetActive(true);
                Invoke("SetFalse", 2.5f);
            }      
            else if(ID == 56)
            {
                SpiralShieldBelow.SetActive(true);
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
        ToxicShot.SetActive(false);
        PoisonousAir.SetActive(false);
        PoisonousBubble.SetActive(false);
        var Enabled = ToxicRing.GetComponent<ParticleSystem>().collision;
        BoxCollider Col = ToxicRing.GetComponent<BoxCollider>();
        Col.enabled = false;
        Enabled.enabled = false;
        ToxicRing.GetComponent<Sealingscript>().OnEnable();
        AirCannon.SetActive(false);
        PoisonCloud.SetActive(false);
        SpiralShield.SetActive(false);
        WindArrow.SetActive(false);
        MiniNado.SetActive(false);
        ToxicWorm.SetActive(false);
        var main = ToxicWorm.GetComponent<ParticleSystem>().main;
        main.startLifetime = 5.3f;
        ToxicWorm.transform.GetChild(0).gameObject.SetActive(true);
        Tornado.SetActive(false);
        SpiralShieldBelow.SetActive(false);
    }
}
