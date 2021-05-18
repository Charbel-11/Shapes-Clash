using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterShieldCol : MonoBehaviour
{
    public Shape_Player player;
    public Shape_Player otherPlayer;

    private void Start()
    {
        player = GetComponentInParent<Shape_Player>();
        var parentName = transform.parent.name;
        if (parentName == "Player1")
            otherPlayer = GameObject.Find("Player2").GetComponent<Shape_Player>();
        else
            otherPlayer = GameObject.Find("Player1").GetComponent<Shape_Player>();
    }

    private void OnTriggerEnter(Collider col)
    {
        int BulletPower = otherPlayer.GetBulletPower();
        int ShieldPower = player.GetDefensePower();
        int AttackPower = otherPlayer.GetAttackPower();

        if (col.tag == "Player")
        {
            if (ShieldPower >= AttackPower)
                otherPlayer.gameObject.GetComponent<Animator>().SetBool("Hit", true);
            //else do nothing, the player passes through, it's water
        }

        if (col.tag == "BulletP1" || col.tag == "BulletP2")
        {
            if (ShieldPower <= BulletPower)
            {
                gameObject.SetActive(false);
                player.GetComponent<Animator>().SetInteger("ID", -1);
            }
            else
                player.SetShieldPower(new int[3] { 0, 0, ShieldPower - BulletPower });
        }
        if (col.tag == "Bullet")
        {
            gameObject.SetActive(false);
        }
        if (col.tag == "Fire")
        {
            if (ShieldPower <= BulletPower)
            {
                gameObject.SetActive(false);
                player.GetComponent<Animator>().SetInteger("ID", -1);
            }
            else
                player.SetShieldPower(new int[3] { 0, 0, ShieldPower - BulletPower });
        }
    }
}