using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmotesMotherClass : MonoBehaviour
{
    public static bool OK;
    public int ID;
    public Animator Anim;

    private Shape_Player Player;
    private Button But;
    private GameMaster GM;

    private void Start()
    {
        OK = true;
        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();

        if (GameMaster.Online || GameMaster.botOnline)
        {
            Player = GameObject.Find("Player1").GetComponent<Shape_Player>();
        }
        else
        {
            if (gameObject.transform.root.gameObject.name == "Canvas1")
                Player = GameObject.Find("Player1").GetComponent<Shape_Player>();
            else
                Player = GameObject.Find("Player2").GetComponent<Shape_Player>();
        }

        Anim = Player.gameObject.GetComponent<Animator>();
        But = gameObject.GetComponent<Button>();

        if (ID != -1)
        {
            But.transform.GetChild(0).GetComponent<Image>().enabled = true;

            if (Player is Cube_Player)
                But.transform.GetChild(0).GetComponent<Image>().sprite = GetComponentInParent<Emotes>().EmotesCube[ID];
            else if (Player is Pyramid_Player)
                But.transform.GetChild(0).GetComponent<Image>().sprite = GetComponentInParent<Emotes>().EmotesPyr[ID];
            else if (Player is Star_Player)
                But.transform.GetChild(0).GetComponent<Image>().sprite = GetComponentInParent<Emotes>().EmotesStar[ID];
            else if (Player is Sphere_Player)
                But.transform.GetChild(0).GetComponent<Image>().sprite = GetComponentInParent<Emotes>().EmotesSphere[ID];

            But.interactable = true;
        }
        else
        {
            But.transform.GetChild(0).GetComponent<Image>().enabled = false;
            But.interactable = false;
        }
    }
    public void UseEmote()
    {
        Anim.SetInteger("EID", ID);
        Invoke("SetIt", 1.5f);
        transform.parent.parent.GetComponent<Emotes>().Back();
        transform.parent.parent.GetComponent<Emotes>().Controller.GetComponent<Button>().interactable = false;
        if (GameMaster.Online)
        {
            ClientTCP.PACKAGE_Emotes(ID, PlayerPrefs.GetString("Username"));
        }
    }
    private void SetIt()
    {
        Anim.SetInteger("EID", -1);
       if (OK) transform.parent.parent.GetComponent<Emotes>().Controller.GetComponent<Button>().interactable = true;
    }
}