using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Emotes : MonoBehaviour
{
    public GameObject panel;
    public GameObject E1;
    public GameObject E2;
    public GameObject E3;
    public GameObject E4;
    public Sprite[] EmotesCube, EmotesPyr, EmotesStar, EmotesSphere;
    public GameObject Controller;
    private void Start()
    {
        Back();

        if (transform.name == "EmotesController")
        {
            Shape_Player Player;
            Button But = gameObject.GetComponent<Button>();

            if (GameMaster.Online || GameMaster.botOnline)
            {
                Player = GameObject.Find("Game Manager").GetComponent<GameMaster>().player1;
            }
            else
            {
                if (gameObject.transform.root.gameObject.name == "Canvas1")
                    Player = GameObject.Find("Player1").GetComponent<Shape_Player>();
                else
                    Player = GameObject.Find("Player2").GetComponent<Shape_Player>();
            }

            int idx = 0;
            if (Player is Pyramid_Player)
                idx = 1;
            else if (Player is Star_Player)
                idx = 2;
            else if (Player is Sphere_Player)
                idx = 3;

            But.GetComponent<Image>().color = ShapeConstants.bckdAbEPColor[idx];            
        }
    }
    public void Emote()
    {
        panel.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    public void Back()
    {
        panel.gameObject.SetActive(false);
        Controller.SetActive(true);
        //Invoke("Resetting", 3f);
    }
    private void Resetting()
    {
        E1.GetComponent<EmotesMotherClass>().Anim.SetInteger("EID", -1);
        if (GameMaster.Online)
        {
            ClientTCP.PACKAGE_Emotes(-1);
        }
    }
}