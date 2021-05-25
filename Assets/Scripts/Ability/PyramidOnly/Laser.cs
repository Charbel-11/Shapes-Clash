using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {
    private Shape_Player player, otherPlayer;

    private int laserPower;

    private Collider colliderComp;

    private void Awake() {
        player = GetComponentInParent<Shape_Player>();
        colliderComp = GetComponent<BoxCollider>();

        if (transform.parent.name == "Player1")
            otherPlayer = GameObject.Find("Player2").GetComponent<Shape_Player>();
        else
            otherPlayer = GameObject.Find("Player1").GetComponent<Shape_Player>();
    }

    private void OnEnable() {
        colliderComp.enabled = true;
        if (otherPlayer.GetTriggerCollision())
            colliderComp.enabled = false;
    }

    private void OnTriggerEnter(Collider col) {
        if (otherPlayer.escapesFrom(0)) { return; }

        laserPower = player.GetBulletPower();
        int shieldPower = otherPlayer.GetDefensePower();

        if (col.tag == "BulletP1" || col.tag == "BulletP2") {
            int bulletPower = otherPlayer.GetBulletPower();

            if (laserPower > bulletPower) {
                laserPower -= bulletPower;
            }
            else if (laserPower <= bulletPower) {
                this.gameObject.SetActive(false);
                player.GetComponent<Animator>().SetInteger("ID", -1);
            }
        }
        else if (col.tag == "Laser") {
            int otherLaserPower = otherPlayer.GetBulletPower();

            if (laserPower > otherLaserPower) {
                laserPower -= otherLaserPower;
            }
            else if (laserPower <= otherLaserPower) {
                player.GetComponent<Animator>().SetInteger("ID", -1);
                this.gameObject.SetActive(false);
            }
        }
        else if (col.tag == "Shield") {
            if (laserPower > shieldPower) {
                laserPower -= shieldPower;
            }
            else if (laserPower <= shieldPower) {
                player.GetComponent<Animator>().SetInteger("ID", -1);
                Invoke("SetFalse", 0.5f);
            }
        }
    }
    private void OnParticleCollision(GameObject col) {
        if (otherPlayer.escapesFrom(0)) { return; }

        laserPower = player.GetBulletPower();
        int shieldPower = otherPlayer.GetDefensePower();

        if (col.tag == "BulletP1" || col.tag == "BulletP2") {
            if (otherPlayer.getAttack(0) == 0) { return; }
            int bulletPower = otherPlayer.GetBulletPower();

            if (laserPower > bulletPower) {
                col.gameObject.SetActive(false);
                laserPower -= bulletPower;
            }
            else if (laserPower <= bulletPower) {
                player.GetComponent<Animator>().SetInteger("ID", -1);
                Invoke("SetFalse", 0.35f);
            }
        }
        else if (col.tag == "Laser") {
            int otherLaserPower = otherPlayer.GetBulletPower();

            if (laserPower > otherLaserPower) {
                otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                laserPower -= otherLaserPower;
            }
            else if (laserPower <= otherLaserPower) {
                player.GetComponent<Animator>().SetInteger("ID", -1);
                this.gameObject.SetActive(false);
            }
        }
        else if (col.tag == "Shield") {
            if (laserPower > shieldPower) {
                laserPower -= shieldPower;
            }
            else if (laserPower <= shieldPower) {
                player.GetComponent<Animator>().SetInteger("ID", -1);
                Invoke("SetFalse", 0.5f);
            }
        }
    }
    private void SetFalse() {
        gameObject.SetActive(false);
    }
}