using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {
    private Shape_Player player, otherPlayer;

    private Animator playerAnim;
    private int IDOfAbility;
    private int otherPlayerAbilityId;
    public static int AttackPower;
    public static bool isfirsttime, firstime;
    public Collider Collider;

    public static bool firstTimeWaterP1, firstTimeWaterP2;

    private bool Doit = true, isP1;
    public GameObject LastCol;

    private GameMaster GM;
    private string parentName;

    void Start() {
        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();

        firstTimeWaterP1 = true;
        firstTimeWaterP2 = true;

        player = gameObject.GetComponentInParent<Shape_Player>();
        playerAnim = player.GetComponent<Animator>();
        parentName = transform.parent.name;

        if (parentName == "Player1") {
            otherPlayer = GameObject.Find("Player2").GetComponent<Shape_Player>();
            isP1 = true;
        }
        else {
            otherPlayer = GameObject.Find("Player1").GetComponent<Shape_Player>();
            isP1 = false;
        }

        isfirsttime = firstime = true;
        Collider.enabled = true;
    }

    public void updateState() {
        if (isP1)
            otherPlayer = GM.player2;
        else
            otherPlayer = GM.player1;

        IDOfAbility = player.GetIdOfAnimUsed();
        otherPlayerAbilityId = otherPlayer.GetIdOfAnimUsed();

        if (otherPlayerAbilityId == 201) {
            if (firstime) {
                if (IDOfAbility < 100) {
                    firstime = false;
                    Invoke("BlackHoleAnim", 1f);
                }
            }
            else
                playerAnim.SetInteger("ID", -1);
        }
        if (Doit) {
            Collider.enabled = true;
            Doit = false;
            Invoke("ResetIt", 3.25f);
        }
        if (otherPlayer.GetTriggerCollision()) {
            Collider.enabled = false;
            Invoke("ResetIt", 2.5f);
        }
    }

    void BlackHoleAnim() {
        player.GetComponent<Animator>().SetInteger("ID", 20);
    }

    private void OnTriggerEnter(Collider col) {
        if (isP1)
            otherPlayer = GM.player2;
        else
            otherPlayer = GM.player1;

        IDOfAbility = player.GetIdOfAnimUsed();
        otherPlayerAbilityId = otherPlayer.GetIdOfAnimUsed();

        if (IDOfAbility > 100) { return; }
        if (col.name == "Ground" || (col.tag == "BulletP1" && isP1 && otherPlayerAbilityId != 11) || (col.tag == "BulletP2" && !isP1 && otherPlayerAbilityId != 11))
            return;

        if (isP1) { GM.HitFace1 = true; }
        else { GM.HitFace2 = true; }

        if (!col.isTrigger) { return; }

        if (LastCol == null)
            LastCol = col.gameObject;
        else {
            if (LastCol == col.gameObject || (LastCol.tag == "Bullet" && col.tag == "Bullet")) { return; }
            else { LastCol = col.gameObject; }
        }


        if (isfirsttime && (col.tag == "BulletP1" || col.tag == "BulletP2" || col.tag == "Fire")) {
            AttackPower = player.getOverallAttack();
            isfirsttime = false;
        }

        //If the opponent used fountain attack
        if (col.tag == "Fountain" && player.GetIdOfAnimUsed() == 4) {
            AttackPower = player.getOverallAttack();
            int Att2 = otherPlayer.getOverallAttack();
            if (AttackPower > Att2) {
                AttackPower -= Att2;
                col.gameObject.SetActive(false);
                otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
            }
            else if (AttackPower < Att2) {
                AttackPower = 0;
                playerAnim.SetInteger("ID", -1);
            }
            else {
                AttackPower = 0;
                playerAnim.SetInteger("ID", -1);
                otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                col.gameObject.SetActive(false);
            }
        }

        //If Eye of Horus was used the opponent is hit unless he escapes
        //if (col.tag == "Laser" && player.GetEscapeType() != "Below" && player.GetEscapeType() != "Straight")
        //playerAnim.SetBool("Hit", true);
        //If the other player has used tackle and this player hasn't
        //else if (col.tag == "Player" && (otherPlayerAbilityId == 3 || otherPlayerAbilityId == 17 || otherPlayerAbilityId == 21))
        //playerAnim.SetBool("Hit", true);
        //If both player have used tackle or equivalent
        if ((IDOfAbility == 3 || IDOfAbility == 17 || IDOfAbility == 21) && (otherPlayerAbilityId == 3 || otherPlayerAbilityId == 17 || otherPlayerAbilityId == 21) && col.tag == "Player") {
            AttackPower = player.getOverallAttack();
            int AttackPower2 = otherPlayer.getOverallAttack();
            if (AttackPower > AttackPower2) {
                //otherPlayer.GetComponent<Animator>().SetBool("Hit", true);
                otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
            }
            else if (AttackPower2 > AttackPower) {
                //playerAnim.SetBool("Hit", true);
                playerAnim.SetInteger("ID", -1);
            }
            else {
                otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                playerAnim.SetInteger("ID", -1);
            }
        }
        //If the other player has used shadow attack and this player hasn't
        else if ((IDOfAbility != 4 && IDOfAbility < 100) && otherPlayerAbilityId == 4 && col.tag == "Player") {
            //playerAnim.SetBool("Hit", true);
            if (IDOfAbility == 31)
                player.transform.Find("FireLaser").gameObject.SetActive(false);
        }
        else if ((col.tag == "BulletP1" || col.tag == "BulletP2") && IDOfAbility == 4) {
            if (!new List<int> { 14, 17, 18, 19 }.Contains(col.gameObject.layer))
                return;

            int BulletPower = col.GetComponent<BulletPlayer>().bulletPower;
            if (BulletPower == AttackPower) {
                AttackPower = 0;

                otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                col.gameObject.SetActive(false);

                IDOfAbility = -1;
                playerAnim.SetInteger("ID", -1);
            }
            else if (AttackPower > BulletPower) {
                AttackPower -= BulletPower;

                otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                col.gameObject.SetActive(false);
            }
            else if (BulletPower > AttackPower) {
                IDOfAbility = -1;
                playerAnim.SetInteger("ID", -1);
            }
        }
        /*        //If a player is hit by a bullet;  seems useless as we can't satisfy both bulletp1 tag and null bulletp1
                else if ((col.tag == "BulletP1" || col.tag == "BulletP2") && IDOfAbility != 3 && IDOfAbility != 17 && IDOfAbility != 21) {
                    if (col.tag == "BulletP1") {
                        if (col.GetComponent<BulletPlayer1>() == null && col.GetComponent<BulletPlayer2>().isMate)
                            Destroy(col.gameObject);
                    }
                    else {
                        if (col.GetComponent<BulletPlayer2>() == null && col.GetComponent<BulletPlayer1>().isMate)
                            Destroy(col.gameObject);
                    }
                }
        */
        else if (((col.tag == "BulletP1" && !isP1) || (col.tag == "BulletP2" && isP1)) && (IDOfAbility == 3 || IDOfAbility == 17 || IDOfAbility == 21) && otherPlayerAbilityId < 100) {
            int BulletPower = col.GetComponent<BulletPlayer>().bulletPower;
            if (BulletPower == AttackPower) {
                AttackPower = 0;

                if (col.gameObject.name == "IceDisk")
                    otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);

                col.gameObject.SetActive(false);
                playerAnim.SetInteger("ID", -1);
            }
            else if (AttackPower > BulletPower) {
                AttackPower -= BulletPower;

                if (col.gameObject.name == "IceDisk")
                    otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);

                col.gameObject.SetActive(false);
            }
            else if (BulletPower > AttackPower) {
                //if (!col.GetComponent<BulletPlayer1>().isMate)
                //playerAnim.SetBool("Hit", true);
            }
        }
        else if ((col.tag == "Fire") && (IDOfAbility != 3 && IDOfAbility != 17 && IDOfAbility != 21 && IDOfAbility != 4 && IDOfAbility != 7)) {
            playerAnim.SetBool("OnFire", true);
        }
        //If the other player has used a fire ability and this player hasn't
        else if (col.tag == "Fire" && (IDOfAbility == 3 || IDOfAbility == 17 || IDOfAbility == 21)) {
            int BulletPower = otherPlayer.GetBulletPower();
            if (BulletPower == AttackPower) {
                col.gameObject.SetActive(false);
                otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
            }
            else if (AttackPower > BulletPower) {
                AttackPower -= BulletPower;
                col.gameObject.SetActive(false);
                otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
            }
            else if (BulletPower > AttackPower) {
                playerAnim.SetBool("OnFire", true);
            }
        }
        else if (col.tag == "Fire" && (IDOfAbility == 4 || IDOfAbility == 7)) {
            col.gameObject.SetActive(false);
            otherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
        }
        else if (col.tag == "Ice") {
            //playerAnim.SetBool("Hit", true);
        }
    }
    private void OnParticleCollision(GameObject other) {
        if (isP1) { GM.HitFace1 = true; }
        else { GM.HitFace2 = true; }

        if (other.tag == "BulletP1" && player.gameObject.name == "Player1") { return; }
        if (other.tag == "BulletP2" && player.gameObject.name == "Player2") { return; }

        if (other.tag == "Poison Drops")
            return;
        if (other.tag == "Shield" || other.tag == "Bullet") { return; }
        if (player.GetEscapeType() != "" && otherPlayer.GetAttackType() != player.GetEscapeType()) { return; }

        if (other.name == "Child")
            other = other.transform.parent.gameObject;
        if (isfirsttime && (other.tag == "BulletP1" || other.tag == "BulletP2" || other.tag == "Fire")) {
            AttackPower = player.getOverallAttack();
            isfirsttime = false;
        }

        IDOfAbility = player.GetIdOfAnimUsed();
        if ((IDOfAbility == 3 || IDOfAbility == 17 || IDOfAbility == 21) && otherPlayer.GetIdOfAnimUsed() != 24) {
            int AttPow = other.GetComponent<BulletPlayer>().bulletPower; 

            if (AttPow > AttackPower) {
                playerAnim.SetInteger("ID", -1);
                //playerAnim.SetBool("Hit", true);

                other.GetComponent<BulletPlayer>().bulletPower -= AttackPower;
                Collider.enabled = false;
            }
            else if (AttPow < AttackPower) {
                other.SetActive(false);
                AttackPower -= AttPow;
            }
            else {
                other.SetActive(false);
                playerAnim.SetInteger("ID", -1);
            }
        }
        else if (otherPlayer.GetIdOfAnimUsed() != 24) {
            //playerAnim.SetBool("Hit", true);
            Collider.enabled = false;
        }
    }
    public void ResetIt() {
        Collider.enabled = true;
        Doit = true;
    }
}