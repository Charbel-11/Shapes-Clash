using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesBack : MonoBehaviour {
    public GameObject SHOP;
    public GameObject ABS;

    private ShopManager SM;

    private void Start()
    {
        SM = GameObject.Find("Shop Manager").GetComponent<ShopManager>();
    }

    public void back()
    {
        if (!SM.canChange) { return; }

        SHOP.gameObject.SetActive(true);
        ABS.gameObject.SetActive(false);
    }
}
