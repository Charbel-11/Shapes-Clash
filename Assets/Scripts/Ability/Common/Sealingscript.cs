using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sealingscript : MonoBehaviour {
    public static bool playerHurt, playerEscape;

    FallInPieces objectToFall;
    Shape_Player player, otherPlayer;
    private bool firstTime = true, Go = false;
    private int BulletPower, AttackPower, ShieldPow;
    ParticleSystem Psys;
    ParticleSystem.Particle[] Pars = new ParticleSystem.Particle[1];

    public Vector3 Pos;
    public Vector3[][] Positions;
    public GameObject LastCol;
    public bool IsAnim;

    private void Start() {
        objectToFall = gameObject.GetComponent<FallInPieces>();
        player = GetComponentInParent<Shape_Player>();
        if (player.gameObject.name == "Player1") {
            otherPlayer = GameObject.Find("Player2").GetComponent<Shape_Player>();
        }
        else {
            otherPlayer = GameObject.Find("Player1").GetComponent<Shape_Player>();
        }

        if (gameObject.name == "WaterCage") {
            Psys = GetComponent<ParticleSystem>();
            if (gameObject.layer == 10 || gameObject.layer == 12 || gameObject.layer == 13 || gameObject.layer == 14 || gameObject.layer == 18) {
                Pos = new Vector3(0f, 0.7f, 0f);
            }
            else {
                Pos = new Vector3(0f, 1.4f, 0f);
            }
        }
        else if (gameObject.name == "ToxicRing") {
            Psys = GetComponent<ParticleSystem>();
            Pars = new ParticleSystem.Particle[Psys.main.maxParticles];
            Positions = new Vector3[Pars.Length][];
        }
    }
    private void FixedUpdate() {
        if (gameObject.name == "WaterCage" && Go) {
            Psys.GetParticles(Pars);
            Pars[0].position = Pos;
            Pars[0].velocity = Vector3.zero;
            Psys.SetParticles(Pars, 1);
        }
        else if (gameObject.name == "ToxicRing" && !Go) {
            int i = 0;
            Psys.GetParticles(Pars);
            foreach (ParticleSystem.Particle P in Pars) {
                Positions[i] = new Vector3[3];
                Positions[i][0] = Pars[i].position;
                Positions[i][1] = Pars[i].velocity;
                Positions[i][2] = Pars[i].angularVelocity3D;
                i++;
            }
        }
        else if (gameObject.name == "ToxicRing" && Go) {
            int i = 0;
            Psys.GetParticles(Pars);
            foreach (ParticleSystem.Particle P in Pars) {
                Pars[i].position = Positions[i][0];
                Pars[i].velocity = Positions[i][1];
                Pars[i].angularVelocity3D = Positions[i][2];
                i++;
            }
            Psys.SetParticles(Pars, Pars.Length);
        }
    }

    private void OnTriggerEnter(Collider col) {
        if (firstTime) {
            AttackPower = otherPlayer.GetAttackPower();
            BulletPower = otherPlayer.GetBulletPower();
            ShieldPow = player.GetDefensePower();
            firstTime = false;
        }

        if (LastCol == null) {
            if (col.name == "Child" || col.name == "Child(Clone)") {
                LastCol = col.transform.parent.gameObject;
            }
            else {
                LastCol = col.gameObject;
            }
        }
        else {
            if (LastCol == col.gameObject || (LastCol.tag == "Bullet" && col.tag == "Bullet") || (col.transform.parent != null && LastCol == col.transform.parent.gameObject)) {
                return;
            }
            else {
                LastCol = col.gameObject;
            }
        }

        if (col.tag == "BulletP2" || col.tag == "BulletP1") {
            if (BulletPower > ShieldPow) {
                disableShield(); ShieldPow = 0;
            }
            else if (BulletPower <= ShieldPow) {
                ShieldPow -= BulletPower;
            }
        }
        else if (col.tag == "Shield") {
            //objectToFall.Fall(); TODO : Implement the col for below shields
        }
        else if (col.tag == "Player") {
            int tempColID = otherPlayer.GetIdOfAnimUsed();
            int tempID = player.GetIdOfAnimUsed();

            //In the case of special atck or/and sealing attack
            if (tempColID != 3 & tempColID != 21 && tempColID != 17 && tempID == 6) {
                playerHurt = true;
                objectToFall.Fall();
                col.GetComponentInParent<Animator>().SetBool("Hit", true);
            }
            else if ((tempColID == 3 || tempColID == 17 || tempColID == 21) && tempID < 100) {
                if (AttackPower > ShieldPow) {
                    disableShield();
                    playerEscape = true;
                }
                else if (AttackPower < ShieldPow) {
                    ShieldPow -= BulletPower;
                    otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                    otherPlayer.SetIdOfAnimUSed(-1);
                }
                else {
                    disableShield();

                    otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                    otherPlayer.SetIdOfAnimUSed(-1);
                }
            }
            else if (tempColID == 4 && tempID < 100) {
                int AttPow = player.GetAttackPower();
                if (gameObject.name == "WaterCage") {
                    col.GetComponent<PlayerCollision>().Collider.enabled = false;
                }
                Go = true;
                if (AttackPower < AttPow) {
                    AttPow -= AttackPower;
                    otherPlayer.GetComponent<Animator>().SetTrigger("Stuck");
                }
                else if (AttackPower == ShieldPow) {
                    otherPlayer.GetComponent<Animator>().SetTrigger("Stuck");
                    objectToFall.Fall();
                    player.GetComponent<Animator>().SetInteger("ID", -1);
                    AttPow = 0;
                    AttackPower = 0;
                }
                else {
                    AttackPower -= AttPow;
                    AttPow = 0;
                    objectToFall.Fall();
                    player.GetComponent<Animator>().SetInteger("ID", -1);
                }
            }
        }
        else if (col.tag == "Bullet") {
            if (player.attack[1] <= otherPlayer.attack[1])
                if (player is Cube_Player || player is Sphere_Player)
                    player.GetComponent<Animator>().SetInteger("ID", -1);
                else
                    gameObject.SetActive(false);
        }
        else if (col.tag == "Fountain") {
            if (player.attack[1] <= otherPlayer.attack[1]) {
                if (player is Cube_Player || player is Sphere_Player)
                    player.GetComponent<Animator>().SetInteger("ID", -1);
                else
                    gameObject.SetActive(false);
                if (player.attack[1] == otherPlayer.attack[1])
                    col.gameObject.SetActive(false);
            }
            else
                col.gameObject.SetActive(false);
        }
    }
    private void OnParticleCollision(GameObject col) {
        if (LastCol == null) {
            if (col.name == "Child" || col.name == "Child(Clone)") {
                LastCol = col.transform.parent.gameObject;
            }
            else {
                LastCol = col.gameObject;
            }
        }
        else {
            if (LastCol == col.gameObject || (LastCol.tag == "Bullet" && col.tag == "Bullet") || (col.transform.parent != null && LastCol == col.transform.parent.gameObject)) {
                return;
            }
            else {
                LastCol = col.gameObject;
            }
        }
        if (col.name == "ProtectiveEarth1" || col.name == "ProtectiveEarth2") {
            Go = true;
            return;
        }

        if (firstTime) {
            AttackPower = otherPlayer.GetAttackPower();
            BulletPower = otherPlayer.GetBulletPower();
            ShieldPow = player.GetDefensePower();
            firstTime = false;
        }
        if (col.tag == "BulletP2" || col.tag == "BulletP1") {
            Go = true;
            ShieldPow -= BulletPower;
            if (ShieldPow < 0) { disableShield(); }
        }
        else if (col.tag == "Player") {
            int tempColID = otherPlayer.GetIdOfAnimUsed();
            int tempID = player.GetIdOfAnimUsed();
            //In the case of special atck or/and sealing attack
            if (tempColID != 3 && tempColID != 21 && tempColID != 17 && tempColID != 4 && tempID == 6) {
                playerHurt = true;
                col.GetComponentInParent<Animator>().SetBool("Hit", true);
            }
            else if ((tempColID == 3 || tempColID == 17 || tempColID == 21) && tempID < 100) {
                if (gameObject.name == "WaterCage") {
                    col.GetComponent<PlayerCollision>().Collider.enabled = false;
                }
                Go = true;
                if (AttackPower < ShieldPow) {
                    ShieldPow -= AttackPower;
                    otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                }
                else if (AttackPower == ShieldPow) {
                    otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                }
            }
            else if (tempColID == 4 && tempID < 100) {
                int AttPow = player.GetAttackPower();
                if (gameObject.name == "WaterCage") {
                    col.GetComponent<PlayerCollision>().Collider.enabled = false;
                }
                Go = true;
                if (AttackPower < AttPow) {
                    AttPow -= AttackPower;
                    otherPlayer.GetComponent<Animator>().SetTrigger("Stuck");
                }
                else if (AttackPower == ShieldPow) {
                    otherPlayer.GetComponent<Animator>().SetTrigger("Stuck");
                    player.GetComponent<Animator>().SetInteger("ID", -1);
                    AttPow = AttackPower = 0;
                }
                else {
                    AttackPower -= AttPow;
                    AttPow = 0;
                    player.GetComponent<Animator>().SetInteger("ID", -1);
                }
            }
        }
        else if (col.tag == "Bullet") {
            if (player.attack[1] <= otherPlayer.attack[1])
                if (player is Cube_Player || player is Sphere_Player)
                    player.GetComponent<Animator>().SetInteger("ID", -1);
                else
                    gameObject.SetActive(false);
        }
        else if (col.tag == "Fountain") {
            if (player.attack[1] <= otherPlayer.attack[1]) {
                if (player is Cube_Player || player is Sphere_Player)
                    player.GetComponent<Animator>().SetInteger("ID", -1);
                else
                    gameObject.SetActive(false);
                if (player.attack[1] == otherPlayer.attack[1])
                    col.SetActive(false);
            }
            else
                col.SetActive(false);
        }
    }

    void disableShield() {
        if (gameObject.name != "WaterCage" && gameObject.name != "ToxicRing") {
            objectToFall.Fall();
            gameObject.SetActive(false);
            Invoke("setEnabled1", 2f);
        }
        else {
            GetComponentInChildren<BoxCollider>().enabled = false;
            Invoke("setEnabled2", 2f);
        }
    }

    void setEnabled1() {
        gameObject.SetActive(true);
    }
    void setEnabled2() {
        GetComponentInChildren<BoxCollider>().enabled = true;
    }
    public void OnEnable() {
        firstTime = true;
        Go = false;
        LastCol = null;
    }
}