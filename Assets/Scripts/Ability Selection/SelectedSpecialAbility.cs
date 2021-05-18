using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedSpecialAbility : MonoBehaviour {
    public int level;
    public int ID;
    public string curName;

    private SelectionManager SM;

    public void updateSpecial()
    {
        SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();

        gameObject.GetComponentInChildren<Text>().text = curName;

        if (SM.goOfSpecialAbility == null)
        {
            gameObject.transform.Find("Image").GetComponent<Image>().sprite = null;
            gameObject.transform.Find("Image").GetComponent<Image>().enabled = false;
            gameObject.transform.Find("Panel").GetComponent<Button>().interactable = false;       
            level = 0;
        }
        else
        {
            gameObject.transform.Find("Image").GetComponent<Image>().enabled = true;
            gameObject.transform.Find("Image").GetComponent<Image>().sprite = SM.goOfSpecialAbility.GetComponent<SpecialAbility>().transform.Find("Image").GetComponent<Image>().sprite;
            gameObject.transform.Find("Panel").GetComponent<Button>().interactable = true;
        }

        gameObject.transform.Find("Name").GetComponent<Image>().color = ShapeConstants.bckgNameColor[SM.finalShapeIndex];

        Color tempC = (level == 3) ? ShapeConstants.levelMaxColors[SM.finalShapeIndex] : ShapeConstants.levelColors[SM.finalShapeIndex];
        for (int i = 0; i < level; i++)
            transform.parent.Find("Level").transform.GetChild(i).GetComponent<Image>().color = tempC;
        for (int i = level; i < 3; i++)
            transform.parent.Find("Level").transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1);
    }
}
