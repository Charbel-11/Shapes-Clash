using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Passive : MonoBehaviour {
    public bool endGame;

    private int lvlP1;
    private int lvlP2;

    private int curIdx;

    public void updateInfo()
    {
        curIdx = transform.parent.parent.parent.Find("SelectionManager").GetComponent<SelectionManager>().finalShapeIndex;

        lvlP1 = SelectionManager.PassivesArray[curIdx * 2];
        lvlP2 = SelectionManager.PassivesArray[1 + curIdx * 2];

        transform.Find("P1").Find("ActiveP1").GetComponentInChildren<Text>().text = ShapeConstants.PassiveNames[curIdx * 2];
        transform.Find("P2").Find("ActiveP2").GetComponentInChildren<Text>().text = ShapeConstants.PassiveNames[1 + curIdx * 2];

        if (lvlP1 == 0)
            transform.Find("P1").Find("Percentage").GetComponentInChildren<Text>().text = "0 %";
        else
            transform.Find("P1").Find("Percentage").GetComponentInChildren<Text>().text = (SelectionManager.PassiveStatsArr[curIdx * 2][2 + lvlP1]).ToString() + "%";

        if (lvlP2 == 0)
            transform.Find("P2").Find("Percentage").GetComponentInChildren<Text>().text = "0 %";
        else
            transform.Find("P2").Find("Percentage").GetComponentInChildren<Text>().text = (SelectionManager.PassiveStatsArr[1 + curIdx * 2][2 + lvlP2]).ToString() + "%";

        Color tempC = (lvlP1 == 3) ? ShapeConstants.levelMaxColors[curIdx] : ShapeConstants.levelColors[curIdx];
        for (int i = 0; i < lvlP1; i++)
            transform.Find("LevelP1").transform.GetChild(i).GetComponent<Image>().color = tempC;
        for (int i = lvlP1; i < 3; i++)
            transform.Find("LevelP1").transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1);

        tempC = (lvlP2 == 3) ? ShapeConstants.levelMaxColors[curIdx] : ShapeConstants.levelColors[curIdx];
        for (int i = 0; i < lvlP2; i++)
            transform.Find("LevelP2").transform.GetChild(i).GetComponent<Image>().color = tempC;
        for (int i = lvlP2; i < 3; i++)
            transform.Find("LevelP2").transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1);
    }
}