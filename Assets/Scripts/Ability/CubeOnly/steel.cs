using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class steel : MonoBehaviour {
    public Shape_Player ThisPlayer;
//    int DefensePower;
    public Shape_Player OtherPlayer;
    FallInPieces ObjectToFall;
    GameObject Ground;
    private void Start()
    {
        ThisPlayer = GetComponentInParent<Shape_Player>();

        if (transform.parent.name == "Player1")
            OtherPlayer = GameObject.Find("Player2").GetComponent<Shape_Player>();
        else
            OtherPlayer = GameObject.Find("Player1").GetComponent<Shape_Player>();

   
//        DefensePower = ThisPlayer.GetShieldPower();
        ObjectToFall = GetComponent<FallInPieces>();
        Ground = GameObject.Find("Ground");
    }

    private void Update()
    {
        //If the opponent used fountain atck, since they are already touching, we can't work with onCol
        if (OtherPlayer.GetIdOfAnimUsed()==19)
        {
            if (ThisPlayer.GetDefensePower() >= OtherPlayer.GetAttackPower())
                OtherPlayer.GetComponent<Animator>().SetBool("Hit", true);
            else
            {
                gameObject.SetActive(false);
                //Maybe add a ground of steel being destroyed animation
            }
        }

    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Bullet")
        {
            ObjectToFall.Fall();
            if (OtherPlayer.GetIdOfAnimUsed() == 6)
            {
                if (OtherPlayer.GetAttackPower() > ThisPlayer.GetDefensePower())
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    OtherPlayer.GetComponent<Animator>().SetInteger("ID", -1);
                }
            }
        }
        if(col.tag == "Player")
        {
            //If he escapes below
            if (OtherPlayer.escapeFrom[0] && OtherPlayer.escapeFrom[2]) { 
                OtherPlayer.GetComponent<Animator>().SetTrigger("Stuck");
            }
            if(OtherPlayer.GetIdOfAnimUsed() == 12)
            {
                Ground.GetComponent<Animation>().Stop();
            }
        }
    }

    public void OnParticleCollision(GameObject col)
    {
        Debug.Log("PP " + col.name + " " + col.tag);
    }
}
