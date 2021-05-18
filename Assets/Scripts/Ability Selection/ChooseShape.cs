using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseShape : MonoBehaviour {
    private SelectionManager SM;
    public SelectedShape selectedShape;
    public int ID;

    public void Choose()
    {
        SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();

        SM.finalShapeIndex = ID;
        if (SM.P1) { PlayerPrefs.SetInt("ShapeSelectedID", SM.finalShapeIndex); }
        else { PlayerPrefs.SetInt("2ShapeSelectedID", SM.finalShapeIndex); }
        PlayerPrefs.Save();

        SM.UpdatePoolOfAbilities();
        SM.UpdatePoolOfSpecialAbilities();
        SM.updatePassives();
        selectedShape.UpdateShape();
        SM.updateShapeStats();
        SM.updateEmotes();
        SM.resetSkinPic();

        SM.ResetSelectedShapeButton();
    }
}
