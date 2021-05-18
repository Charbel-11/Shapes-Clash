using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextPlayer : MonoBehaviour {
    private bool online;

    public GameObject YouNeedToChooseMore;

    public GameObject Canvas1;
    public SelectionManager SM1;
    public GameObject Canvas2;
    public SelectionManager SM2;

    private SelectionManager SM;

    private void Start()
    {
        SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
        if (transform.root.name == "Choosing Scene") online = true;
    }

    public void SwitchPlayer()
    {
        int[] IDs = new int[6];
        IDs = SM.finalIDs;

        int chosen = 0;
        for (int i = 0; i < 6; i++)
        {
            chosen += (IDs[i] != -1 ? 1 : 0);
        }

        if (chosen < 4)
        {
            YouNeedToChooseMore.SetActive(true);
        }
        else
        {
            SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();      

            Canvas2.gameObject.SetActive(true);
            SM2.gameObject.SetActive(true);

            Canvas1.gameObject.SetActive(false);
            SM1.gameObject.SetActive(false);
            
            if (online) {
                GameObject topBar = Canvas2.transform.GetChild(0).GetChild(0).gameObject;
                topBar.transform.Find("Text").GetComponent<Text>().text = "2v2 Online";
                topBar.transform.Find("Search for a Game").gameObject.SetActive(true);
                topBar.transform.Find("Start Game").gameObject.SetActive(false);
            }

            SM2.updateShapeStats();
            SM2.UpdatePoolOfAbilities();
            SM2.updateSpecial();
            SM2.ResetSelectedShapeButton();
        }
    }

    public void BackToPlayer1()
    {
        Canvas2.gameObject.SetActive(false);
        SM2.gameObject.SetActive(false);

        SM1.gameObject.SetActive(true);
        Canvas1.gameObject.SetActive(true);
    }
}
