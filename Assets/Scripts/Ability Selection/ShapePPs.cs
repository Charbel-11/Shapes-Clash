using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapePPs : MonoBehaviour {

    public int state;       //1 when we need to update only 1 shape (the current one; if the script is attached at each shape)

    private SelectionManager SM;

    public void updateShapePPs()
    {
        if (state == 0)
        {
            SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();

            for (int i = 0; i < 4; i++)
            {
                Text t = transform.GetChild(i).GetComponent<Text>();
                int PP = SelectionManager.shapePPs[i];

                int j = 0;
                while (j < ShapeConstants.PPRange.Length && ShapeConstants.PPRange[j] <= PP) { j++; }
                t.text = PP.ToString();
                t.color = ShapeConstants.PPColors[j];
               
                transform.GetChild(i).gameObject.SetActive(i == SM.finalShapeIndex);
            }
        }
        else if (state == 1)
        {
            int curID = transform.parent.GetComponentInChildren<ChooseShape>().ID;

            Text t = GetComponent<Text>();
            int PP = SelectionManager.shapePPs[curID];

            int j = 0;
            while (j < ShapeConstants.PPRange.Length && ShapeConstants.PPRange[j] <= PP) { j++; }
            t.text = PP.ToString();
            t.color = ShapeConstants.PPColors[j];
        }
    }
}
