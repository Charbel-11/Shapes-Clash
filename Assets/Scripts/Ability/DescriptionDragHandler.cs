using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DescriptionDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Vector3 startPos, curPos, maxChange;

    private bool set, disableBut;
    private bool opened;
    private GameMaster GM;

    private void Start()
    {
        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
        set = disableBut = opened = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        maxChange = new Vector3(0f, 0f, 0f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        curPos = Input.mousePosition;
        if (!set)
        {
            set = true; startPos = curPos;
        }

        if (!opened)
        {
            maxChange.x = Mathf.Max(maxChange.x, curPos.x - startPos.x);
            maxChange.y = Mathf.Max(maxChange.y, Mathf.Abs(curPos.y - startPos.y));

            if (!disableBut && (maxChange.x > 20 || maxChange.y > 20)) { disableBut = true; disableAb(); }

            if (curPos.x < startPos.x) { maxChange.y = 200; }   //enough to not open description
        }
        else
        {
            maxChange.x = Mathf.Min(maxChange.x, curPos.x - startPos.x);
            maxChange.y = Mathf.Max(maxChange.y, Mathf.Abs(curPos.y - startPos.y));

            if (curPos.x > startPos.x) { maxChange.y = 200; }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        set = false;

        if (!opened)
        {
            if (maxChange.y > 110 || maxChange.x < 100)
            {
                disableBut = false; StartCoroutine(revert());
                return;
            }

            OpenDescription();
        }
        else
        {
            if (maxChange.y > 110 || maxChange.x > -100) { return; }

            CloseDescription();
        }
    }

    private void OpenDescription()
    {
        opened = true; disableAb();
        transform.parent.Find("AbDescription").gameObject.SetActive(true);
    }
    private void CloseDescription()
    {
        disableBut = opened = false; enableAb();
        transform.parent.Find("AbDescription").gameObject.SetActive(false);
    }
    
    private void disableAb()
    {
        gameObject.GetComponent<InGameAb>().canUseAb = false;
    }
    private void enableAb()
    {
        gameObject.GetComponent<InGameAb>().canUseAb = true;
    }

    IEnumerator revert()
    {
        yield return new WaitForSeconds(0.1f);
        enableAb();
    }
}