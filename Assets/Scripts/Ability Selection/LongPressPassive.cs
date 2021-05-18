using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LongPressPassive : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int position;        //first or second passive

    private bool butIsDown;
    private float downTime;

    private bool Selection;
    private bool MainMenu;
    private SelectionManager SM;
    private OpStat OS;

    private void Start()
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
            OS = transform.GetComponentInParent<OpStat>();
            Selection = false;
        }
        MainMenu = transform.root.name == "Main Menu";
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
        if (MainMenu)
        {
            if (!this.butIsDown)
                return;
            if (Time.realtimeSinceStartup - this.downTime > 1.5f)
            {
                OS.OpenPassivePanel(OS.opShapeID * 2 + position);
                this.butIsDown = false;
            }
        }
        else if (!Selection)
        {
            if (!this.butIsDown)
                return;
            if (Time.realtimeSinceStartup - this.downTime > 1.5f)
            {
                OS.OpenPassivePanel(OS.GM.shapeID2 * 2 + position);
                this.butIsDown = false;
            }
        }
        else
        {
            if (!this.butIsDown)
                return;
            if (Time.realtimeSinceStartup - this.downTime > 1.5f)
            {
                SM.OpenPassivePanel(2 * SM.finalShapeIndex + position);
                this.butIsDown = false;
            }
        }
    }
}