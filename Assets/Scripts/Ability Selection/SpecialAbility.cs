using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SpecialAbility : MonoBehaviour {
    public int ID;
    public bool selected;
    public int level;
    public int shapeID;

    private Button but;
    private SelectionManager SM;

    private void Start()
    {
        but = gameObject.GetComponent<Button>();
        but.onClick.AddListener(TaskOnClick);
        SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
    }

    public void setButton()
    {
        but = gameObject.GetComponent<Button>();
        SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();

        if (ID > 200)
        {
            Int32.TryParse(SelectionManager.Super200[ID - 201], out level);
        }
        else if (ID > 100)
        {
            Int32.TryParse(SelectionManager.Super100[ID - 101], out level);
        }

        if (level == 0)
        {
            but.interactable = false;
            transform.Find("Lock").GetComponent<Image>().enabled = true;
            transform.Find("Panel").GetComponent<Image>().color = new Color(0.7843137f, 0.7843137f, 0.7843137f, 0.5f);
        }
        else
        {
            but.interactable = true;
            transform.Find("Panel").GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            transform.Find("Lock").GetComponent<Image>().enabled = false;
        }

        gameObject.transform.Find("Name").GetComponent<Image>().color = ShapeConstants.bckgNameColor[shapeID];
        Color tempC = (level == 3) ? ShapeConstants.levelMaxColors[shapeID] : ShapeConstants.levelColors[shapeID];
        for (int i = 0; i < level; i++)
        {
            transform.Find("Level").transform.GetChild(i).GetComponent<Image>().color = tempC;
        }
        for (int i = level; i < 3; i++)
        {
            transform.Find("Level").transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1);
        }
    }

    public void setOrder()
    {
        if (level == 0) { transform.SetAsLastSibling(); }
    }

    void TaskOnClick()
    {
        if (SM.goOfSpecialAbility != null)
        {
            SM.goOfSpecialAbility.transform.Find("Panel").gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f);
            SM.goOfSpecialAbility.gameObject.GetComponent<SpecialAbility>().selected = false;
            SM.goOfSpecialAbility.gameObject.GetComponent<SpecialAbility>().but.interactable = true;
        }

        transform.Find("Panel").gameObject.GetComponent<Image>().color = new Color(1f, .9f, 0);
        selected = true; but.interactable = false;
        SM.IDOfSpecialAbility = ID;
        SM.goOfSpecialAbility = gameObject;
        SM.specialAbilityName = gameObject.GetComponentInChildren<Text>().text;

        if (SM.P1)
            PlayerPrefs.SetInt("SpecialAbilityID" + SM.finalShapeIndex.ToString(), SM.IDOfSpecialAbility);
        else
            PlayerPrefs.SetInt("2SpecialAbilityID" + SM.finalShapeIndex.ToString(), SM.IDOfSpecialAbility);
        PlayerPrefs.Save();

        SM.updateSpecial();
    }
}
