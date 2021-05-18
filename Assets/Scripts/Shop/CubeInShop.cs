using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeInShop : MonoBehaviour {
    public GameObject Bullet;
    public GameObject LobbedBullet;
    Animation anim;
    GameObject[] Children;
    FallInPieces[] ObjectsToFall;
    int Childcount;
    public GameObject Earthquake;


    private void Start()
    {
        Childcount = Earthquake.transform.childCount;
        Children = new GameObject[Earthquake.transform.childCount];
        ObjectsToFall = new FallInPieces[Earthquake.transform.childCount];
        for (int i = 0; i < Childcount; i++)
        {
            Children[i] = Earthquake.transform.GetChild(i).gameObject;
            ObjectsToFall[i] = Children[i].GetComponent<FallInPieces>();
        }

    }
    public void doIt(int ID)
    {
        if (ID == 1)
        {
            Invoke("Attack", 0.1f);
        }
        else if (ID == 5 )
        {
            Invoke("Attack", 0.15f);
            Invoke("Attack", 0.25f);
            Invoke("Attack", 0.6f);
            Invoke("Attack", 0.7f);
        }
        else if (ID == 10)
        {
            Invoke("AerialAttack", 0.35f);
        }
        else if (ID == 12 || ID == 202)
        {
            anim = GameObject.Find("Ground").GetComponent<Animation>();
            anim.Play();
            for (int i = 0; i < 5; i++)
            {
                Invoke("Earthquake1", i / 2);
            }
            Invoke("Stop", 2f);
        }
    }

    public void Attack()
    {
        var pos = gameObject.transform.Find("position").GetComponent<Transform>();
        Instantiate(Bullet, pos.position, pos.rotation);
    }
    public void AerialAttack()
    {
        var pos = gameObject.transform.Find("position").GetComponent<Transform>();
        Instantiate(LobbedBullet, pos.position, pos.rotation);
    }


    void Earthquake1()
    {
        for (int i = 0; i < Childcount; i++)
        {
            ObjectsToFall[i].Fall();
        }
    }

    void Stop()
    {
        anim.Stop();
    }
}
