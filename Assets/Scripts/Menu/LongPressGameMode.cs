using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LongPressGameMode : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject GameModePanels;
    public int id;

    private bool butIsDown;
    private float downTime;

    private void Start()
    {
        if (id == 2)
        {
            int unlockedNum = 0;
            for (int i = 0; i < 4; i++)
                unlockedNum += (ValuesChange.shapeLvls[i] > 0 ? 1 : 0);

            if (unlockedNum < 2)
            {
                transform.Find("Lock").GetComponent<Image>().enabled = true;
                GetComponent<Button>().interactable = false;
            }
            else
            {
                transform.Find("Lock").GetComponent<Image>().enabled = false;
                GetComponent<Button>().interactable = true;
            }
        }
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
        if (!this.butIsDown)
            return;
        if (Time.realtimeSinceStartup - this.downTime > 1.5f)
        {          
            for(int i = 0; i < GameModePanels.transform.childCount; i++)
            {
                GameModePanels.transform.GetChild(i).gameObject.SetActive(i == id);
            }
            this.butIsDown = false;
        }
    }
}