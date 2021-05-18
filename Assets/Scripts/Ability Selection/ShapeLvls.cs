using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeLvls : MonoBehaviour {

    public int state;       //1 when we need to update only 1 shape (the current one; if the script is attached at each shape)
    public SelectionManager SM;

    public void updateLevels()
    {
        if (state == 0)
        {
            for (int i = 0; i < 4; i++)
            { 
                Text t = transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>();
                int lvl = SelectionManager.shapeLvls[i];
                if (lvl < 1) {
                    t.text = "0";

                    t = transform.GetChild(i).transform.GetChild(0).GetComponentInChildren<Text>();
                    t.text = SelectionManager.shapeExp[i].ToString();

                    Slider curSliderr = transform.GetChild(i).transform.GetChild(0).GetComponent<Slider>();
                    curSliderr.transform.Find("Fill Area").transform.GetChild(0).GetComponent<Image>().color = new Color(0f, .575f, 1f);
                    curSliderr.maxValue = 100000000;
                    curSliderr.value = SelectionManager.shapeExp[i];

                    transform.GetChild(i).gameObject.SetActive(i == SM.finalShapeIndex);
                    continue;
                }

                t.text = lvl.ToString();

                t = transform.GetChild(i).transform.GetChild(0).GetComponentInChildren<Text>();
                int xp = SelectionManager.shapeExp[i];
                t.text = xp.ToString() + (SelectionManager.shapeLvls[i] == ShapeConstants.maxLevel ? "" : "/" + SelectionManager.levelStats[SelectionManager.shapeLvls[i] - 1][4][0].ToString());

                Slider curSlider = transform.GetChild(i).transform.GetChild(0).GetComponent<Slider>();
                curSlider.transform.Find("Fill Area").transform.GetChild(0).GetComponent<Image>().color = new Color(0f, .575f, 1f);
                curSlider.maxValue = (SelectionManager.shapeLvls[i] == ShapeConstants.maxLevel ? xp : SelectionManager.levelStats[SelectionManager.shapeLvls[i] - 1][4][0]);
                curSlider.value = xp;

                transform.GetChild(i).gameObject.SetActive(i == SM.finalShapeIndex);
            }
        }
        else if (state == 1)
        {
            int curID = transform.parent.GetComponentInChildren<ChooseShape>().ID;

            Text t = transform.GetChild(1).GetComponentInChildren<Text>();
            int lvl = SelectionManager.shapeLvls[curID];
            if (lvl < 1)
            {
                t.text = "0";

                t = transform.GetChild(0).GetComponentInChildren<Text>();
                t.text = SelectionManager.shapeExp[curID].ToString();

                Slider curSliderr = transform.GetChild(0).GetComponent<Slider>();
                curSliderr.transform.Find("Fill Area").transform.GetChild(0).GetComponent<Image>().color = new Color(0f, .575f, 1f);
                curSliderr.maxValue = 100000000;
                curSliderr.value = SelectionManager.shapeExp[curID];
                return;
            }

            t.text = lvl.ToString();

            t = transform.GetChild(0).GetComponentInChildren<Text>();
            int xp = SelectionManager.shapeExp[curID];
            t.text = xp.ToString() + (SelectionManager.shapeLvls[curID] == ShapeConstants.maxLevel ? "" : "/" + SelectionManager.levelStats[SelectionManager.shapeLvls[curID] - 1][4][0].ToString());

            Slider curSlider = transform.GetChild(0).GetComponent<Slider>();
            curSlider.transform.Find("Fill Area").transform.GetChild(0).GetComponent<Image>().color = new Color(0f, .575f, 1f);
            curSlider.maxValue = (SelectionManager.shapeLvls[curID] == ShapeConstants.maxLevel ? xp : SelectionManager.levelStats[SelectionManager.shapeLvls[curID] - 1][4][0]);
            curSlider.value = xp;
        }
    }
}