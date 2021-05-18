using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmoteToSelect : MonoBehaviour
{
    public static int Chosen;       //Set to 0 in SM

    public int ID;
    public bool common;

    private bool selected;
    private Button But;
    private int pos;
    private string s;
    private bool locked;

    private SelectionManager SM;

    public void Start()
    {
        pos = -1;
        But = transform.GetChild(0).GetComponent<Button>();
        SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();

        if (SelectionManager.unlockedEmotes[ID] == 0)
        {
            deSelect();
            locked = true;
            transform.Find("Pics").GetComponent<Button>().enabled = false;
            transform.Find("Lock").GetComponent<Image>().enabled = true;
            transform.parent.SetAsLastSibling();
            return;
        }
        else
        {
            locked = false;
            transform.Find("Pics").GetComponent<Button>().enabled = true;
            transform.Find("Lock").GetComponent<Image>().enabled = false;
        }

        transform.parent.Find("Name").GetComponentInChildren<Text>().text = transform.parent.name;

        int i = 0;
        foreach (int e in SM.EmotesID)
        {
            if (ID == e) { pos = i; select(true); break; }
            i++;
        }
        if (pos == -1) { deSelect(); }

        if (SM.P1) { s = "EmotesID"; }
        else { s = "EmotesID2"; }
    }

    public void select(bool b = false)
    {
        if (selected) { deSelect(); return; }
        if (Chosen < 4 || b)
        {
            selected = true;
            gameObject.GetComponent<Image>().color = new Color(1f, 0.9411f, 0f);

            if (b)
            {
                SM.updateEmotes();
                return;
            }

            int i = 0;
            foreach (int e in SM.EmotesID)
            {
                if (e == -1) { pos = i; SM.EmotesID[i] = ID; break; }
                i++;
            }

            PlayerPrefsX.SetIntArray(s, SM.EmotesID);
            PlayerPrefs.Save();

            SM.updateEmotes();
        }
    }

    public void deSelect()
    {
        selected = false;
        gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f);

        if (pos != -1)
        {
            SM.EmotesID[pos] = -1;
            PlayerPrefsX.SetIntArray(s, SM.EmotesID);
            PlayerPrefs.Save();

            SM.updateEmotes();
        }
    }
}