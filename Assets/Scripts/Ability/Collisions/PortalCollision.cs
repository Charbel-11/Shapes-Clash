using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCollision : MonoBehaviour {
    //Portal1 is the start portal and portal2 is the end portal
    private GameObject portal2;

    private Transform trans;
    private List<GameObject> Obj = new List<GameObject>();

    public BoxCollider ColliderTrigger;
    private BoxCollider ColliderNotTrigger;
    public Shape_Player OtherPlayer;

    private GameMaster GM;

    private void Awake()
    {
        portal2 = gameObject.transform.Find("Portal 2").gameObject;
        trans = portal2.GetComponent<Transform>();
        if (transform.root.name == "Player1")
        {
            OtherPlayer = GameObject.Find("Player2").GetComponent<Shape_Player>();
        }
        else
        {
            OtherPlayer = GameObject.Find("Player1").GetComponent<Shape_Player>();
        }
        BoxCollider[] Boxes = GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider B in Boxes)
        {
            if (B != ColliderTrigger)
            {
                ColliderNotTrigger = B;
                break;
            }
        }
        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
    }
    private void OnEnable()
    {
        if (OtherPlayer.GetIdOfAnimUsed() == 47)
        {
            ColliderTrigger.enabled = false;
        }
        if (OtherPlayer.GetIdOfAnimUsed() == 46 || OtherPlayer.GetIdOfAnimUsed() == 38)
        {
            ColliderNotTrigger.enabled = false;
        }
        Obj.Clear();
    }
    private void OnDisable()
    {
        ColliderTrigger.enabled = true;
        ColliderNotTrigger.enabled = true;
    }
    //We assume collision can occur only with portal 1
    //If we change that, we could add a collider on portal 2
    private void OnTriggerEnter(Collider col)
    {
        if (!col.isTrigger || col.tag == "DrainingHands")
        {
            return;
        }
        if (Obj.Contains(col.gameObject))
        {
            return;
        }
        if (col.tag == "Player")
        {
            col.GetComponentInParent<Animator>().SetInteger("ID", -1);
            col.GetComponentInParent<Animator>().SetBool("Hit", true);
        }

        else if (col.tag == "Fire")
            col.GetComponentInParent<Animator>().SetTrigger("SelfFire");
        else if (col.tag == "Ice")
        {
            col.GetComponentInParent<Animator>().SetInteger("ID", -1);
            col.GetComponentInParent<Animator>().SetBool("Hit", true);        //To change for some freezy thing
        }
        else if (col.name != "Ground" && col.tag != "Steel" && col.tag != "Shield" && col.tag !="Bullet")
        {
            if (OtherPlayer.name == "Player1")
                GM.HitFace1 = true;
            else
                GM.HitFace2 = true;
            if (col.name == "FlamesParticleEffect" || col.name == "Fireball" || col.name == "IceDisk")
            {
                col.transform.root.GetComponent<Animator>().SetBool("Portal", true);
            }
            if (col.name != "FlamesParticleEffect" && col.name!= "Fireball" && col.name != "IceDisk")
            {
                col.gameObject.GetComponent<Transform>().position = trans.position;
                if (col.tag == "BulletP1")
                    col.gameObject.GetComponent<Rigidbody>().velocity = (new Vector3(0.7f, 0, 1f) * 100 * Time.deltaTime);
                else
                    col.gameObject.GetComponent<Rigidbody>().velocity = (new Vector3(-0.7f, 0, -1f) * 100 * Time.deltaTime);
            }                
            if (col.tag == "BulletP1")
            {
                if (!Obj.Contains(col.gameObject))
                {
                    if (!col.GetComponent<BulletPlayer1>().PSystem)
                    {
                        col.gameObject.layer = 16;
                        col.tag = "BulletP2";
                    }
                    Obj.Add(col.gameObject);
                }                
            }
            else if(col.tag == "BulletP2")
            {
                if (!Obj.Contains(col.gameObject))
                {
                    if (!col.GetComponent<BulletPlayer2>().PSystem)
                    {
                        col.gameObject.layer = 13;
                        col.tag = "BulletP1";
                    }
                    Obj.Add(col.gameObject);
                }               
            }
        }
        Invoke("ResetList", 2.5f);    
    }
    private void OnParticleCollision(GameObject col)
    {
        if (col.name != "Ground" && col.tag != "Steel" && col.tag != "Shield")
        {
            if (col.name == "Child" || col.name == "Child(Clone)")
            {
                col = col.transform.parent.gameObject;
            }

            if (OtherPlayer.name == "Player1")
                GM.HitFace1 = true;
            else
                GM.HitFace2 = true;

            ParticleSystem Psys = col.GetComponent<ParticleSystem>();
            ParticleSystem.Particle[] Pars = new ParticleSystem.Particle[Psys.main.maxParticles];
            Psys.GetParticles(Pars);
            Vector3 Pos = trans.position;
            if(col.name == "PoisonousBubble")
            {
                Vector3 Position;
                if(OtherPlayer.name == "Player2")
                {
                    Position = new Vector3(-0.4f, 0, -0.55f);
                }
                else
                {
                    Position = new Vector3(-0.7f, 0, -1.7f);
                }
                col.transform.GetChild(0).localPosition = Position;
            }
            else if(col.name == "WaterBubbleBarrage")
            {
                Vector3 Position;
                if (OtherPlayer.name == "Player2")
                {
                    Position = new Vector3(-1.12f, -0.56f, -0.93f);
                }
                else
                {
                    Position = new Vector3(-0.83f, -0.96f, -1.48f);
                }
                col.transform.GetChild(0).localPosition = Position;
            }
            else if(col.name == "MudAttack")
            {
                col.transform.position = trans.position;
                var Main = Psys.main;
                Main.startLifetime = 0.3f;
            }
            else if(col.name == "AirCannon")
            {
                if(OtherPlayer.name == "Player1")
                {
                    col.GetComponent<BulletPlayer1>().AirCannonMir.SetActive(true);
                }
                else
                {
                    col.GetComponent<BulletPlayer2>().AirCannonMir.SetActive(true);
                }
                col.GetComponent<ParticleSystem>().Stop();
            }
            else if(col.name == "IceShardsAttackStraight")
            {
                if (OtherPlayer.name == "Player1")
                {
                    col.GetComponent<BulletPlayer1>().IceShardsMir.SetActive(true);
                }
                else
                {
                    col.GetComponent<BulletPlayer2>().IceShardsMir.SetActive(true);
                }
                col.SetActive(false);
            }
            else if (col.name == "FireLaser")
            {
                if (OtherPlayer.name == "Player1")
                {
                    col.GetComponent<BulletPlayer1>().FireLaserMir.SetActive(true);
                }
                else
                {
                    col.GetComponent<BulletPlayer2>().FireLaserMir.SetActive(true);
                }
                col.SetActive(false);
            }
            else if (col.name == "ToxicShot")
            {
                if (OtherPlayer.name == "Player1")
                {
                    col.GetComponent<BulletPlayer1>().ToxicShotMir.SetActive(true);
                }
                else
                {
                    col.GetComponent<BulletPlayer2>().ToxicShotMir.SetActive(true);
                }
                col.SetActive(false);
            }
            for (int i = 0; i< Pars.Length; i++)
            {
                Pars[i].position = Pos;
                if(col.name == "PoisonousBubble" || col.name == "WaterBubbleBarrage")
                {
                    Vector3 Vel = new Vector3(0.2f, 0, 0.26125f);
                    Pars[i].remainingLifetime -= 0.1f;
                    Pars[i].velocity = Vel;
                }
            }
            if(col.name != "MudAttack")
            {
                Psys.SetParticles(Pars, Pars.Length);
            }
            if(col.tag == "BulletP1" || col.tag == "BulletP2")
            {
                if (!Obj.Contains(col))
                {
                    Obj.Add(col);
                }
            }
            /*if (col.tag == "BulletP1")
            {
                if (!Obj.Contains(col))
                {
                    col.layer = 11;
                    //col.tag = "BulletP2";
                    Obj.Add(col);
                }
            }
            else if (col.tag == "BulletP2")
            {
                if (!Obj.Contains(col))
                {
                    col.layer = 10;
                    //col.tag = "BulletP1";
                    Obj.Add(col);
                }
            }*/
        }
        Invoke("ResetList", 1.8f);
    }
    private void ResetList()
    {
        foreach(GameObject Ob in Obj)
        {
            if (Ob != null && (Ob.name == "PoisonousBubble" || Ob.name == "WaterBubbleBarrage"))
            {
                Vector3 Position;
                if(Ob.name == "WaterBubbleBarrage")
                {
                    Position = new Vector3(5.543407f, -0.56f, 7.4f);
                }
                else
                {
                    Position = new Vector3(5.436007f, -0.4833332f, 7.408067f);
                }
                Ob.transform.GetChild(0).position = Position;
            }
            else if(Ob!= null && Ob.name == "MudAttack")
            {
                Vector3 Pos = new Vector3(0,0.05f,-0.01f);
                Ob.transform.localPosition = Pos;
                var Main = Ob.GetComponent<ParticleSystem>().main;
                Main.startLifetime = 0.7f;
            }
            else if(Ob!= null && Ob.name == "AirCannon")
            {
                if (OtherPlayer.name == "Player1")
                {
                    Ob.GetComponent<BulletPlayer1>().AirCannonMir.SetActive(false);
                }
                else
                {
                    Ob.GetComponent<BulletPlayer2>().AirCannonMir.SetActive(false);
                }
            }
            else if (Ob != null && Ob.name == "IceShardsAttackStraight")
            {
                if (OtherPlayer.name == "Player1")
                {
                    Ob.GetComponent<BulletPlayer1>().IceShardsMir.SetActive(false);
                }
                else
                {
                    Ob.GetComponent<BulletPlayer2>().IceShardsMir.SetActive(false);
                }
            }
            else if (Ob != null && Ob.name == "FireLaser")
            {
                if (OtherPlayer.name == "Player1")
                {
                    Ob.GetComponent<BulletPlayer1>().FireLaserMir.SetActive(false);
                }
                else
                {
                    Ob.GetComponent<BulletPlayer2>().FireLaserMir.SetActive(false);
                }
            }
            else if (Ob != null && Ob.name == "ToxicShot")
            {
                if (OtherPlayer.name == "Player1")
                {
                    Ob.GetComponent<BulletPlayer1>().ToxicShotMir.SetActive(false);
                }
                else
                {
                    Ob.GetComponent<BulletPlayer2>().ToxicShotMir.SetActive(false);
                }
            }
        }
        Obj.Clear();
    }
}
