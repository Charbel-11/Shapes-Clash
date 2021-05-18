using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Get_EP : Shape_Abilities {

    public override void Awake()
    {
        ID = 0;
        base.Awake();
    }

    public override void SetSpecs()
    {
        attack = new int[3] { 0, 0, 0 };
        defense = new int[3] { 0, 0, 0 };
        escapeFrom = new bool[3] { false, false, false };
    }

    public override void Start()
    {
        if (transform.root.name == "Canvas1" || transform.root.name == "Canvas2")
        {
            string[] names = { "ImageCube", "ImagePyramid", "ImageStar", "ImageSphere" };
            GameMaster gm = GameObject.Find("Game Manager").GetComponent<GameMaster>();
            int curID;
            if (GameMaster.Online) curID = gm.shapeID1;
            else curID = (transform.root.name == "Canvas1") ? gm.shapeID1 : gm.shapeID2;
            for (int i = 0; i < names.Length; i++)
            {
                transform.Find(names[i]).gameObject.SetActive(i == curID);
            }
            if (GameMaster.Spectate)
                GetComponent<Button>().interactable = false;
        }
    }
}