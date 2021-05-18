using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assist : MonoBehaviour
{
    private int AttPow;
    public Animator Anim;
    private FallInPieces Fall;
    private GameObject[] Children = new GameObject[3];

    void Start()
    {
        Anim = gameObject.GetComponent<Animator>();
        Fall = gameObject.GetComponent<FallInPieces>();
        for (int i = 0; i < 3; i++)
        {
            Children[i] = gameObject.transform.GetChild(i).gameObject;
        }
    }
    public void Initialize(int AttP)
    {
        AttPow = AttP;
    }
    public int GetAttPow()
    {
        return AttPow;
    }
    public IEnumerator SetFalse()
    {
        yield return new WaitForSeconds(2f);
        foreach(GameObject Child in Children)
        {
            Child.SetActive(false);
        }      
        Anim.SetInteger("ID", -1);
        yield return new WaitForSeconds(0.1f);
        foreach (GameObject Child in Children)
        {
            Child.SetActive(true);
        }
        gameObject.SetActive(false);
    }

}
