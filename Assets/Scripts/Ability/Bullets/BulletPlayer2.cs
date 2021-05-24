using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class BulletPlayer2 : MonoBehaviour
{
    public int speed;
    public bool lobbed;
    Shape_Player player1;
    Shape_Player player2;
    FallInPieces objectToFall;
    bool firsttime = true;
    public int bulletpowerP2;
    int bulletpowerP1;
    int player1Shield;
    public bool isMate;
    public MateCode Mate;
    private bool MateDone = false;
    public bool PSystem;
    public GameObject LastCol;
    ParticleSystem.Particle[] Pars = new ParticleSystem.Particle[1];
    ParticleSystem Psys;
    private bool Stop = false;
    private Vector3 Vel = Vector3.zero;
    private Vector3 Pos = Vector3.zero;
    private float RemLife = 0;
    Stopwatch Watch = new Stopwatch();
    private float T;
    public bool IsAnim;
    public GameObject AirCannonMir;
    public GameObject IceShardsMir;
    public GameObject FireLaserMir;
    public GameObject ToxicShotMir;
    public GameObject BeforeLastCol = null;
    private bool FirstCollision = true;
    private Collider[] MyCols;
    private bool SetSpeed = false;
    private void Awake()
    {
        player1 = GameObject.Find("Player1").GetComponent<Shape_Player>();
        player2 = GameObject.Find("Player2").GetComponent<Shape_Player>();
        if (isMate)
        {
            Mate = player2.gameObject.transform.Find("Mate").GetComponent<MateCode>();
            GetComponent<BoxCollider>().isTrigger = true;
            GetComponent<Rigidbody>().useGravity = false;

            GetComponent<Rigidbody>().velocity = (new Vector3(-0.6f, 0, -1f) * speed * Time.fixedDeltaTime);
        }
        else if (!lobbed && !PSystem)
        {
            GetComponent<BoxCollider>().isTrigger = true;
            GetComponent<Rigidbody>().useGravity = false;

            GetComponent<Rigidbody>().velocity = (new Vector3(-0.6f, 0, -1f) * speed * Time.fixedDeltaTime);
        }
        if (isMate)
        {
            bulletpowerP2 = Mate.GetBulletPow();
        }
        else
        {
            bulletpowerP2 = player2.GetBulletPower();
        }
        //Script that instantiates 4 mini cubes in random directions with gravity
        if (!PSystem)
        {
            objectToFall = gameObject.GetComponent<FallInPieces>();
        }
        if (gameObject.name == "WaterBubbleBarrage" || gameObject.name == "PoisonousBubble")
        {
            Psys = gameObject.GetComponent<ParticleSystem>();
        }
        MyCols = gameObject.GetComponents<Collider>();
    }
    private void OnEnable()
    {
        FirstCollision = true;
        LastCol = null;
        BeforeLastCol = null;
        MateDone = false;
        SetSpeed = false;
        if (PSystem)
        {
            bulletpowerP2 = player2.GetBulletPower();
            firsttime = true;
            if (gameObject.name == "WaterBubbleBarrage" || gameObject.name == "PoisonousBubble")
            {
                Stop = false;
                Watch = new Stopwatch();
                Vel = Vector3.zero;
                Pos = Vector3.zero;
                RemLife = 0;
            }
        }
        foreach (Collider C in MyCols)
            C.enabled = true;
        if (!PSystem)
            if (player2.GetTriggerCollision())
                foreach (Collider C in MyCols)
                    if (!C.isTrigger)
                        C.enabled = false;
    }
    private void FixedUpdate()
    {
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().velocity = (new Vector3(-0.6f, 0, -1f) * speed * Time.fixedDeltaTime);
        if ((gameObject.name == "WaterBubbleBarrage" || gameObject.name == "PoisonousBubble") && !Stop)
        {
            Psys.GetParticles(Pars);
            if (Vel == Vector3.zero && Pos == Vector3.zero)
            {
                Vel = Pars[0].animatedVelocity;
                Pos = Pars[0].position;
                RemLife = Pars[0].remainingLifetime;
            }
            else
            {
                if (Pars[0].animatedVelocity.y == 0)
                {
                    Vel = Pars[0].animatedVelocity;
                    Pos = Pars[0].position;
                    RemLife = Pars[0].remainingLifetime;
                }
            }
        }
        else if (Stop)
        {
            long Secs = Watch.ElapsedMilliseconds;
            float Life = ((RemLife * 1000 - Secs) / 1000) - T;
            if (Life < 0)
            {
                Life = 0;
            }
            Vector3 Val = Vector3.zero;
            if (gameObject.name == "WaterBubbleBarrage")
            {
                Val = new Vector3(-3.5f, 0, -4.6f);
            }
            else if (gameObject.name == "PoisonousBubble")
            {
                Val = new Vector3(-1.6f, 0, -2.1f);
            }
            if (Life <= 0.15)
            {
                Vel = Vector3.zero;
            }
            else if (Vel != Val)
            {
                Vel = Val;
            }
            Pars[0].velocity = Vel;
            if (Vel != Vector3.zero)
            {
                Pars[0].position = new Vector3(Pos.x + ((Vel.x * Secs) / 1000), Pos.y, Pos.z + ((Vel.z * Secs) / 1000));
            }
            Pars[0].remainingLifetime = Life;
            Psys.SetParticles(Pars, 1);
        }
    }
    //Doesn't work for lobbed bullets since they use gravity
    private void OnTriggerEnter(Collider col)
    {
        if (col.name == "Ground" || col.tag == "Untagged" || col.tag == "BulletP2")
        {
            return;
        }
        if(!col.isTrigger && col.name == "Child(Clone)")
        {
            if (FirstCollision)
            {
                OnParticleCollision(col.gameObject);
                FirstCollision = false;
            }
            return;
        }
        if (!col.isTrigger && col.tag != "Shield")
        {
            return;
        }
        if (LastCol == null)
        {
            LastCol = col.gameObject;
            BeforeLastCol = col.gameObject;
        }
        else
        {
            if (LastCol == col.gameObject || (LastCol.tag == "Bullet" && col.tag == "Bullet"))
            {
                return;
            }
            else
            {
                BeforeLastCol = LastCol;
                LastCol = col.gameObject;
            }
        }
        int Dir = FindDir(gameObject);
        if (col.tag == "BulletP1" && !MateDone && col.GetComponent<BulletPlayer1>().isMate)
        {
            bulletpowerP1 = col.GetComponent<BulletPlayer1>().bulletpowerP1;
            MateDone = true;
        }
        else if (firsttime)
        {           
            if (col.tag == "BulletP1" && col.GetComponent<BulletPlayer1>().isMate)
            {
                return;
            }
            if(col.tag == "Shield")
                player1Shield = col.GetComponent<ShieldCol>().ShieldPower[Dir];
            if (col.tag == "BulletP1")
            {
                if (col.GetComponent<BulletPlayer1>().BeforeLastCol == gameObject || col.GetComponent<BulletPlayer1>().BeforeLastCol == null)
                {
                    bulletpowerP1 = player1.GetBulletPower();
                }
                else
                {
                    bulletpowerP1 = col.GetComponent<BulletPlayer1>().bulletpowerP1;
                }
            }
            else
            {
                bulletpowerP1 = player1.GetBulletPower();
            }
            firsttime = false;
        }

        //Use fall() method only when THIS gameObject will be SendToDestructioned, since we have a script
        //for each bullet, it works.

        if (col.tag == "BulletP1" || col.tag == "Fire" || col.tag == "Ice")
        {
            if (bulletpowerP2 > bulletpowerP1)
            {
                if (!PSystem)
                {
                    GetComponent<Rigidbody>().velocity = (new Vector3(-0.6f, 0, -1f) * speed * Time.fixedDeltaTime);
                    SetSpeed = true;
                }
                bulletpowerP2 -= bulletpowerP1;               
            }
            else if (bulletpowerP1 > bulletpowerP2)
            {               
                if (!PSystem)
                {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (bulletpowerP1 == bulletpowerP2)
            {
                if (!col.GetComponent<BulletPlayer1>().PSystem)
                {
                    SendToDestruction(col.gameObject);
                }                
                if (!PSystem)
                {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
        else if (col.tag == "Shield")
        {
            if (bulletpowerP2 > player1Shield)
            {
                if (!PSystem)
                {
                    GetComponent<Rigidbody>().velocity = (new Vector3(-0.6f, 0, -1f) * speed * Time.fixedDeltaTime);
                    SetSpeed = true;
                }
                bulletpowerP2 -= player1Shield;                
            }
            else if (player1Shield > bulletpowerP2)
            {                
                if (!PSystem)
                {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (bulletpowerP2 == player1Shield)
            {                
                if (!PSystem)
                {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
        else if (col.tag == "Player")
        {
            //if (col.transform.parent.name != "Player2" || (col.transform.parent.name == "Player2" && gameObject.layer == 10))
            //{
                if (!PSystem)
                {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
            //}
        }
        else if (col.tag == "Bullet")
        {
            if (bulletpowerP2 > player1Shield)
            {
                if (!PSystem)
                {
                    GetComponent<Rigidbody>().velocity = (new Vector3(-0.6f, 0, -1f) * speed * Time.fixedDeltaTime);
                    SetSpeed = true;
                }
                bulletpowerP2 -= player1Shield;                
            }
            else if (player1Shield > bulletpowerP2)
            {
                player1.SetShieldPower(new int[3] { 0, 0, player1Shield - bulletpowerP2 });
                if (!PSystem)
                {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (bulletpowerP2 == player1Shield)
            {
                player1.SetShieldPower(new int[3] { 0, 0, player1Shield - bulletpowerP2 });
                if (!PSystem)
                {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
    public void OnParticleCollision(GameObject col)
    {
        if (col.name == "Ground" || col.tag == "Untagged" || col.tag == "BulletP2")
        {
            return;
        }
        if (col.name == "Child" || col.name == "Child(Clone)")
        {
            col = col.transform.parent.gameObject;
            if(col.GetComponent<BulletPlayer1>().LastCol!=gameObject)
                col.GetComponent<BulletPlayer1>().OnParticleCollision(gameObject);
        }
        if (LastCol == null)
        {
            LastCol = col;
            BeforeLastCol = col;
        }
        else
        {
            if (LastCol == col || (LastCol.tag == "Bullet" && col.tag == "Bullet") || (LastCol.tag == "Shield" && col.tag == "Shield"))
            {
                return;
            }
            else
            {
                if (col != LastCol)
                {
                    BeforeLastCol = LastCol;
                }
                LastCol = col;
            }
        }
        if (col.tag == "DrainingHands" && (gameObject.name == "WaterBubbleBarrage" || gameObject.name == "PoisonousBubble"))
        {
            T = 0.35f;
            Stop = true;
            Watch.Start();
        }
        int Dir = FindDir(gameObject);
        if (col.tag == "BulletP1" && !MateDone && col.GetComponent<BulletPlayer1>().isMate)
        {
            bulletpowerP1 = col.GetComponent<BulletPlayer1>().bulletpowerP1;
            MateDone = true;
        }
        else if (firsttime)
        {
            if (col.tag == "BulletP1" && col.GetComponent<BulletPlayer1>().isMate)
            {
                return;
            }
            if (col.tag == "Shield")
                player1Shield = col.GetComponent<ShieldCol>().ShieldPower[Dir];
            if (col.tag == "BulletP1")
            {
                if (col.GetComponent<BulletPlayer1>().BeforeLastCol == gameObject || col.GetComponent<BulletPlayer1>().BeforeLastCol == null)
                {
                    bulletpowerP1 = player1.GetBulletPower();
                }
                else
                {
                    bulletpowerP1 = col.GetComponent<BulletPlayer1>().bulletpowerP1;
                }
            }
            else
            {
                bulletpowerP1 = player1.GetBulletPower();
            }
            firsttime = false;
        }
        //Use fall() method only when THIS gameObject will be SendToDestructioned, since we have a script
        //for each bullet, it works.

        if (col.tag == "BulletP1" || col.tag == "Fire" || col.tag == "Ice")
        {
            if (bulletpowerP2 > bulletpowerP1)
            {
                if (gameObject.name == "WaterBubbleBarrage" || gameObject.name == "PoisonousBubble")
                {
                    T = 0.35f;
                    Stop = true;
                    Watch.Start();
                }
                bulletpowerP2 -= bulletpowerP1;
            }
            else if (bulletpowerP1 > bulletpowerP2)
            {             
                if (!PSystem)
                {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (bulletpowerP1 == bulletpowerP2)
            {
                if (!PSystem)
                {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
        else if (col.tag == "Shield")
        {
            if (bulletpowerP2 > player1Shield)
            {
                if (gameObject.name == "WaterBubbleBarrage" || gameObject.name == "PoisonousBubble")
                {
                    T = 0.35f;
                    Stop = true;
                    Watch.Start();
                }
                bulletpowerP2 -= player1Shield;
            }
            else if (player1Shield > bulletpowerP2)
            {
                if (!PSystem)
                {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (bulletpowerP2 == player1Shield)
            {
                if (!PSystem)
                {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
        else if (col.tag == "Player")
        {
            if (gameObject.name == "WaterBubbleBarrage" || gameObject.name == "PoisonousBubble")
            {
                T = 0.75f;
                Stop = true;
                Watch.Start();
            }
        }
        else if (col.tag == "Bullet")
        {
            if (bulletpowerP2 > player1Shield)
            {
                if (gameObject.name == "WaterBubbleBarrage" || gameObject.name == "PoisonousBubble")
                {
                    T = 0.75f;
                    Stop = true;
                    Watch.Start();
                }
                bulletpowerP2 -= player1Shield;
            }
            else if (player1Shield > bulletpowerP2)
            {
                player1.SetShieldPower(new int[3] { 0, 0, player1Shield - bulletpowerP2 });
                if (!PSystem)
                {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else if (col.name != "ToxicRing")
                {
                    if (col.GetComponent<Sealingscript>().IsAnim)
                    {
                        player1.GetComponent<Animator>().SetInteger("ID", -1);
                    }
                    gameObject.SetActive(false);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (bulletpowerP2 == player1Shield)
            {
                player1.SetShieldPower(new int[3] { 0, 0, player1Shield - bulletpowerP2 });
                if (!PSystem)
                {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }   
    private void OnBecameInvisible()
    {
        if (!PSystem)
        {
            SendToDestruction(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    private void OnDisable()
    {
        if (IsAnim)
        {
            player2.GetComponent<Animator>().SetInteger("ID", -1);
        }
    }
    private void SendToDestruction(GameObject Object)
    {
        if (Object != null)
        {
            GameMaster.ObjectsToDestroy.Add(Object);
            Object.SetActive(false);
        }
    }
    private int FindDir(GameObject col)
    {
        if (col.layer == 13 || col.layer == 16)
            return 2;
        else if (col.layer == 12 || col.layer == 15)
            return 0;
        else if (col.layer == 14 || col.layer == 17 || col.layer == 18 || col.layer == 19)
            return 1;
        else
            return -1;
    }
}