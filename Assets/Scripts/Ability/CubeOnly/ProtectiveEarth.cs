using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectiveEarth : MonoBehaviour {

    //If anything collides that isn't a child of THIS player, all cube/ground are destroyed
    //and the opponent's attack is reduced by 5.

    private Shape_Player player;
    private Shape_Player otherPlayer;
    private GameMaster GM;

    private void Start()
    {
        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
        player = GetComponentInParent<Shape_Player>();

        if (player.gameObject.name == "Player1")
            otherPlayer = GameObject.Find("Player2").GetComponent<Shape_Player>();
        else
            otherPlayer = GameObject.Find("Player1").GetComponent<Shape_Player>();
    }

    private void Update()
    {
        if (otherPlayer.GetIdOfAnimUsed() > 100)
        {
            //Debug.Log("Protective earth allowed " + player.gameObject.name + " to avoid 5 damage");

            if (player.gameObject.name == "Player1")
                GM.protectiveEarthEffectP1 = true;
            else
                GM.protectiveEarthEffectP2 = true;

            foreach (Transform child in gameObject.transform.parent)
            {
                child.GetComponent<FallInPieces>().Fall();
                child.gameObject.SetActive(false);
            }
            gameObject.transform.parent.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Portal") { return; }

        //Don't want to break the effect unecessarily
        if ((player.GetIdOfAnimUsed() == 4 && otherPlayer.getAttack(1) == 0) || (player.GetIdOfAnimUsed() == 3 && otherPlayer.getAttack(2) == 0) || player.GetIdOfAnimUsed() == 202)
            return;

        if (col.gameObject.transform.root.name == "Player1" || col.gameObject.transform.root.name == "Player2")
        {
            if (col.gameObject.transform.root.name != gameObject.transform.root.name)
            {
                //Debug.Log("Protective earth allowed " + player.gameObject.name + " to avoid 5 damage");

                if (player.gameObject.name == "Player1")
                    GM.protectiveEarthEffectP1 = true;
                else
                    GM.protectiveEarthEffectP2 = true;

                destroyRest();
            }
        }
        else
        {
            if (player.gameObject.name == "Player1")
            {
                if (col.tag == "BulletP2" || col.tag == "Fire")
                {
                    //Debug.Log("Protective earth allowed Player 1 to avoid 5 damage");

                    GM.protectiveEarthEffectP1 = true;

                    destroyRest();
                }
            }
            else
            {
                if (col.tag == "BulletP1" || col.tag == "Fire")
                {
                    //Debug.Log("Protective earth allowed Player 2 to avoid 5 damage");

                    GM.protectiveEarthEffectP2 = true;

                    destroyRest();
                }
            }
        }
    }
    private void OnParticleCollision(GameObject col)
    {
        //Don't want to break the effect unecessarily
        if ((player.GetIdOfAnimUsed() == 4 && otherPlayer.getAttack(1) == 0) || (player.GetIdOfAnimUsed() == 3 && otherPlayer.getAttack(2) == 0) || player.GetIdOfAnimUsed() == 202)
            return;

        if (col.gameObject.transform.root.name == "Player1" || col.gameObject.transform.root.name == "Player2")
        {
            if (col.gameObject.transform.root.name != gameObject.transform.root.name)
            {
                //Debug.Log("Protective earth allowed " + player.gameObject.name + " to avoid 5 damage");

                if (player.gameObject.name == "Player1")
                    GM.protectiveEarthEffectP1 = true;
                else
                    GM.protectiveEarthEffectP2 = true;

                destroyRest();
            }
        }
        else
        {
            if (player.gameObject.name == "Player1")
            {
                if (col.tag == "BulletP2" || col.tag == "Fire")
                {
                    //Debug.Log("Protective earth allowed Player 1 to avoid 5 damage");

                    GM.protectiveEarthEffectP1 = true;

                    destroyRest();
                }
            }
            else
            {
                if (col.tag == "BulletP1" || col.tag == "Fire")
                {
                    //Debug.Log("Protective earth allowed Player 2 to avoid 5 damage");

                    GM.protectiveEarthEffectP2 = true;

                    destroyRest();
                }
            }
        }
    }

    private void destroyRest()
    {
        GameObject PE = player.transform.Find("Design").transform.Find("ProtectiveEarth1").gameObject;
        GameObject PE2 = player.transform.Find("ProtectiveEarth2").gameObject;
        foreach(Transform child in PE.transform)
        {
            child.GetComponent<FallInPieces>().Fall();
            child.gameObject.SetActive(false);
        }
        foreach (Transform child in PE2.transform)
        {
            child.GetComponent<FallInPieces>().Fall();
            child.gameObject.SetActive(false);
        }
        PE.SetActive(false);
        PE2.SetActive(false);
    }
}
