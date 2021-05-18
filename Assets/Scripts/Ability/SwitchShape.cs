using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchShape : Shape_Abilities{
    public override void Awake()
    {
        ID = 53;
    }

    public override void SetSpecs()
    {
        attack = new int[3] { 0, 0, 0 };
        defense = new int[3] { 0, 0, 0 };
        escapeFrom = new bool[3] { false, false, false };
    }

    public override void Start()
    {
        GameMaster GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();

        if (transform.root.gameObject.name == "Player2")
        {
            P1 = false;
            player = GetComponentInParent<Shape_Player>();
        }
        else
        {
            P1 = true;
            player = GameObject.Find("Game Manager").GetComponent<GameMaster>().player1;

            EPBgrd = transform.Find("EPB").GetComponent<Image>();
            EPT = transform.Find("EP").GetComponent<Text>();

            EP_Needed = 2;      //implemented in GM online switch function directly
            EPT.text = EP_Needed.ToString();
            EPBgrd.color = ShapeConstants.bckdAbEPColor[GM.shapeID1];

            thisButt = gameObject.GetComponent<Button>();
            thisButt.GetComponent<Image>().color = new Color(1f, 1f, 1f);

            //Disabled();
        }

        if (transform.root.name == "Canvas1" || transform.root.name == "Canvas2")
        {
            string[] names = { "ImageCube", "ImagePyramid", "ImageStar", "ImageSphere" };
            for (int i = 0; i < names.Length; i++)
                transform.Find(names[i]).gameObject.SetActive(i == GM.shapeID12);
        }
    }

    //To be set up manually with the button
    public override void UseAbility()
    {
        if ((P1 && choiceDoneP1) || (!P1 && choiceDoneP2)) { return; }

        GameObject.Find("Game Manager").GetComponent<GameMasterOnline>().switchShape();
    }
}