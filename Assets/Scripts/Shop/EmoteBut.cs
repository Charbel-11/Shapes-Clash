using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmoteBut : MonoBehaviour
{
    public int ID;
    public bool common;

    public bool unlocked;
    public bool highlighted;

    private string pName;
    private Button but;
    private ShopManager SM;

    private void Start()
    {
        SM = GameObject.Find("Shop Manager").GetComponent<ShopManager>();

        pName = transform.parent.name;

        but = gameObject.GetComponent<Button>();
        but.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        if (!SM.canChange) { return; }

        if (highlighted) { SM.initialEmoteState(); return; }
        Highlight();
    }

    public void Highlight()
    {
        if (SM.goOfSelectedEmote != null) { SM.goOfSelectedEmote.GetComponent<EmoteBut>().RemoveHighlight(); }
        else {
            SM.previewText.SetActive(false);
            SM.buyEmote.gameObject.SetActive(true);
            SM.previews.gameObject.SetActive(true);
            SM.emoteName.gameObject.SetActive(true);
        }

        gameObject.GetComponent<Image>().color = new Color(1f, 1f, 0f);
        SM.goOfSelectedEmote = gameObject;
        SM.updateEmoteButton(ID);
        SM.emoteName.GetComponent<Text>().text = pName;
        SM.ShowAnimation(ID);
        if (SM.panelOpen == 1) { SM.commonInfoEmote.gameObject.SetActive(true); }
        highlighted = true;
    }

    public void updateState()
    {
        if (common)
        {
            for (int i = 0; i < 4; i++)
            {
                transform.GetChild(i).gameObject.SetActive(ShopManager.selectedShapeIndex == i);
            }
        }

        unlocked = (ShopManager.EmotesUnlock[ID] == 1);
        if (unlocked) 
            gameObject.transform.Find("Cost").gameObject.SetActive(false);
        else
            gameObject.transform.Find("Cost").GetComponentInChildren<Text>().text = ShopManager.EmotesPrice[ID].ToString();

        RemoveHighlight();
    }

    public void RemoveHighlight()
    {
        gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f);
        highlighted = false;
    }
}