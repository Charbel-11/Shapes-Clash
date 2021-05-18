using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesCol : MonoBehaviour
{
    public static bool firsttime;
    int player2Shield;
    int bulletpowerP1;
    int bulletpowerP2;
    Shape_Player player1;
    Shape_Player player2;
    private void Awake()
    {
        player1 = GetComponentInParent<Shape_Player>();
        if (transform.parent.name == "Player1")
        {
            player2 = GameObject.Find("Player2").GetComponent<Shape_Player>();
        }
        else
            player2 = GameObject.Find("Player1").GetComponent<Shape_Player>();
        firsttime = true;
    }
    private void OnTriggerEnter(Collider col)
    {
        if (firsttime)
        {
            player2Shield = player2.GetDefensePower();
            bulletpowerP1 = player1.GetBulletPower();
            bulletpowerP2 = player2.GetBulletPower();
            firsttime = false;
        }
        //Use fall() method only when THIS gameObject will be destroyed, since we have a script
        //for each bullet, it works
        if (col.tag == "Fire" || col.tag == "Ice")
        {
            if (bulletpowerP1 > bulletpowerP2)
            {
                bulletpowerP1 -= bulletpowerP2;
            }
            else if (bulletpowerP2 > bulletpowerP1)
            {
                gameObject.SetActive(false);
                GetComponentInParent<Animator>().SetInteger("ID", -1);
            }
            else if (bulletpowerP2 == bulletpowerP1)
            {
                gameObject.SetActive(false);
                GetComponentInParent<Animator>().SetInteger("ID", -1);
            }
        }
        else if (col.tag == "Shield")
        {
            if (bulletpowerP1 > player2Shield)
            {
                bulletpowerP1 -= player2Shield;
            }
            else if (bulletpowerP1 < player2Shield)
            {
                gameObject.SetActive(false);
                GetComponentInParent<Animator>().SetInteger("ID", -1);
            }
            else if (bulletpowerP1 == player2Shield)
            {
                gameObject.SetActive(false);
                GetComponentInParent<Animator>().SetInteger("ID", -1);
            }
        }
        else if (col.transform.root.name == transform.root.name)
        {
            GetComponentInParent<Animator>().SetInteger("ID", -1);
        }
        else if (col.tag == "Bullet")
        {
            gameObject.SetActive(false);
            GetComponentInParent<Animator>().SetInteger("ID", -1);
        }
        else if (col.tag == "BulletP1" || col.tag == "BulletP2")
        {
            if (bulletpowerP1 > bulletpowerP2)
            {
                bulletpowerP1 -= bulletpowerP2;
            }
            else if (bulletpowerP2 > bulletpowerP1)
            {
                gameObject.SetActive(false);
                GetComponentInParent<Animator>().SetInteger("ID", -1);
            }
            else if (bulletpowerP2 == bulletpowerP1)
            {
                gameObject.SetActive(false);
                GetComponentInParent<Animator>().SetInteger("ID", -1);
            }
        }
        player1.SetBulletPower(bulletpowerP1);
    }
}
