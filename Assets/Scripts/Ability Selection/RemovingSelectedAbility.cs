using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Code to attach on the red button

public class RemovingSelectedAbility : MonoBehaviour {
    private Button but;
    private SelectionManager SM;

    private void Awake()
    {
        but = gameObject.GetComponent<Button>();
        but.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        rem();
    }

    public void rem()
    {
        SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();

        SelectedAbility sA = transform.parent.GetComponent<SelectedAbility>();
        sA.SetSelected(false);

        for (int i = 0; i < 3; i++)
        {
            sA.levelsContainer.transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1);
        }

        foreach (Transform child in transform.parent)
        {
            if (child.name == "Name")
            {
                child.GetChild(0).GetComponent<Text>().text = "";
            }
            else if (child.name != this.name)
            {
                child.GetComponent<Image>().enabled = false;
                child.transform.GetChild(0).GetComponent<Image>().enabled = false;
            }
        }

        SM.finalIDs[sA.Position - 1] = -1;
        SM.finalTexts[sA.Position - 1] = "";
        SM.ReActivateRemovedAbility(sA.IDOfFinalAbility);

        if (SM.P1)
        {
            PlayerPrefsX.SetIntArray("ActiveAbilityID" + SM.finalShapeIndex.ToString(), SM.finalIDs);
            PlayerPrefsX.SetStringArray("ActiveAbilityText" + SM.finalShapeIndex.ToString(), SM.finalTexts);
        }
        else
        {
            PlayerPrefsX.SetIntArray("2ActiveAbilityID" + SM.finalShapeIndex.ToString(), SM.finalIDs);
            PlayerPrefsX.SetStringArray("2ActiveAbilityText" + SM.finalShapeIndex.ToString(), SM.finalTexts);
        }
        PlayerPrefs.Save();

        if (SM.IDOfSelectedAbility != -1)
            SM.HighlightUnselectedButtoons();
        
        gameObject.SetActive(false);
    }
}