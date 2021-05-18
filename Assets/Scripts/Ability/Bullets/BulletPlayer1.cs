using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class BulletPlayer1 : MonoBehaviour
{
    public bool lobbed;
    public int speed;
    public int bulletpowerP1;
    int bulletpowerP2;
    Shape_Player player1;
    Shape_Player player2;
    FallInPieces objectToFall;
    bool firsttime = true;
    int player2Shield;
    public bool isMate;
    public MateCode Mate;
    private bool MateDone = false;
    public bool PSystem;
    public GameObject LastCol;
    ParticleSystem.Particle[] Pars;
    ParticleSystem Psys;
    private bool Stop = false;
    private Vector3 Vel = Vector3.zero;
    private Vector3 Pos = Vector3.zero;
    private float RemLife = 0;
    Stopwatch Watch = new Stopwatch();
    private float T;
    public bool IsAnim;
    public GameObject InstantiateCollider;
    private bool First = true;
    public GameObject[] Colliders;
    public GameObject AirCannonMir;
    public GameObject IceShardsMir;
    public GameObject FireLaserMir;
    public GameObject ToxicShotMir;
    public GameObject BeforeLastCol = null;
    private Collider[] MyCols;
    private bool SetSpeed = false;
    private void Awake()
    {
        player1 = GameObject.Find("Player1").GetComponent<Shape_Player>();
        player2 = GameObject.Find("Player2").GetComponent<Shape_Player>();
        if (isMate)
        {
            Mate = player1.gameObject.transform.Find("Mate").GetComponent<MateCode>();
            GetComponent<BoxCollider>().isTrigger = true;
            GetComponent<Rigidbody>().useGravity = false;

            GetComponent<Rigidbody>().velocity = (new Vector3(0.7f, 0, 1f) * speed * Time.fixedDeltaTime);
        }
        else if (!lobbed && !PSystem)
        {
            GetComponent<BoxCollider>().isTrigger = true;
            GetComponent<Rigidbody>().useGravity = false;

            GetComponent<Rigidbody>().velocity = (new Vector3(0.7f, 0, 1f) * speed * Time.fixedDeltaTime);
        }
        else if(!PSystem)
        {
            gameObject.layer = 12;
            if (player2.GetIdOfAnimUsed() == 4 || player2.GetIdOfAnimUsed() == 3)
                speed = 6;  //Make him miss when we should
            else
                speed = 5;
            GetComponent<Rigidbody>().mass = 1.5f;
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<BoxCollider>().isTrigger = false;
            GetComponent<Rigidbody>().AddForce(new Vector3(35f, 122.5f, 50f) * speed);
            Destroy(gameObject, 2f);
        }
        if (isMate)
        {
            bulletpowerP1 = Mate.GetBulletPow();
        }
        else
        {
            bulletpowerP1 = player1.GetBulletPower();
        }
        // Script that instantiates 4 mini cubes in random directions with gravity
        if (!PSystem)
        {
            objectToFall = gameObject.GetComponent<FallInPieces>();
        }
        else
        {
            Psys = gameObject.GetComponent<ParticleSystem>();
            Pars = new ParticleSystem.Particle[Psys.main.maxParticles];
            Colliders = new GameObject[Pars.Length];
        }
        MyCols = gameObject.GetComponents<Collider>();
    }
    //Both Functions For fixing the Physics for the WaterBarrage
    private void OnEnable()
    {
        LastCol = null;
        BeforeLastCol = null;
        MateDone = false;
        SetSpeed = false;
        if (PSystem)
        {
            bulletpowerP1 = player1.GetBulletPower();
            firsttime = true;
            Stop = false;
            Watch = new Stopwatch();
            Vel = Vector3.zero;
            Pos = Vector3.zero;
            RemLife = 0;
            First = true;
        }
        foreach (Collider C in MyCols)
            C.enabled = true;
        if (!PSystem || gameObject.name == "IceDisk" || gameObject.name == "FlamesParticleEffect")
            if (player2.GetTriggerCollision())
                foreach (Collider C in MyCols)
                    if (!C.isTrigger)
                        C.enabled = false;
    }

    private void FixedUpdate()
    {
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().velocity = (new Vector3(0.7f, 0, 1f) * speed * Time.fixedDeltaTime);

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
        else if ((gameObject.name == "WaterBubbleBarrage" || gameObject.name == "PoisonousBubble") && Stop)
        {
            long Secs = Watch.ElapsedMilliseconds;
            float Life = ((RemLife * 1000 - Secs) / 1000) - T;
            if (Life < 0)
            {
                Life = 0;
            }
            Vector3 Val = Vector3.zero;
            if(gameObject.name == "WaterBubbleBarrage")
            {
                 Val = new Vector3(3.5f, 0, 4.6f);
            }
            else if (gameObject.name == "PoisonousBubble")
            {
                 Val = new Vector3(1.6f, 0, 2.1f);
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
            if(Vel!= Vector3.zero)
            {
                Pars[0].position = new Vector3(Pos.x + ((Vel.x * Secs) / 1000), Pos.y, Pos.z + ((Vel.z * Secs) / 1000));
            }
            Pars[0].remainingLifetime = Life;
            Psys.SetParticles(Pars, 1);
        }
        if (PSystem && gameObject.name != "Fireball" && gameObject.name != "IceDisk" && gameObject.name!= "FlamesParticleEffect") 
        {
            Psys.GetParticles(Pars);
            int i = 0;
            if (First)
            {
                foreach (ParticleSystem.Particle P in Pars)
                {
                    Colliders[i] = Instantiate(InstantiateCollider, P.position, Psys.transform.rotation, Psys.transform);
                    Colliders[i].layer = Colliders[i].transform.parent.gameObject.layer;
                    i++;
                    if (i > 40)
                    {
                        First = false;
                        return;
                    }
                }
                First = false;
            }
            else
            {
                foreach(GameObject G in Colliders)
                {
                    SendToDestruction(G);
                }
                foreach (ParticleSystem.Particle P in Pars)
                {
                    Quaternion Q = new Quaternion(P.rotation3D.x, P.rotation3D.y, P.rotation3D.z, 1);
                    Colliders[i] = Instantiate(InstantiateCollider, P.position, Q,Psys.transform);
                    Colliders[i].layer = Colliders[i].transform.parent.gameObject.layer;
                    if (gameObject.name == "PoisonousBubble" || gameObject.name == "WaterBubbleBarrage")
                    {
                        Colliders[i].transform.position = P.position;
                    }
                    else if (gameObject.name == "ToxicShot" && (player2.GetIdOfAnimUsed()!= 38 && player2.GetIdOfAnimUsed()!= 46))
                    {
                        Colliders[i].transform.localPosition = new Vector3(P.position.x-1,P.position.y-4,P.position.z);
                    }
                    else if (gameObject.name == "IceShardsAttackStraight")
                    {
                        Colliders[i].transform.localPosition = new Vector3(P.position.x, P.position.y, P.position.z + 1.7f);
                    }
                    else if(gameObject.name == "FireLaser")
                    {
                        Colliders[i].transform.localPosition = new Vector3(P.position.x, P.position.y, P.position.z + 4f);
                    }
                    else
                    {
                        Colliders[i].transform.localPosition = P.position;
                    }
                    i++;
                    if (i > 20)
                    {
                        return;
                    }
                }
            }
        }
        /*
        if(SetSpeed && !lobbed)
                GetComponent<Rigidbody>().velocity = (new Vector3(0.7f, 0, 1f) * speed * Time.deltaTime);*/       
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.name == "Ground" || col.tag == "Untagged" || col.tag == "BulletP1")
        {
            return;
        }
        if (!col.isTrigger && col.tag!="Shield")
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
        int dir = FindDir(gameObject);
        if (col.tag == "BulletP2" && !MateDone && col.GetComponent<BulletPlayer2>().isMate)
        {
            bulletpowerP2 = col.GetComponent<BulletPlayer2>().bulletpowerP2;
            MateDone = true;
        }
        else if (firsttime)
        {
            if (col.tag == "BulletP2" && col.GetComponent<BulletPlayer2>().isMate)
            {
                return;
            }
            if(col.tag == "Shield")
                player2Shield = col.GetComponent<ShieldCol>().ShieldPower[dir];
            if (col.tag == "BulletP2")
            {
                if (col.GetComponent<BulletPlayer2>().BeforeLastCol == gameObject || col.GetComponent<BulletPlayer2>().BeforeLastCol == null)
                {
                    bulletpowerP2 = player2.GetBulletPower();
                }
                else
                {
                    bulletpowerP2 = col.GetComponent<BulletPlayer2>().bulletpowerP2;
                }
            }
            else
            {
                bulletpowerP2 = player2.GetBulletPower();
            }
            firsttime = false;
        }
        //Use fall() method only when THIS gameObject will be SendToDestructioned, since we have a script
        //for each bullet, it works.

        if (col.tag == "BulletP2" || col.tag == "Fire" || col.tag == "Ice")
        {
            if (bulletpowerP1 > bulletpowerP2)
            {
                if (!PSystem)
                {
                    GetComponent<Rigidbody>().velocity = (new Vector3(0.7f, 0, 1f) * speed * Time.fixedDeltaTime);
                    SetSpeed = true;
                }
                bulletpowerP1 -= bulletpowerP2;                
            }
            else if (bulletpowerP2 > bulletpowerP1)
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
            else if (bulletpowerP2 == bulletpowerP1)
            {                
                if (!col.GetComponent<BulletPlayer2>().PSystem)
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
            if (bulletpowerP1 > player2Shield)
            {
                if (!PSystem)
                {
                    GetComponent<Rigidbody>().velocity = (new Vector3(0.7f, 0, 1f) * speed * Time.fixedDeltaTime);
                    SetSpeed = true;
                }
                bulletpowerP1 -= player2Shield;               
            }
            else if (bulletpowerP1 < player2Shield)
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
            else if (bulletpowerP1 == player2Shield)
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
            //if(col.transform.parent.name != "Player1" || (col.transform.parent.name == "Player1" && gameObject.layer == 11))
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
            if (bulletpowerP1 > player2Shield)
            {
                if (!PSystem)
                {
                    GetComponent<Rigidbody>().velocity = (new Vector3(0.7f, 0, 1f) * speed * Time.fixedDeltaTime);
                    SetSpeed = true;
                }
                bulletpowerP1 -= player2Shield;                
            }
            else if (bulletpowerP1 < player2Shield)
            {
                player2.SetShieldPower(new int[3] { 0, 0, player2Shield - bulletpowerP1 });
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
            else if (bulletpowerP1 == player2Shield)
            {
                player2.SetShieldPower(new int[3] { 0, 0, player2Shield - bulletpowerP1 });
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
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().velocity = (new Vector3(0.7f, 0, 1f) * speed * Time.fixedDeltaTime);
    }
    public void OnParticleCollision(GameObject col)
    {
        if (col.name == "Ground" || col.tag == "Untagged" || col.tag == "BulletP1")
        {
            return;
        }
        if (col.name == "Child" || col.name == "Child(Clone)")
        {
            col = col.transform.parent.gameObject;
        }
        if(LastCol == null)
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
            T = 0.75f;
            Stop = true;
            Watch.Start();
        }
        int dir = FindDir(gameObject);
        if (col.tag == "BulletP2" && !MateDone && col.GetComponent<BulletPlayer2>().isMate)
        {
            bulletpowerP2 = col.GetComponent<BulletPlayer2>().bulletpowerP2;
            MateDone = true;
        }
        else if (firsttime)
        {
            if (col.tag == "BulletP2" && col.GetComponent<BulletPlayer2>().isMate)
            {
                return;
            }
            if (col.tag == "Shield")
                player2Shield = col.GetComponent<ShieldCol>().ShieldPower[dir];
            if (col.tag == "BulletP2")
            {
                if (col.GetComponent<BulletPlayer2>().BeforeLastCol == gameObject || col.GetComponent<BulletPlayer2>().BeforeLastCol == null)
                {
                    bulletpowerP2 = player2.GetBulletPower();
                    /*if (col.GetComponent<BulletPlayer2>().BeforeLastCol == null)
                        //Debug.Log("Null");
                    else
                        //Debug.Log(col.GetComponent<BulletPlayer2>().BeforeLastCol.name);*/
                }
                else
                {
                    bulletpowerP2 = col.GetComponent<BulletPlayer2>().bulletpowerP2;
                }
            }
            else
            {
                bulletpowerP2 = player2.GetBulletPower();
            }
            firsttime = false;
        }
        //Use fall() method only when THIS gameObject will be destroyed, since we have a script
        //for each bullet, it works.

        if (col.tag == "BulletP2" || col.tag == "Fire" || col.tag == "Ice")
        {
            if (bulletpowerP1 > bulletpowerP2)
            {
                if (gameObject.name == "WaterBubbleBarrage" || gameObject.name == "PoisonousBubble")
                {
                    T = 0.35f;
                    Stop = true;
                    Watch.Start();
                }
                bulletpowerP1 -= bulletpowerP2;                
            }
            else if (bulletpowerP2 > bulletpowerP1)
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
            else if (bulletpowerP2 == bulletpowerP1)
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
            if (bulletpowerP1 > player2Shield)
            {
                if (gameObject.name == "WaterBubbleBarrage" || gameObject.name == "PoisonousBubble")
                {
                    Stop = true;
                    Watch.Start();
                }
                bulletpowerP1 -= player2Shield;                
            }
            else if (bulletpowerP1 < player2Shield)
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
            else if (bulletpowerP1 == player2Shield)
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
            if (bulletpowerP1 > player2Shield)
            {
                if (gameObject.name == "WaterBubbleBarrage" || gameObject.name == "PoisonousBubble")
                {
                    T = 0.75f;
                    Stop = true;
                    Watch.Start();
                }
                bulletpowerP1 -= player2Shield;                
            }
            else if (bulletpowerP1 < player2Shield)
            {
                player2.SetShieldPower(new int[3] { 0, 0, player2Shield - bulletpowerP1 });
                if (!PSystem)
                {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else if(col.name != "ToxicRing") //??
                {
                    if (col.GetComponent<Sealingscript>().IsAnim)
                    {
                        player2.GetComponent<Animator>().SetInteger("ID", -1);
                    }
                    gameObject.SetActive(false);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (bulletpowerP1 == player2Shield)
            {
                player2.SetShieldPower(new int[3] { 0, 0, player2Shield - bulletpowerP1 });
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
            player1.GetComponent<Animator>().SetInteger("ID", -1);
        }
        if (PSystem)
        {
            foreach(GameObject G in Colliders)
            {
                SendToDestruction(G);
            }
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