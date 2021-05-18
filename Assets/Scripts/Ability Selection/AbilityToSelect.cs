using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AbilityToSelect : MonoBehaviour {
    public int ID;
    //    public string text;
    public Sprite pic;
    public bool common;

    private bool alreadySelected;
    private bool currentlySelected;
    private Button but;
    private bool clickedOnce;
    private SelectionManager SM;
    private int level;

    public bool GetAlreadySelected()
    {
        return alreadySelected;
    }
    public void SetAlreadySelected(bool newBool)
    {
        alreadySelected = newBool;

        if (level != 0)
        {
            if (GetComponent<DragHandler>())
            {
                GetComponent<DragHandler>().draggable = !alreadySelected;
                GetComponent<CanvasGroup>().blocksRaycasts = !alreadySelected;
            }

            but.interactable = !alreadySelected;
        }

        transform.parent.GetComponentInChildren<abilityLevel>().updateTransparency();
    }

    public bool GetCurrentlySelected()
    {
        return currentlySelected;
    }
    public void SetCurrentlySelected(bool newBool)
    {
        currentlySelected = newBool;
    }

    public void SetClickedOnce(bool newBool)
    {
        clickedOnce = newBool;
    }

    private void Awake()
    {
        but = gameObject.GetComponent<Button>();
        but.onClick.AddListener(TaskOnClick);
        SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
    }

    public void setButton()
    {
        clickedOnce = false;
        but = gameObject.GetComponent<Button>();
        SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();

        setPic();

        Int32.TryParse(SelectionManager.AbLevelArray[ID], out level);
        if (level == 0)
        {
            but.interactable = false;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            GetComponent<DragHandler>().draggable = false;
            transform.Find("Lock").GetComponent<Image>().enabled = true;
        }
        else
        {
            transform.Find("Lock").GetComponent<Image>().enabled = false;
        }

        if (SM.finalIDs.Contains(this.ID))
            SetAlreadySelected(true);
        else
            SetAlreadySelected(false);
    }

    public void setOrder()
    {
        if (level == 0)
            transform.parent.SetAsLastSibling();       //locked abilities are seen last
    }

    public void setPic()
    {
        if (common)
        {
            string[] names = { "ImageCube", "ImagePyramid", "ImageStar", "ImageSphere" };
            foreach (Transform child in transform)
            {
                if (child.name == "Lock") { continue; }
                if (child.name == names[SM.finalShapeIndex])
                {
                    child.gameObject.SetActive(true);
                    pic = child.GetComponent<Image>().sprite;
                    continue;
                }
                child.gameObject.SetActive(false);
            }
        }
        else
        {
            pic = transform.Find("Image").GetComponent<Image>().sprite;
        }
        transform.parent.transform.Find("Name").GetComponent<Image>().color = ShapeConstants.bckgNameColor[SM.finalShapeIndex];
    }

    //Executes when the button is pressed
    public void TaskOnClick()
    {
        if (DragHandler.abilityDragged != null && DragHandler.abilityDragged != gameObject) { return; }
        if (this.clickedOnce == false && SM.IDOfSelectedAbility == -1)
        {
            this.currentlySelected = true;
            this.clickedOnce = true;
            gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.1f, 1.1f, 1.1f);
            gameObject.GetComponent<Image>().color = new Color(255f, 223f, 0);
            SM.IDOfSelectedAbility = this.ID;

            SM.goOfSelectedAb = gameObject;
            SM.HighlightUnselectedButtoons();
        }
        //Click once again to get back to the original state
        else if (this.clickedOnce == true && SM.IDOfSelectedAbility == this.ID)
        {
            this.currentlySelected = false;
            this.clickedOnce = false;
            gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f,1f,1f);
            gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f);
            SM.IDOfSelectedAbility = -1;
            SM.StopTheHighlight();
        }
        //If we clicked on a button and another button was already clicked
        else if (SM.IDOfSelectedAbility != this.ID)
        {
            //Same as if we clicked on it the first time but we have to disable the one clicked before
            this.currentlySelected = true;
            this.clickedOnce = true;
            gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.1f, 1.1f, 1.1f);
            gameObject.GetComponent<Image>().color = new Color(255f, 223f, 0);
            SM.IDOfSelectedAbility = this.ID;

            SM.goOfSelectedAb.GetComponent<AbilityToSelect>().SetCurrentlySelected(false);
            SM.goOfSelectedAb.GetComponent<AbilityToSelect>().SetClickedOnce(false);
            SM.goOfSelectedAb.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            SM.goOfSelectedAb.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f);

            SM.goOfSelectedAb = gameObject;
        }
    }
}