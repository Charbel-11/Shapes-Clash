using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MateCode : MonoBehaviour {
    private int LifePoints;
    private int AttPow;
    private int BulletPow = 0;
    public Animator Anim;
    public GameObject Bullet;
    private FallInPieces Fall;
    private int AddAtt = 0;
    private int AddLP = 0;
    public bool Reconnect = false;

    void Start()
    {
        Anim = gameObject.GetComponent<Animator>();
        if(!Reconnect)
            gameObject.SetActive(false);
        Fall = gameObject.GetComponent<FallInPieces>();
    }
    public void SetAdditionals(int AddAttack, int AddLifePoints)
    {
        AddAtt = AddAttack;
        AddLP = AddLifePoints;
    }
    public void Initialize(int LP, int AttP)
    {
        LifePoints = LP + AddLP;
        AttPow = AttP + AddAtt;
        BulletPow += AttPow;
    }
    public void ResetBulletPow()
    {
        BulletPow = AttPow;
    }
    public void IncreaseBulletPow(int add)
    {
        BulletPow += add;
    }
    public int GetBulletPow()
    {
        return BulletPow;
    }
    public int GetAttPow()
    {
        return AttPow;
    }
    public int GetLP()
    {
        return LifePoints;
    }
    public void SetLife(int NewLife)
    {
        LifePoints = NewLife;
        if (LifePoints == 0)
        {
            Fall.Fall();
            Anim.SetInteger("ID", -1);
            Invoke("SetFalse", 1f);
        }
    }
    public void BulletInstantiation()
    {
        var pos = gameObject.transform.Find("position").GetComponent<Transform>();
        GameMaster.ObjectsToDestroy.Add(Instantiate(Bullet, pos.position, pos.rotation));
    }
    private void SetFalse()
    {
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        Reconnect = false;
    }
}
