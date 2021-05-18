using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseSkin : MonoBehaviour {
    public int ID;

    private SelectionManager SM;

    public void Choose()
    {
        SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();

        SM.skin = ID;
        if (SM.P1) { PlayerPrefs.SetInt("SkinID", ID); }
        else { PlayerPrefs.SetInt("SkinID2", ID); }   
        PlayerPrefs.Save();

        SM.resetSkinButton();
    }
}