using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class BulletPlayer : MonoBehaviour {
    Shape_Player player, otherPlayer;

    public bool isMate, isAnim, isPartSystem;
    public bool isP1;   // Must be set for prefab bullets
    public int speed, bulletPower;

    public GameObject[] collidersGO;
    public GameObject AirCannonMir, IceShardsMir, FireLaserMir, ToxicShotMir;
    public GameObject lastCol, beforelastCol = null;
    public GameObject InstantiateCollider;

    public MateCode Mate;

    private string bulletTag, otherPlayerBulletTag;
    private int otherPlayerBulletPower, otherPlayerShield;
    private float T, RemLife = 0, mult;     //check what T is
    private bool firstTime = true, First = true, firstCollision;
    private bool MateDone = false, Stop = false;

    private FallInPieces objectToFall;
    private ParticleSystem.Particle[] particles;
    private ParticleSystem partSystem;
    private Stopwatch watch = new Stopwatch();
    private Collider[] colliders;

    private Vector3 Vel = Vector3.zero;
    private Vector3 Pos = Vector3.zero;

    private void Awake() {
        if (isP1 || transform.root.name == "Player1") {
            player = GameObject.Find("Player1").GetComponent<Shape_Player>();
            otherPlayer = GameObject.Find("Player2").GetComponent<Shape_Player>();
            isP1 = true; mult = 1;
            bulletTag = "BulletP1"; otherPlayerBulletTag = "BulletP2";
        }
        else {
            player = GameObject.Find("Player2").GetComponent<Shape_Player>();
            otherPlayer = GameObject.Find("Player1").GetComponent<Shape_Player>();
            isP1 = false; mult = -1; 
            bulletTag = "BulletP2"; otherPlayerBulletTag = "BulletP1";
        }

        if (!isPartSystem) {
            GetComponent<BoxCollider>().isTrigger = true;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().velocity = (new Vector3(0.7f * mult, 0, 1f * mult) * speed * Time.fixedDeltaTime);
        }

        if (isMate) {
            Mate = player.gameObject.transform.Find("Mate").GetComponent<MateCode>();
            bulletPower = Mate.GetBulletPow();
        }
        else {
            bulletPower = player.GetBulletPower();
        }

        if (!isPartSystem) {
            objectToFall = gameObject.GetComponent<FallInPieces>();
        }
        else {
            partSystem = gameObject.GetComponent<ParticleSystem>();
            particles = new ParticleSystem.Particle[partSystem.main.maxParticles];
            collidersGO = new GameObject[particles.Length];
        }

        colliders = gameObject.GetComponents<Collider>();
    }

    private void OnEnable() {
        lastCol = beforelastCol = null;
        MateDone = false;
        firstCollision = true;

        if (isPartSystem) {
            bulletPower = player.GetBulletPower();
            firstTime = First = true;
            Stop = false;
            watch = new Stopwatch();
            Vel = Vector3.zero;
            Pos = Vector3.zero;
            RemLife = 0;
        }

        foreach (Collider col in colliders) { col.enabled = true; }

        if (!isPartSystem || gameObject.name == "IceDisk" || gameObject.name == "FlamesParticleEffect")
            if (otherPlayer.GetTriggerCollision())     
                foreach (Collider C in colliders)
                    if (!C.isTrigger)
                        C.enabled = false;
    }

    private void FixedUpdate() {
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().velocity = (new Vector3(0.7f * mult, 0, 1f * mult) * speed * Time.fixedDeltaTime);

        if ((gameObject.name == "WaterBubbleBarrage" || gameObject.name == "PoisonousBubble") && !Stop) {
            partSystem.GetParticles(particles);
            if ((Vel == Vector3.zero && Pos == Vector3.zero) || particles[0].animatedVelocity.y == 0) {
                Vel = particles[0].animatedVelocity;
                Pos = particles[0].position;
                RemLife = particles[0].remainingLifetime;
            }
        }
        //        else if ((gameObject.name == "WaterBubbleBarrage" || gameObject.name == "PoisonousBubble") && Stop) {
        else if (Stop) {
            long Secs = watch.ElapsedMilliseconds;
            float Life = ((RemLife * 1000 - Secs) / 1000) - T;
            if (Life < 0) { Life = 0; }
         
            Vector3 Val = Vector3.zero;
            if (gameObject.name == "WaterBubbleBarrage") {
                Val = new Vector3(3.5f * mult, 0, 4.6f * mult);
            }
            else if (gameObject.name == "PoisonousBubble") {
                Val = new Vector3(1.6f * mult, 0, 2.1f * mult);
            }
            if (Life <= 0.15) {
                Vel = Vector3.zero;
            }
            else if (Vel != Val) {
                Vel = Val;
            }
            particles[0].velocity = Vel;
            if (Vel != Vector3.zero) {
                particles[0].position = new Vector3(Pos.x + ((Vel.x * Secs) / 1000), Pos.y, Pos.z + ((Vel.z * Secs) / 1000));
            }
            particles[0].remainingLifetime = Life;
            partSystem.SetParticles(particles, 1);
        }

        if (isP1 && isPartSystem && gameObject.name != "Fireball" && gameObject.name != "IceDisk" && gameObject.name != "FlamesParticleEffect") {
            partSystem.GetParticles(particles);
            int i = 0;
            if (First) {
                foreach (ParticleSystem.Particle P in particles) {
                    collidersGO[i] = Instantiate(InstantiateCollider, P.position, partSystem.transform.rotation, partSystem.transform);
                    collidersGO[i].layer = collidersGO[i].transform.parent.gameObject.layer;
                    i++; if (i > 40) { break; }
                }
                First = false;
            }
            else {
                foreach (GameObject G in collidersGO) {
                    SendToDestruction(G);
                }
                foreach (ParticleSystem.Particle P in particles) {
                    Quaternion Q = new Quaternion(P.rotation3D.x, P.rotation3D.y, P.rotation3D.z, 1);
                    collidersGO[i] = Instantiate(InstantiateCollider, P.position, Q, partSystem.transform);
                    collidersGO[i].layer = collidersGO[i].transform.parent.gameObject.layer;
                    if (gameObject.name == "PoisonousBubble" || gameObject.name == "WaterBubbleBarrage") {
                        collidersGO[i].transform.position = P.position;
                    }
                    else if (gameObject.name == "ToxicShot" && (otherPlayer.GetIdOfAnimUsed() != 38 && otherPlayer.GetIdOfAnimUsed() != 46)) {
                        collidersGO[i].transform.localPosition = new Vector3(P.position.x - 1, P.position.y - 4, P.position.z);
                    }
                    else if (gameObject.name == "IceShardsAttackStraight") {
                        collidersGO[i].transform.localPosition = new Vector3(P.position.x, P.position.y, P.position.z + 1.7f);
                    }
                    else if (gameObject.name == "FireLaser") {
                        collidersGO[i].transform.localPosition = new Vector3(P.position.x, P.position.y, P.position.z + 4f);
                    }
                    else {
                        collidersGO[i].transform.localPosition = P.position;
                    }
                    i++; if (i > 20) { return; }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider col) {
        if (col.name == "Ground" || col.tag == "Untagged" || col.tag == bulletTag) { return; }
        if (!isP1 && !col.isTrigger && col.name == "Child(Clone)") {
            if (firstCollision) {
                OnParticleCollision(col.gameObject);
                firstCollision = false;
            }
            return;
        }
        if (!col.isTrigger && col.tag != "Shield") { return; }
        
        if (lastCol == null) {
            lastCol = col.gameObject;
            beforelastCol = col.gameObject;
        }
        else {
            if (lastCol == col.gameObject || (lastCol.tag == "Bullet" && col.tag == "Bullet")) {
                return;
            }
            else {
                beforelastCol = lastCol;
                lastCol = col.gameObject;
            }
        }

        int dir = FindDir(gameObject);
        if (col.tag == otherPlayerBulletTag && !MateDone && col.GetComponent<BulletPlayer>().isMate) {
            otherPlayerBulletPower = col.GetComponent<BulletPlayer>().bulletPower;
            MateDone = true;
        }
        else if (firstTime) {
            if (col.tag == otherPlayerBulletTag && col.GetComponent<BulletPlayer>().isMate) {
                return;
            }
            if (col.tag == "Shield") {
                otherPlayerShield = col.GetComponent<ShieldCol>().ShieldPower[dir];
            }
            if (col.tag == otherPlayerBulletTag) {
                if (col.GetComponent<BulletPlayer>().beforelastCol == gameObject || col.GetComponent<BulletPlayer>().beforelastCol == null) {
                    otherPlayerBulletPower = otherPlayer.GetBulletPower();
                }
                else {
                    otherPlayerBulletPower = col.GetComponent<BulletPlayer>().bulletPower;
                }
            }
            else {
                otherPlayerBulletPower = otherPlayer.GetBulletPower();
            }
            firstTime = false;
        }

        if (col.tag == otherPlayerBulletTag || col.tag == "Fire" || col.tag == "Ice") {
            if (bulletPower > otherPlayerBulletPower) {
                if (!isPartSystem) { //redundant no?
                    GetComponent<Rigidbody>().velocity = (new Vector3(0.7f * mult, 0, 1f * mult) * speed * Time.fixedDeltaTime);
                }
                bulletPower -= otherPlayerBulletPower;
            }
            else if (otherPlayerBulletPower > bulletPower) {
                if (!isPartSystem) {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else {
                    gameObject.SetActive(false);
                }
            }
            else if (otherPlayerBulletPower == bulletPower) {
                if (!col.GetComponent<BulletPlayer>().isPartSystem) {   //redundant no?
                    SendToDestruction(col.gameObject);
                }
                if (!isPartSystem) {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else {
                    gameObject.SetActive(false);
                }
            }
        }
        else if (col.tag == "Shield") {
            if (bulletPower > otherPlayerShield) {
                if (!isPartSystem) { //again, redundant?
                    GetComponent<Rigidbody>().velocity = (new Vector3(0.7f * mult, 0, 1f * mult) * speed * Time.fixedDeltaTime);
                }
                bulletPower -= otherPlayerShield;
            }
            else if (bulletPower <= otherPlayerShield) {
                if (!isPartSystem) {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else {
                    gameObject.SetActive(false);
                }
            }
        }
        else if (col.tag == "Player") {
            if (!isPartSystem) {
                objectToFall.Fall();
                SendToDestruction(gameObject);
            }
        }
        else if (col.tag == "Bullet") {     //what is just Bullet? why not p1 or p2...
            if (bulletPower > otherPlayerShield) {
                if (!isPartSystem) { //again, redundant?
                    GetComponent<Rigidbody>().velocity = (new Vector3(0.7f * mult, 0, 1f * mult) * speed * Time.fixedDeltaTime);
                }
                bulletPower -= otherPlayerShield;
            }
            else if (bulletPower <= otherPlayerShield) {
                otherPlayer.SetShieldPower(new int[3] { 0, 0, otherPlayerShield - bulletPower });
                if (!isPartSystem) {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else {
                    gameObject.SetActive(false);
                }
            }
        }
        if (GetComponent<Rigidbody>())  //redundant? maybe at collision it loses velocity so this is necessary
            GetComponent<Rigidbody>().velocity = (new Vector3(0.7f * mult, 0, 1f * mult) * speed * Time.fixedDeltaTime); 
    }

    public void OnParticleCollision(GameObject col) {
        if (col.name == "Ground" || col.tag == "Untagged" || col.tag == bulletTag) { return; }

        if (col.name == "Child" || col.name == "Child(Clone)") {
            col = col.transform.parent.gameObject;
            if (!isP1 && col.GetComponent<BulletPlayer>().lastCol != gameObject)
                col.GetComponent<BulletPlayer>().OnParticleCollision(gameObject);
        }

        if (lastCol == null) { lastCol = beforelastCol = col; }
        else {
            if (lastCol == col || (lastCol.tag == "Bullet" && col.tag == "Bullet") || (lastCol.tag == "Shield" && col.tag == "Shield")) {
                return;
            }
            else {
                if (col != lastCol) { beforelastCol = lastCol; }
                lastCol = col;
            }
        }
        if (col.tag == "DrainingHands" && (gameObject.name == "WaterBubbleBarrage" || gameObject.name == "PoisonousBubble")) {
            T = 0.75f;
            Stop = true;
            watch.Start();
        }

        int dir = FindDir(gameObject);
        if (col.tag == otherPlayerBulletTag && !MateDone && col.GetComponent<BulletPlayer>().isMate) {
            otherPlayerBulletPower = col.GetComponent<BulletPlayer>().bulletPower;
            MateDone = true;
        }
        else if (firstTime) {
            if (col.tag == otherPlayerBulletTag && col.GetComponent<BulletPlayer>().isMate) { return; }

            if (col.tag == "Shield")
                otherPlayerShield = col.GetComponent<ShieldCol>().ShieldPower[dir];
            if (col.tag == otherPlayerBulletTag) {
                if (col.GetComponent<BulletPlayer>().beforelastCol == gameObject || col.GetComponent<BulletPlayer>().beforelastCol == null) {
                    otherPlayerBulletPower = otherPlayer.GetBulletPower();
                }
                else {
                    otherPlayerBulletPower = col.GetComponent<BulletPlayer>().bulletPower;
                }
            }
            else {
                otherPlayerBulletPower = otherPlayer.GetBulletPower();
            }
            firstTime = false;
        }

        if (col.tag == otherPlayerBulletTag || col.tag == "Fire" || col.tag == "Ice") {
            if (bulletPower > otherPlayerBulletPower) {
                if (gameObject.name == "WaterBubbleBarrage" || gameObject.name == "PoisonousBubble") {
                    T = 0.35f;
                    Stop = true;
                    watch.Start();
                }
                bulletPower -= otherPlayerBulletPower;
            }
            else if (otherPlayerBulletPower >= bulletPower) {
                if (!isPartSystem) {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else {
                    gameObject.SetActive(false);
                }
            }
        }
        else if (col.tag == "Shield") {
            if (bulletPower > otherPlayerShield) {
                if (gameObject.name == "WaterBubbleBarrage" || gameObject.name == "PoisonousBubble") {
                    T = 0.35f;
                    Stop = true;
                    watch.Start();
                }
                bulletPower -= otherPlayerShield;
            }
            else if (bulletPower <= otherPlayerShield) {
                if (!isPartSystem) {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else {
                    gameObject.SetActive(false);
                }
            }
        }
        else if (col.tag == "Player") {
            if (gameObject.name == "WaterBubbleBarrage" || gameObject.name == "PoisonousBubble") {
                T = 0.75f;
                Stop = true;
                watch.Start();
            }
        }
        else if (col.tag == "Bullet") {
            if (bulletPower > otherPlayerShield) {
                if (gameObject.name == "WaterBubbleBarrage" || gameObject.name == "PoisonousBubble") {
                    T = 0.75f;
                    Stop = true;
                    watch.Start();
                }
                bulletPower -= otherPlayerShield;
            }
            else if (bulletPower < otherPlayerShield) {
                otherPlayer.SetShieldPower(new int[3] { 0, 0, otherPlayerShield - bulletPower });
                if (!isPartSystem) {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else if (col.name != "ToxicRing") //??
                {
                    if (col.GetComponent<Sealingscript>().IsAnim) {
                        otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                    }
                    gameObject.SetActive(false);
                }
                else {
                    gameObject.SetActive(false);
                }
            }
            else if (bulletPower == otherPlayerShield) {
                otherPlayer.SetShieldPower(new int[3] { 0, 0, otherPlayerShield - bulletPower });
                if (!isPartSystem) {
                    objectToFall.Fall();
                    SendToDestruction(gameObject);
                }
                else {
                    gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnBecameInvisible() {
        if (!isPartSystem) { SendToDestruction(gameObject); }
        else { gameObject.SetActive(false); }
    }
    private void OnDisable() {
        if (isAnim) { player.GetComponent<Animator>().SetInteger("ID", -1); }
        if (isP1 && isPartSystem) {
            foreach (GameObject G in collidersGO) {
                SendToDestruction(G);
            }
        }
    }
    private void SendToDestruction(GameObject Object) {
        if (Object != null) {
            GameMaster.ObjectsToDestroy.Add(Object);
            Object.SetActive(false);
        }
    }

    private int FindDir(GameObject col) {
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