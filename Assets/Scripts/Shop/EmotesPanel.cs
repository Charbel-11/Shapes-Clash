using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmotesPanel : MonoBehaviour {

    private Transform CommonP;
    private ShopManager SM;
    private bool initialized = false;

    private void Start()
    {
        openCommon();
    }

    public void openCommon()
    {
        if (!initialized)
        {
            initialized = true;
            CommonP = gameObject.transform.Find("CommonP");

            SM = GameObject.Find("Shop Manager").GetComponent<ShopManager>();
        }

        if (!SM.canChange) { return; }

        CommonP.gameObject.SetActive(true);
        SM.panelOpen = 1;
        SM.initialEmoteState();
    }
}