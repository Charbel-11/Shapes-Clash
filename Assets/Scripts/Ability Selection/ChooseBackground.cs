using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseBackground : MonoBehaviour {
    public int ID;

    private SelectionManager SM;

    public void Choose()
    {
        SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();

        SM.background = ID;
        PlayerPrefs.SetInt("BckgdID", ID);
        PlayerPrefs.Save();

        SM.ResetBckgdButtons();
    }
}