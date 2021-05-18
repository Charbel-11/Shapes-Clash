using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCollision : MonoBehaviour {
    public Shape_Player player;
    public Shape_Player otherPlayer;
    public Camera camerafight;
    private bool animDoneOnce;
    private Animator playerAnim;

    private void Start()
    {
        camerafight = GameObject.Find("FightCamera").GetComponent<Camera>();

        player = GetComponentInParent<Shape_Player>();
        if (this.transform.parent.name == "Player1")
            otherPlayer = GameObject.Find("Player2").GetComponent<Shape_Player>();
        else
            otherPlayer = GameObject.Find("Player1").GetComponent<Shape_Player>();

        animDoneOnce = false;
    }

    private void Update()
    { 
        if (animDoneOnce == false && (camerafight.isActiveAndEnabled == true) && (player.GetIdOfAnimUsed() == 4) && (otherPlayer.GetIdOfAnimUsed() == 4))
        {
            animDoneOnce = true;
            Invoke("Retreat", 0.05f);
            Invoke("SetBoolToFalse", 2f);
        }
    }

    void Retreat()
    {
        playerAnim = GetComponentInParent<Animator>();
        playerAnim.SetTrigger("Retreat");
    }

     void SetBoolToFalse()
    {
        animDoneOnce = false;
    }
}
