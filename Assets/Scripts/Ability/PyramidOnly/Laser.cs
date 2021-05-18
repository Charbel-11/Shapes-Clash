using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {
    private Shape_Player player;
    private Shape_Player otherPlayer;

    private int laserPower;

    public Collider ColliderPar;

    private void Awake()
    {
        player = GetComponentInParent<Shape_Player>();

        if (transform.parent.name == "Player1")
            otherPlayer = GameObject.Find("Player2").GetComponent<Shape_Player>();
        else
            otherPlayer = GameObject.Find("Player1").GetComponent<Shape_Player>();
    }
    
    private void OnEnable()
    {
        ColliderPar.enabled = true;
        if (otherPlayer.GetTriggerCollision())
            ColliderPar.enabled = false;
    }

    private void Update()
    {
        if (player.GetIdOfAnimUsed() == 13 && otherPlayer.GetIdOfAnimUsed() == 13)
        {
            laserPower = player.GetBulletPower();
            int otherLaserPower = otherPlayer.GetBulletPower();

            if (laserPower > otherLaserPower)
            {
                otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
            }
            else if (laserPower == otherLaserPower)
            {
                player.GetComponent<Animator>().SetInteger("ID", -1);
                this.gameObject.SetActive(false);
                otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
            }
            else
            {
                player.GetComponent<Animator>().SetInteger("ID", -1);
                this.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (otherPlayer.escapeFrom[0]) { return; }

        laserPower = player.GetBulletPower();
        int shieldPower = otherPlayer.GetDefensePower();
        //Debug.Log("LaserCol : " + col.name);
        if (col.tag == "BulletP1" || col.tag == "BulletP2")
        {
            int bulletPower = otherPlayer.GetBulletPower();

            if (laserPower > bulletPower)
            {
                col.GetComponent<FallInPieces>().Fall();
                col.gameObject.SetActive(false);
                player.SetBulletPower(laserPower - bulletPower);
            }
            else if(laserPower == bulletPower)
            {
                this.gameObject.SetActive(false);
                player.GetComponent<Animator>().SetInteger("ID", -1);
                col.GetComponent<FallInPieces>().Fall();
                col.gameObject.SetActive(false);
            }
            else
            {
                player.GetComponent<Animator>().SetInteger("ID", -1);
                this.gameObject.SetActive(false);
            }
        }
        else if (col.tag == "Laser")
        {
            int otherLaserPower = otherPlayer.GetBulletPower();

            if (laserPower > otherLaserPower)
            {
                otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                col.gameObject.SetActive(false);
                player.SetBulletPower(laserPower - otherLaserPower);
            }
            else if (laserPower == otherLaserPower)
            {
                player.GetComponent<Animator>().SetInteger("ID", -1);
                this.gameObject.SetActive(false);
                otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                col.gameObject.SetActive(false);
            }
            else
            {
                player.GetComponent<Animator>().SetInteger("ID", -1);
                this.gameObject.SetActive(false);
            }
        }
        else if(col.tag == "Shield")
        {
            if (laserPower > shieldPower)
            {
                laserPower -= shieldPower;
            }
            else if (laserPower <= shieldPower)
            {
                player.GetComponent<Animator>().SetInteger("ID", -1);
                 gameObject.SetActive(false);
            }
        }
    }
    private void OnParticleCollision(GameObject col)
    {
        if (otherPlayer.escapeFrom[0]) { return; }

        laserPower = player.GetBulletPower();
        int shieldPower = otherPlayer.GetDefensePower();
        if (col.tag == "BulletP1" || col.tag == "BulletP2")
        {
            if (otherPlayer.attack[0] == 0) { return; }
            int bulletPower = otherPlayer.GetBulletPower();

            if (laserPower > bulletPower)
            {
                col.gameObject.SetActive(false);
                player.SetBulletPower(laserPower - bulletPower);
            }
            else if (laserPower == bulletPower)
            {
                player.GetComponent<Animator>().SetInteger("ID", -1);
                //col.GetComponent<FallInPieces>().Fall();
                col.gameObject.SetActive(false);
                Invoke("SetFalse", 0.35f);
            }
            else
            {
                ColliderPar.enabled = false;
                player.GetComponent<Animator>().SetInteger("ID", -1);
                Invoke("SetFalse", 0.35f);
            }
        }
        else if (col.tag == "Laser")
        {
            int otherLaserPower = otherPlayer.GetBulletPower();

            if (laserPower > otherLaserPower)
            {
                otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                col.gameObject.SetActive(false);
                player.SetBulletPower(laserPower - otherLaserPower);
            }
            else if (laserPower == otherLaserPower)
            {
                player.GetComponent<Animator>().SetInteger("ID", -1);
                this.gameObject.SetActive(false);
                otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                col.gameObject.SetActive(false);
            }
            else
            {
                player.GetComponent<Animator>().SetInteger("ID", -1);
                this.gameObject.SetActive(false);
            }
        }
        else if (col.tag == "Shield")
        {
            if (laserPower > shieldPower)
            {
                laserPower -= shieldPower;
            }
            else if (laserPower <= shieldPower)
            {
                player.GetComponent<Animator>().SetInteger("ID", -1);
                gameObject.SetActive(false);
            }
        }
    }
    private void SetFalse()
    {
        gameObject.SetActive(false);
    }
}