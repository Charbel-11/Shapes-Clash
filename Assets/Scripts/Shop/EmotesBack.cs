using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotesBack : MonoBehaviour
{
    public GameObject SHOP;
    public GameObject EMOTES;
    public GameObject Preview;

    private ShopManager SM;

    private void Start()
    {
        SM = GameObject.Find("Shop Manager").GetComponent<ShopManager>();
    }

    public void back()
    {
        if (!SM.canChange) { return; }

        SHOP.gameObject.SetActive(true);
        EMOTES.gameObject.SetActive(false);
        Preview.gameObject.SetActive(false);
    }
}
