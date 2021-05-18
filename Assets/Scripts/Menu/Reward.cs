using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reward : MonoBehaviour {
    public int road;   //0,1,2,3 for the shapes and 4 for the main one

    private int ID;
    private ValuesChange MM;

    private void Start()
    {
        MM = GameObject.Find("Menu Manager").GetComponent<ValuesChange>();
        Int32.TryParse(transform.name[transform.name.Length - 1].ToString(), out ID);
        ID--;

        int[] cur = ValuesChange.trophyRoadStats[road][ID];

        if (cur[5] != -1 || cur[4] != -1 || cur[3] != -1)
        {
            transform.Find("Text").gameObject.SetActive(false);
            transform.Find("Image").gameObject.SetActive(false);
            transform.Find("Text2").gameObject.SetActive(false);
            transform.Find("Image2").gameObject.SetActive(false);
            transform.Find("Duo").gameObject.SetActive(false);
            transform.Find("Rarity").gameObject.SetActive(true);
            transform.Find("Ability").gameObject.SetActive(true);

            int abID;
            if (cur[5] != -1) {
                abID = cur[5];
                transform.Find("Rarity").GetComponent<Image>().color = ShapeConstants.rarityColors[2];
                transform.Find("Rarity").GetChild(0).GetComponent<Text>().text = ShapeConstants.rarityNames[2];
            }
            else if (cur[4] != -1)
            {
                abID = cur[4];
                transform.Find("Rarity").GetComponent<Image>().color = ShapeConstants.rarityColors[1];
                transform.Find("Rarity").GetChild(0).GetComponent<Text>().text = ShapeConstants.rarityNames[1];
            }
            else
            {
                abID = cur[3];
                transform.Find("Rarity").GetComponent<Image>().color = ShapeConstants.rarityColors[0];
                transform.Find("Rarity").GetChild(0).GetComponent<Text>().text = ShapeConstants.rarityNames[0];
            }

            foreach (Transform go in MM.AllAbilities.transform)
            {
                if (go.GetComponent<Shape_Abilities>().ID == abID)
                {
                    if (go.GetComponent<Shape_Abilities>().common)
                        transform.Find("Ability").Find("AbPic").GetComponent<Image>().sprite = go.GetChild(ValuesChange.selectedShapeIndex).GetComponent<Image>().sprite;
                    else
                        transform.Find("Ability").Find("AbPic").GetComponent<Image>().sprite = go.GetChild(0).GetComponent<Image>().sprite;

                    if (road != 4) transform.Find("Ability").Find("Name").GetComponent<Image>().color = ShapeConstants.bckgNameColor[road];
                    transform.Find("Ability").Find("Name").GetComponentInChildren<Text>().text = go.name;
                    break;
                }
            }
        }
        else if (cur[2] != 0)
        {
            transform.Find("Text").gameObject.SetActive(false);
            transform.Find("Image").gameObject.SetActive(false);
            transform.Find("Text2").gameObject.SetActive(true);
            transform.Find("Image2").gameObject.SetActive(true);
            transform.Find("Duo").gameObject.SetActive(false);
            transform.Find("Rarity").gameObject.SetActive(false);
            transform.Find("Ability").gameObject.SetActive(false);

            transform.Find("Text2").GetComponent<Text>().text = cur[2].ToString();
        }
        else
        {
            transform.Find("Text").gameObject.SetActive(false);
            transform.Find("Image").gameObject.SetActive(false);
            transform.Find("Text2").gameObject.SetActive(false);
            transform.Find("Image2").gameObject.SetActive(false);
            transform.Find("Duo").gameObject.SetActive(true);
            transform.Find("Rarity").gameObject.SetActive(false);
            transform.Find("Ability").gameObject.SetActive(false);

            transform.Find("Duo").Find("Text").GetComponent<Text>().text = cur[0].ToString();
            transform.Find("Duo").Find("Text2").GetComponent<Text>().text = cur[1].ToString();
        }
    }
}