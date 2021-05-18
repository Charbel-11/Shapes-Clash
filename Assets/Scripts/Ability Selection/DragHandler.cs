using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static GameObject abilityDragged;
    public bool draggable;

    public Vector3 startPos;
    public Transform startParent;
    public Transform canvas;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!draggable || abilityDragged != null) { return; }

        if (GetComponent<AbilityToSelect>().GetCurrentlySelected() == false)
            GetComponent<AbilityToSelect>().TaskOnClick();
        abilityDragged = gameObject;
        startPos = transform.position;
        startParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        canvas = GameObject.FindGameObjectWithTag("Drag Canvas").transform;
        GameObject.Find("Top Bar").transform.Find("Back").GetComponent<Button>().interactable = false;
        transform.GetComponentInParent<ScrollRect>().vertical = false;
        transform.parent = canvas;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!draggable || abilityDragged != gameObject) { return; }

        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!draggable || abilityDragged != gameObject) { return; }

        abilityDragged = null;

        if (transform.parent == canvas)
        {
            transform.position = startPos;
            transform.SetParent(startParent);
        }
        transform.GetComponentInParent<ScrollRect>().vertical = true;
        GameObject.Find("Top Bar").transform.Find("Back").GetComponent<Button>().interactable = true;

        if (GetComponent<AbilityToSelect>().GetAlreadySelected() == false)
        {
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<AbilityToSelect>().TaskOnClick();
        }
    }
}