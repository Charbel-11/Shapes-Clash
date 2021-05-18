using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class SelectedAbility : MonoBehaviour, IDropHandler {
    // Position of the button (1 to 6)
    public int Position;
    public int IDOfFinalAbility;
    public Button but;
    public GameObject levelsContainer;

    //True when we select an ability from the ability pool, false otherwise
    public bool canNowBeSelected;
    //true if an ability is already selected here
    public bool selected;
    private int level;
    private SelectionManager SM;

    public bool GetSelected()
    {
        return selected;
    }
    public void SetSelected(bool newVal)
    {
        selected = newVal;
        updateState();
    }

    public bool GetCanNowBeSelected()
    {
        return canNowBeSelected;
    }
    public void SetCanNowBeSelected(bool newVal)
    {
        canNowBeSelected = newVal;
        updateState();
    }

    private void Start()
    {
        //but is the button of the FIRST children having a button
        but = GetComponent<Button>();
        but.onClick.AddListener(TaskOnClick);
        //Its text/Id is updated by SM calling its method
    }

    public void updateState()
    {
        if(but == null) { Start(); }

        //We can interact either to open the explanatory panel if an ability is already chosen or to set a new ability
        if ((canNowBeSelected == false && selected == false) || (canNowBeSelected == true && selected == true))
            but.interactable = false;
        else
            but.interactable = true;
    }

    public void TaskOnClick()
    {
        if (canNowBeSelected && !selected)
        {
            SetSelected(true);
            IDOfFinalAbility = SM.IDOfSelectedAbility;

            SM.finalIDs[Position - 1] = IDOfFinalAbility;
            SM.finalTexts[Position - 1] = SM.goOfSelectedAb.transform.parent.Find("Name").GetComponentInChildren<Text>().text;

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

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
                if (child.name == "Name")
                {
                        continue;
                }
                child.GetComponent<Image>().enabled = true;
                if (child.name != "X")
                {
                    child.transform.GetChild(0).GetComponent<Image>().enabled = true;
                }
            }
            gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = SM.goOfSelectedAb.GetComponent<AbilityToSelect>().pic;

            SM.goOfSelectedAb.GetComponent<AbilityToSelect>().SetAlreadySelected(true);
            SM.goOfSelectedAb.GetComponent<AbilityToSelect>().SetCurrentlySelected(false);
            SM.goOfSelectedAb.GetComponent<AbilityToSelect>().SetClickedOnce(false);
            SM.goOfSelectedAb.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            SM.goOfSelectedAb.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f);

            SM.IDOfSelectedAbility = -1;
            SM.StopTheHighlight();
            SM.UpdatePoolOfAbilities();

            updateLevel();
        }
    }

    public void UpdateContent()
    {
        SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
        
        //We should set the ID from playerprefs; same for the text
        IDOfFinalAbility = SM.finalIDs[Position - 1];
        transform.Find("Name").GetComponentInChildren<Text>().text = SM.finalTexts[Position - 1];
        transform.Find("Name").GetComponent<Image>().color = ShapeConstants.bckgNameColor[SM.finalShapeIndex];

        if (IDOfFinalAbility == -1) {
            SetSelected(false);
            for(int i = 0; i < 3; i++) levelsContainer.transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1);
            foreach (Transform child in transform)
            {
                if (child.name == "Name")
                {
                    child.GetChild(0).GetComponent<Text>().text = "";
                }
                else if (child.name != "X")
                {
                    child.GetComponent<Image>().enabled = false;
                    child.transform.GetChild(0).GetComponent<Image>().enabled = false;
                }
                else if (child.name == "X")
                {
                    child.gameObject.SetActive(false);
                }
            }
            return;
        }

        bool found = false;
        GameObject com = SM.containersOfAllAbilities[0];
        foreach (Transform child in com.transform)
        {
            if (IDOfFinalAbility == child.GetComponentInChildren<AbilityToSelect>().ID)
            {
                gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = child.GetComponentInChildren<AbilityToSelect>().pic;
                found = true; break;
            }
        }
        if (!found)
        {
            GameObject spec = SM.containersOfAllAbilities[SM.finalShapeIndex + 1];
            foreach (Transform child in spec.transform)
            {
                if (IDOfFinalAbility == child.GetComponentInChildren<AbilityToSelect>().ID)
                {
                    gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = child.GetComponentInChildren<AbilityToSelect>().pic;
                    break;
                }
            }
        }

        SetSelected(true);
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(0).GetComponent<Image>().enabled = true;
        transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().enabled = true;

        if (transform.parent.parent.name == "Change Abilities")
        {
            transform.Find("X").gameObject.SetActive(true);
        }

        canNowBeSelected = false;

        updateLevel();

        //If we selected a locked ability, remove it
        if (level == 0)
        {
            if (transform.Find("X"))
                transform.Find("X").GetComponent<RemovingSelectedAbility>().rem();
        }
    }

    public void updateLevel()
    {
        if (IDOfFinalAbility == -1) { level = 0; }
        else { Int32.TryParse(SelectionManager.AbLevelArray[IDOfFinalAbility], out level); }

        Color tempC = (level == 3) ? ShapeConstants.levelMaxColors[SM.finalShapeIndex] : ShapeConstants.levelColors[SM.finalShapeIndex];
        for (int i = 0; i < level; i++)
            levelsContainer.transform.GetChild(i).GetComponent<Image>().color = tempC;
        for (int i = level; i < 3; i++)
            levelsContainer.transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (SM.goOfSelectedAb.transform.parent == SM.goOfSelectedAb.GetComponent<DragHandler>().canvas)
        {
            SM.goOfSelectedAb.transform.position = SM.goOfSelectedAb.GetComponent<DragHandler>().startPos;
            SM.goOfSelectedAb.transform.SetParent(SM.goOfSelectedAb.GetComponent<DragHandler>().startParent);
            SM.goOfSelectedAb.transform.GetComponentInParent<ScrollRect>().vertical = true;
        }
        DragHandler.abilityDragged = null;
        GameObject.Find("Top Bar").transform.Find("Back").GetComponent<Button>().interactable = true;

        if (selected)
            GetComponentInChildren<RemovingSelectedAbility>().rem();

        TaskOnClick();
    }
}