using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrophyRoadSwitchShape : MonoBehaviour {
    public GameObject[] roads;
    public Sprite[] shapePics;

    private Button but;
    private int atShape;

    private void Start()
    {
        but = gameObject.GetComponent<Button>();
    }

    public void GoToShape(int idx)
    {
        for(int i = 0; i < 4; i++)
            roads[i].SetActive(i == idx);

        transform.Find("Text").GetComponent<Text>().text = "Switch to " + ShapeConstants.ShapeNames[(idx + 1) % 4];
        transform.Find("Image").GetComponent<Image>().sprite = shapePics[(idx + 1) % 4];
        atShape = idx;
    }

    public void GoToNextShape()
    {
        GoToShape((atShape + 1) % 4);
    }
}