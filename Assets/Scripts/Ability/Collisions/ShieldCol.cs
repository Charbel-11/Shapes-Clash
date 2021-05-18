using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCol : MonoBehaviour {
    public Shape_Player player;
    public Shape_Player otherPlayer;
    private GameMaster GM;
    private int PlayerNum;
    public bool Psystem;
    private GameObject[][] shields = new GameObject[3][]; // Above,Below,Straight
    public int[] ShieldPower = new int[3];
    private bool firstTime;
    public GameObject LastCol;
    private void Awake()
    {
        string parentName;
        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
        player = GetComponentInParent<Shape_Player>();
        parentName = transform.root.name;       
        if (parentName == "Player1")
        {
            otherPlayer = GameObject.Find("Player2").GetComponent<Shape_Player>();
            PlayerNum = 1;
        }
        else
        {
            otherPlayer = GameObject.Find("Player1").GetComponent<Shape_Player>();
            PlayerNum = 2;
        }
        if (gameObject.name == "ShieldSphere" || gameObject.name == "WaterBubbleShield" || gameObject.name == "ShieldEffect")
            Psystem = true;
        #region Initializing shields
        if(gameObject.name == "GroundOfSteel")
        {
            shields[1] = new GameObject[1] { transform.Find("GroundOfSteelBelow").gameObject };
            shields[2] = new GameObject[1] { transform.Find("GroundOfSteelStraight").gameObject };
        }
        else if(transform.parent.name == "Shield")
        {
            if(shields[0] == null || shields[0].Length == 0)
            {
                shields[0] = new GameObject[6]; shields[2] = new GameObject[9];
                for (int i = 1; i <= 15; i++)
                {
                    if (i <= 9)
                    {
                        shields[2][i - 1] = transform.parent.Find("Cube" + i.ToString()).gameObject;
                    }
                    else
                    {
                        shields[0][i - 10] = transform.parent.Find("Cube" + i.ToString()).gameObject;
                    }
                }
            }           
        }
        else if(gameObject.name == "Waterfall")
        {
            shields[0] = new GameObject[1] { transform.Find("WaterfallAbove").gameObject };
            shields[2] = new GameObject[1] { transform.Find("WaterfallStraight").gameObject };
        }
        else if (gameObject.name == "WaterBubbleShield")
        {
            shields[1] = new GameObject[1] { transform.Find("WaterBubbleShieldBelow").gameObject };
            shields[2] = new GameObject[1] { transform.Find("WaterBubbleShieldStraight").gameObject };
        }
        else if (gameObject.name == "FlameShield")
        {
            shields[0] = new GameObject[1] { transform.Find("FlameShieldAbove").gameObject };
            shields[2] = new GameObject[1] { transform.Find("FlameShieldStraight").gameObject };
        }
        else if (gameObject.name == "ShieldEffect")
        {
            shields[1] = new GameObject[1] { transform.Find("ShieldEffectBelow").gameObject };
            shields[2] = new GameObject[1] { transform.Find("ShieldEffectStraight").gameObject };
        }
        else if (gameObject.name == "ShieldSphere")
        {
            shields[0] = new GameObject[1] { transform.Find("ShieldSphereAbove").gameObject };
            shields[2] = new GameObject[1] { transform.Find("ShieldSphereStraight").gameObject };
        }
        else if (gameObject.name == "DownwardsSpiral")
        {
            shields[1] = new GameObject[1] { transform.Find("DownwardsSpiralBelow").gameObject };
            shields[2] = new GameObject[1] { transform.Find("DownwardsSpiralStraight").gameObject };
        }
        #endregion

    }
    private void OnEnable()
    {
        firstTime = true;
        foreach (GameObject[] Ar in shields)
        {
            if (Ar == null || Ar.Length == 0)
                continue;
            foreach (GameObject G in Ar)
                G.SetActive(true);
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if (LastCol == null)
        {
            LastCol = col.gameObject;
        }
        else
        {
            if (LastCol == col.gameObject || (LastCol.tag == "Bullet" && col.tag == "Bullet"))
            {
                return;
            }
            else
            {
                LastCol = col.gameObject;
            }
        }

        if (Psystem)
            return;
        int BulletPower = otherPlayer.GetBulletPower();
        int Dir = FindDir(col.gameObject);
        if (firstTime)
        {
            player.defense.CopyTo(ShieldPower,0);
            firstTime = false;
        }
        if (col.tag == "Player")
        {
            if (player.gameObject == col.transform.parent || (Dir == 1 && !(gameObject.layer == 18 || gameObject.layer == 19)))
            {
                return;
            }
            if (ShieldPower[Dir] < BulletPower)
            {
                ShieldPower[Dir] = 0;
                if (gameObject.GetComponent<FallInPieces>())
                    gameObject.GetComponent<FallInPieces>().Fall();
                if (gameObject.name == "Waterfall" || transform.parent.name == "Shield")
                {
                    player.GetComponent<Animator>().SetInteger("ID", -1);
                }
                if (transform.parent.name != "Shield")
                    gameObject.SetActive(false);
            }
            else if (ShieldPower[Dir] == BulletPower)
            {
                ShieldPower[Dir] = 0;
                if (gameObject.GetComponent<FallInPieces>())
                    gameObject.GetComponent<FallInPieces>().Fall();
                otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                if (gameObject.name == "Waterfall" || transform.parent.name == "Shield")
                {
                    player.GetComponent<Animator>().SetInteger("ID", -1);
                }
                if (transform.parent.name != "Shield")
                    gameObject.SetActive(false);
            }
            else
            {
                ShieldPower[Dir]-=BulletPower;
                otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
            }
        }

        if (col.tag == "BulletP1" || col.tag == "BulletP2" || col.tag == "Laser" || col.tag == "Bullet")
        {
            if (ShieldPower[Dir] < BulletPower)
            {
                ShieldPower[Dir] = 0;
                if (gameObject.GetComponent<FallInPieces>())
                    gameObject.GetComponent<FallInPieces>().Fall();
                Disable(Dir);
                /*
                if (gameObject.name == "Waterfall" || transform.parent.name == "Shield")
                {
                    player.GetComponent<Animator>().SetInteger("ID", -1);
                }
                if (transform.parent.name != "Shield")
                    gameObject.SetActive(false);*/
            }
            else if (ShieldPower[Dir] == BulletPower)
            {
                ShieldPower[Dir] = 0;
                if (col.tag == "Bullet")
                    otherPlayer.GetComponent<Animator>().SetInteger("ID", -1); //FIX MATE CASE
                if (gameObject.GetComponent<FallInPieces>())
                    gameObject.GetComponent<FallInPieces>().Fall();
                Disable(Dir);
                /*
                if (gameObject.name == "Waterfall" || transform.parent.name == "Shield")
                {
                    player.GetComponent<Animator>().SetInteger("ID", -1);
                }
                if (transform.parent.name != "Shield")
                    gameObject.SetActive(false);*/
            }
            else
            {
                ShieldPower[Dir] -= BulletPower;
                if (col.tag == "Bullet")
                    otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                if (gameObject.GetComponent<FallInPieces>())
                    gameObject.GetComponent<FallInPieces>().Fall();
            }
        }
        
        if(gameObject.name != "Waterfall")
            Invoke("setEnabled", 1f);
    }
    private void OnParticleCollision(GameObject col)
    {
        if(col.tag == "Untagged")
        {
            return;
        }
        if (col.name == "Child" || col.name == "Child(Clone)")
        {
            col = col.transform.parent.gameObject;
        }
        if (LastCol == null)
        {
            LastCol = col;
        }
        else
        {
            if (LastCol == col || (LastCol.tag == "Bullet" && col.tag == "Bullet"))
            {
                return;
            }
            else
            {
                LastCol = col;
            }
        }
        int BulletPower = otherPlayer.GetBulletPower();
        int Dir = FindDir(col.gameObject);
        if (firstTime)
        {
            player.defense.CopyTo(ShieldPower, 0);
            firstTime = false;
        }
        if (col.tag == "Player")
        {
            if (player.gameObject == col.transform.parent || (Dir == 1 && !(gameObject.layer == 18 || gameObject.layer == 19)))
            {
                return;
            }
            if (ShieldPower[Dir] < BulletPower)
            {
                ShieldPower[Dir] = 0;
                if (gameObject.name == "Waterfall" || transform.parent.name == "Shield")
                {
                    player.GetComponent<Animator>().SetInteger("ID", -1);
                }
                if (transform.parent.name != "Shield")
                    gameObject.SetActive(false);
            }
            else if (ShieldPower[Dir] == BulletPower)
            {
                ShieldPower[Dir] = 0;
                otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                if (gameObject.name == "Waterfall" || transform.parent.name == "Shield")
                {
                    player.GetComponent<Animator>().SetInteger("ID", -1);
                }
                if (transform.parent.name != "Shield")
                    gameObject.SetActive(false);
            }
            else
            {
                ShieldPower[Dir] -= BulletPower;
                otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
            }
        }

        if (col.tag == "BulletP1" || col.tag == "BulletP2" || col.tag == "Fire" || col.tag == "Bullet")
        {
            if (ShieldPower[Dir] < BulletPower)
            {
                ShieldPower[Dir] = 0;
                if (col.tag == "Bullet")
                    otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                if (!Psystem)
                    Invoke("setEnabled", 2f);
                if (gameObject.name == "Waterfall" || transform.parent.name == "Shield")
                {
                    player.GetComponent<Animator>().SetInteger("ID", -1);
                }
                if (transform.parent.name != "Shield")
                    gameObject.SetActive(false);
            }
            else if (ShieldPower[Dir] == BulletPower)
            {
                ShieldPower[Dir] = 0;
                if (col.tag == "Bullet")
                    otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                if (!Psystem)
                    Invoke("setEnabled", 2f);
                if (gameObject.name == "Waterfall" || transform.parent.name == "Shield")
                {
                    player.GetComponent<Animator>().SetInteger("ID", -1);
                }
                if (transform.parent.name != "Shield")
                    gameObject.SetActive(false);
            }
            else
            {
                ShieldPower[Dir] -= BulletPower;
                if (col.tag == "Bullet")
                    otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
            }

        }
    }
    void setEnabled()
    {
        //gameObject.SetActive(true);
    }

    private int FindDir(GameObject col)
    {
        if (col.layer == 13 || col.layer == 16 || (col.tag == "Player" && otherPlayer.GetIdOfAnimUsed() != 4))
            return 2;
        else if (col.layer == 12 || col.layer == 15)
            return 0;
        else if (col.layer == 14 || col.layer == 17 || col.layer == 18 || col.layer == 19 || (col.tag == "Player" && otherPlayer.GetIdOfAnimUsed() == 4))
            return 1;
        else
            return -1;
    }
    public void Disable(int Dir)
    {
        foreach (GameObject G in shields[Dir])
            G.SetActive(false);
    }
}
