using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LongPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler{

    private bool butIsDown;
    private float downTime;

    public int position;        //For the end screen only
    bool Selection;

    private SelectionManager SM;
    private OpStat OS;
    private SelectedAbility selectedAb;
    private AbilityToSelect abToSelect;
    private SpecialAbility specialAb;
    private SelectedSpecialAbility specialAb2;

    void Start()
    {
        if (GameObject.Find("SelectionManager"))
        {
            SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
            OS = null;
            Selection = true;
        }
        else
        {
            SM = null;
            OS = transform.parent.parent.GetComponent<OpStat>();
            Selection = false;
        }

        selectedAb = GetComponentInParent<SelectedAbility>();
        if (selectedAb == null) { selectedAb = GetComponent<SelectedAbility>(); }
        abToSelect = GetComponent<AbilityToSelect>();
        specialAb = GetComponentInParent<SpecialAbility>();
        specialAb2 = GetComponentInParent<SelectedSpecialAbility>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        butIsDown = true;
        downTime = Time.realtimeSinceStartup;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        butIsDown = false;
    }

    void Update()
    {
        if (!Selection)
        {
            if (!this.butIsDown)
                return;
            if (Time.realtimeSinceStartup - this.downTime > 1.5f)
            {
                OS.OpenExplanatoryPanel(position);
                this.butIsDown = false;
            }
        }
        else if (selectedAb != null)
        {
            if (!this.butIsDown || selectedAb.GetSelected() == false || selectedAb.GetCanNowBeSelected() == true)
                return;
            if (Time.realtimeSinceStartup - this.downTime > 1.5f)
            {
                SM.OpenExplanatoryPanel(selectedAb.IDOfFinalAbility);
                this.butIsDown = false;
            }
        }
        else if (abToSelect != null)
        {
            if (!this.butIsDown || abToSelect.GetCurrentlySelected() == true)
                return;
            if (Time.realtimeSinceStartup - this.downTime > 1.5f)
            {
                SM.OpenExplanatoryPanel(abToSelect.ID);
                this.butIsDown = false;
            }
        }
        else if (specialAb != null)
        {
            if (!this.butIsDown)
                return;
            if (Time.realtimeSinceStartup - this.downTime > 1.5f)
            {
                SM.OpenExplanatoryPanel(specialAb.ID);
                this.butIsDown = false;
            }
        }
        else if (specialAb2 != null)
        {
            if (!this.butIsDown)
                return;
            if (Time.realtimeSinceStartup - this.downTime > 1.5f)
            {
                SM.OpenExplanatoryPanel(specialAb2.ID);
                this.butIsDown = false;
            }
        }
    }
}