using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrainingHands : MonoBehaviour {
    //Script attached to the parent of both hands
    public int DrainPP;

    private Shape_Player player;
    private Shape_Player otherPlayer;
    private GameMaster GM;

    //To avoid calling the animation multiple times and lagging
    public bool start;

    private void Start() {
        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();

        player = GetComponentInParent<Shape_Player>();
        if (transform.parent.name == "Player1")
            otherPlayer = GameObject.Find("Player2").GetComponent<Shape_Player>();
        else
            otherPlayer = GameObject.Find("Player1").GetComponent<Shape_Player>();

        start = true;
    }
    private void OnEnable() {
        start = true;
    }
    private void OnTriggerEnter(Collider col) {
        if (col.transform.root.name == gameObject.transform.root.name || col.transform.name == "Ground" || col.tag == "Portal" || col.tag == "Shield")
            return;

        if (player.GetIdOfAnimUsed() == 9) {
            if (player.name == "Player1")
                DrainPP = GM.PPDrain1;
            else
                DrainPP = GM.PPDrain2;

            if (col.tag == "BulletP1" || col.tag == "BulletP2" || col.tag == "Fire" || col.tag == "Ice" || (col.tag == "Player" && (otherPlayer.GetIdOfAnimUsed() == 3 || otherPlayer.GetIdOfAnimUsed() == 17 || otherPlayer.GetIdOfAnimUsed() == 21))) {
                gameObject.SetActive(false);
                GetComponentInParent<Animator>().SetInteger("ID", -1);
            }
            else if (col.tag == "Player" && col.transform.root.name != transform.root.name) {
                if (otherPlayer.GetEscapeType() == "Ultimate")
                    return;
                if (start == true) {
                    start = false;
                    otherPlayer.GetComponent<Animator>().SetTrigger("Drained");
                }
            }
            else if (col.tag == "DrainingHands" && player.GetIdOfAnimUsed() == 9 && otherPlayer.GetIdOfAnimUsed() == 9) {
                int DrainPP2 = col.GetComponent<DrainingHands>().DrainPP;
                if (DrainPP <= DrainPP2) {
                    gameObject.SetActive(false);
                    GetComponentInParent<Animator>().SetInteger("ID", -1);
                }
            }
        }
    }
    private void OnParticleCollision(GameObject col) {
        if (player.GetIdOfAnimUsed() != 9 || col.transform.root.name == gameObject.transform.root.name || col.transform.name == "Ground" || col.tag == "Portal" || col.tag == "Shield")
            return;
        if (col.tag == "BulletP1" || col.tag == "BulletP2" || col.tag == "Fire" || col.tag == "Ice") {
            gameObject.SetActive(false);
            GetComponentInParent<Animator>().SetInteger("ID", -1);
        }
    }

    IEnumerator DrainAfterTime(Collider col) {
        yield return new WaitForSeconds(0.42f);
        col.GetComponentInParent<Animator>().SetTrigger("Drained");
    }
}