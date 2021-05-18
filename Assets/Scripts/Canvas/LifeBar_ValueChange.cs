using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBar_ValueChange : MonoBehaviour
{
    public Shape_Player Player;
    public Slider thisSlider;
    private Text HPText;
    private GameMaster GM;

    private bool started;

    public void Start()
    {
        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
        thisSlider = gameObject.GetComponent<Slider>();

        if (transform.parent.name == "Switch")
            Player = GameObject.Find("Game Manager").GetComponent<GameMaster>().player12;
        else if (transform.root.name == "Canvas2" && GameMaster.Online && transform.name == "Player1Life")
            Player = GM.player1;
        else if (transform.name == "Player1Life" || transform.name == "MateLife")
            Player = GameObject.Find("Player1").GetComponent<Shape_Player>();
        else
            Player = GM.player2;

        if (Player == null) { enabled = false; started = true; return; }

        if (Player is Cube_Player)
            transform.Find("Fill Area").transform.GetChild(0).GetComponent<Image>().color = new Color(.669f, .516f, 0f);
        else if (Player is Pyramid_Player)
            transform.Find("Fill Area").transform.GetChild(0).GetComponent<Image>().color = new Color(0f, 0f, 1f);
        else if (Player is Star_Player)
            transform.Find("Fill Area").transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 0f, 0f);
        else if (Player is Sphere_Player)
            transform.Find("Fill Area").transform.GetChild(0).GetComponent<Image>().color = new Color(0f, 1f, 0f);

        if (transform.name == "MateLife" || transform.name == "MateLife2")
            thisSlider.maxValue = Player.Mate.GetLP();
        else
            thisSlider.maxValue = Player.MaxLP;

        if (Player.MaxLP == 0)
            thisSlider.maxValue = 100 + GameMaster.levelStats[TempOpponent.Opponent.OpLvl - 1][GM.shapeID2][2];

        HPText = transform.Find("Fill Area").GetChild(0).GetChild(0).GetComponent<Text>();

        started = true;
        updateState();
    }

    public void updateState()
    {
        if (!started) { return; }
        if (Player == null) { return; }

        if (transform.parent.name == "Switch")
        {
            thisSlider.value = GM.player12.GetLife();
            HPText.text = GM.player12.GetLife().ToString();
        }
        else if (transform.name == "MateLife" || transform.name == "MateLife2")
        {
            thisSlider.value = Player.Mate.GetLP();
            HPText.text = Player.Mate.GetLP().ToString();
        }
        else
        {
            thisSlider.value = Player.GetLife();
            HPText.text = Player.GetLife().ToString();
        }
    }
}
