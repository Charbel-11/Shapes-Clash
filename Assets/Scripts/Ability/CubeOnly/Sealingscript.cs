using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sealingscript : MonoBehaviour {
    public static bool playerHurt;
    public static bool playerEscape;

    FallInPieces objectToFall;
    Shape_Player Player;
    Shape_Player OtherPlayer;
    private bool FirstTime = true;
    int BulletPower;
    int AttackPower;
    int ShieldPow;
    ParticleSystem Psys;
    ParticleSystem.Particle[] Pars = new ParticleSystem.Particle[1];
    public Vector3 Pos;
    public Vector3[][] Positions;
    bool Go = false;
    public GameObject LastCol;
    public bool IsAnim;
    private void Start()
    {
        // Script that instantiates 4 mini cubes in random directions with gravity
        objectToFall = gameObject.GetComponent<FallInPieces>();
        Player = GetComponentInParent<Shape_Player>();
        if(Player.gameObject.name == "Player1")
        {
            OtherPlayer = GameObject.Find("Player2").GetComponent<Shape_Player>();
        }
        else
        {
            OtherPlayer = GameObject.Find("Player1").GetComponent<Shape_Player>();
        }
        if(gameObject.name == "WaterCage")
        {
            Psys = GetComponent<ParticleSystem>();
            if(gameObject.layer == 10 || gameObject.layer == 12 || gameObject.layer == 13 || gameObject.layer == 14 || gameObject.layer == 18)
            {
                Pos = new Vector3(0f, 0.7f, 0f);
            }
            else
            {
                Pos = new Vector3(0f, 1.4f, 0f);
            }
        }
        else if(gameObject.name == "ToxicRing")
        {
            Psys = GetComponent<ParticleSystem>();
            Pars = new ParticleSystem.Particle[Psys.main.maxParticles];
            Positions = new Vector3[Pars.Length][];
        }
    }
    private void FixedUpdate()
    {
        if (gameObject.name == "WaterCage" && Go)
        {
            Psys.GetParticles(Pars);
            Pars[0].position = Pos;
            Pars[0].velocity = Vector3.zero;
            Psys.SetParticles(Pars,1);
        }
        else if(gameObject.name == "ToxicRing" && !Go)
        {
            int i = 0;
            Psys.GetParticles(Pars);
            foreach (ParticleSystem.Particle P in Pars)
            {
                Positions[i] = new Vector3[3];
                Positions[i][0] = Pars[i].position;
                Positions[i][1] = Pars[i].velocity;
                Positions[i][2] = Pars[i].angularVelocity3D;
                i++;
            }
        }
        else if(gameObject.name == "ToxicRing" && Go)
        {
            int i = 0;
            Psys.GetParticles(Pars);
            foreach (ParticleSystem.Particle P in Pars)
            {
                Pars[i].position = Positions[i][0];
                Pars[i].velocity = Positions[i][1];
                Pars[i].angularVelocity3D = Positions[i][2];
                i++;
            }
            Psys.SetParticles(Pars, Pars.Length);
        }
    }
    
    private void OnTriggerEnter(Collider col)
    {
        if (FirstTime)
        {
            AttackPower = OtherPlayer.GetAttackPower();
            BulletPower = OtherPlayer.GetBulletPower();
            ShieldPow = Player.GetDefensePower();
            FirstTime = false;
        }       
        if (LastCol == null)
        {
            if (col.name == "Child" || col.name == "Child(Clone)")
            {
                LastCol = col.transform.parent.gameObject;
            }
            else
            {
                LastCol = col.gameObject;
            }
        }
        else
        {
            if (LastCol == col.gameObject || (LastCol.tag == "Bullet" && col.tag == "Bullet") || (col.transform.parent != null && LastCol == col.transform.parent.gameObject))
            {
                return;
            }
            else
            {
                LastCol = col.gameObject;
            }
        }
        if (col.tag == "BulletP2" || col.tag == "BulletP1")
        {
            if (BulletPower > ShieldPow)
            {
                objectToFall.Fall();
                gameObject.SetActive(false);
                Invoke("setEnabled", 2f);
            }
            else if (BulletPower < ShieldPow)
            {
                ShieldPow -= BulletPower;
            }
            /*else
            {
                objectToFall.Fall();
                gameObject.SetActive(false);
                Invoke("setEnabled", 2f);
            }*/
        }
        else if (col.tag == "Shield")
        {
            //objectToFall.Fall(); TODO : Implement the col for below shields
        }
        else if (col.tag == "Player")
        {
            int tempColID = OtherPlayer.GetIdOfAnimUsed();
            int tempID = Player.GetIdOfAnimUsed();

            //In the case of special atck or/and sealing attack
            if (tempColID != 3 & tempColID != 21 && tempColID != 17 && tempID == 6)
            {
                playerHurt = true;
                objectToFall.Fall();
                col.GetComponentInParent<Animator>().SetBool("Hit", true);
            }
            else if ((tempColID == 3 || tempColID == 17 || tempColID == 21) && tempID < 100)
            {
                if (AttackPower > ShieldPow)
                {
                    if (gameObject.name != "WaterCage")
                    {
                        objectToFall.Fall();
                        gameObject.SetActive(false);
                        Invoke("setEnabled", 2f);
                    }

                    playerEscape = true;
                }
                else if (AttackPower < ShieldPow)
                {
                    ShieldPow -= BulletPower;
                    OtherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                    OtherPlayer.SetIdOfAnimUSed(-1);
                }
                else
                {
                    if (gameObject.name != "WaterCage")
                    {
                        objectToFall.Fall();
                        gameObject.SetActive(false);
                        Invoke("setEnabled", 2f);
                    }

                    OtherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                    OtherPlayer.SetIdOfAnimUSed(-1);
                }
            }
            else if (tempColID == 4 && tempID < 100)
            {
                int AttPow = Player.GetAttackPower();
                if (gameObject.name == "WaterCage")
                {
                    col.GetComponent<PlayerCollision>().Collider.enabled = false;
                }
                Go = true;
                if (AttackPower < AttPow)
                {
                    AttPow -= AttackPower;
                    //                    Player.SetAttackPower(AttPow);
                    //                    OtherPlayer.SetAttackPower(0);
                    OtherPlayer.GetComponent<Animator>().SetTrigger("Stuck");
                }
                else if (AttackPower == ShieldPow)
                {
                    OtherPlayer.GetComponent<Animator>().SetTrigger("Stuck");
                    objectToFall.Fall();
                    Player.GetComponent<Animator>().SetInteger("ID", -1);
                    AttPow = 0;
                    AttackPower = 0;
                    //                    Player.SetAttackPower(0);
                    //                    OtherPlayer.SetAttackPower(0);
                }
                else
                {
                    AttackPower -= AttPow;
                    AttPow = 0;
                    //                    Player.SetAttackPower(0);
                    //                    OtherPlayer.SetAttackPower(AttackPower);
                    objectToFall.Fall();
                    Player.GetComponent<Animator>().SetInteger("ID", -1);
                }
            }
        }
        else if (col.tag == "Bullet")
        {
            if (Player.attack[1] <= OtherPlayer.attack[1])
                if (Player is Cube_Player || Player is Sphere_Player)
                    Player.GetComponent<Animator>().SetInteger("ID", -1);
                else
                    gameObject.SetActive(false);
        }
        else if (col.tag == "Fountain")
        {
            if (Player.attack[1] <= OtherPlayer.attack[1])
            {
                if (Player is Cube_Player || Player is Sphere_Player)
                    Player.GetComponent<Animator>().SetInteger("ID", -1);
                else
                    gameObject.SetActive(false);
                if (Player.attack[1] == OtherPlayer.attack[1])
                    col.gameObject.SetActive(false);
            }
            else
                col.gameObject.SetActive(false);
        }
    }
    private void OnParticleCollision(GameObject col)
    {
        if (LastCol == null)
        {
            if (col.name == "Child" || col.name == "Child(Clone)")
            {
                LastCol = col.transform.parent.gameObject;
            }
            else
            {
                LastCol = col.gameObject;
            }
        }
        else
        {
            if (LastCol == col.gameObject || (LastCol.tag == "Bullet" && col.tag == "Bullet") || (col.transform.parent != null && LastCol == col.transform.parent.gameObject))
            {
                return;
            }
            else
            {
                LastCol = col.gameObject;
            }
        }
        if (col.name == "ProtectiveEarth1" || col.name == "ProtectiveEarth2")
        {
            Go = true;
            return;
        }
        if (FirstTime)
        {
            AttackPower = OtherPlayer.GetAttackPower();
            BulletPower = OtherPlayer.GetBulletPower();
            ShieldPow = Player.GetDefensePower();
            FirstTime = false;
        }
        if (col.tag == "BulletP2" || col.tag == "BulletP1")
        {
            Go = true;
            ShieldPow -= BulletPower;
        }
        else if (col.tag == "Player")
        {
            int tempColID = OtherPlayer.GetIdOfAnimUsed();
            int tempID = Player.GetIdOfAnimUsed();
            //In the case of special atck or/and sealing attack
            if (tempColID != 3 && tempColID != 21 && tempColID != 17 && tempColID!=4 && tempID == 6)
            {
                playerHurt = true;
                col.GetComponentInParent<Animator>().SetBool("Hit", true);
            }
            else if ((tempColID == 3 || tempColID == 17 || tempColID == 21) && tempID < 100)
            {
                if (gameObject.name == "WaterCage")
                {
                    col.GetComponent<PlayerCollision>().Collider.enabled = false;
                }
                Go = true;
                if (AttackPower < ShieldPow)
                {
                    ShieldPow -= AttackPower;
                    OtherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                }
                else if(AttackPower == ShieldPow)
                {
                    OtherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                }
            }
            else if (tempColID == 4 && tempID < 100)
            {
                int AttPow = Player.GetAttackPower();
                if (gameObject.name == "WaterCage")
                {
                    col.GetComponent<PlayerCollision>().Collider.enabled = false;
                }
                Go = true;
                if (AttackPower < AttPow)
                {
                    AttPow -= AttackPower;
//                    Player.SetAttackPower(AttPow);
//                    OtherPlayer.SetAttackPower(0);
                    OtherPlayer.GetComponent<Animator>().SetTrigger("Stuck");
                }
                else if (AttackPower == ShieldPow)
                {
                    OtherPlayer.GetComponent<Animator>().SetTrigger("Stuck");
                    Player.GetComponent<Animator>().SetInteger("ID", -1);
                    AttPow = 0;
                    AttackPower = 0;
 //                   Player.SetAttackPower(0);
  //                  OtherPlayer.SetAttackPower(0);
                }
                else
                {
                    AttackPower -= AttPow;
                    AttPow = 0;
//                    Player.SetAttackPower(0);
//                    OtherPlayer.SetAttackPower(AttackPower);
                    Player.GetComponent<Animator>().SetInteger("ID", -1);
                }
            }
        }
        else if(col.tag == "Bullet")
        {
            if (Player.attack[1] <= OtherPlayer.attack[1])
                if (Player is Cube_Player || Player is Sphere_Player)
                    Player.GetComponent<Animator>().SetInteger("ID",-1);
                else
                    gameObject.SetActive(false);
        }
        else if(col.tag == "Fountain")
        {
            if (Player.attack[1] <= OtherPlayer.attack[1])
            {
                if (Player is Cube_Player || Player is Sphere_Player)
                    Player.GetComponent<Animator>().SetInteger("ID", -1);
                else
                    gameObject.SetActive(false);
                if (Player.attack[1] == OtherPlayer.attack[1])
                    col.SetActive(false);
            }
            else
                col.SetActive(false);
        }
    }
    void setEnabled()
    {
        gameObject.SetActive(true);
    }
    public void OnEnable()
    {
        FirstTime = true;
        Go = false;
        LastCol = null;
    }
}
