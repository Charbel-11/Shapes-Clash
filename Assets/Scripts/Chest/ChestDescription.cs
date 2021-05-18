using System;
using UnityEngine;
using UnityEngine.UI;

public class ChestDescription : MonoBehaviour
{
    public int chestNum;

    private Text T;
    private Chest theChest;
    private int WS;

    void Awake()
    {
        T = GetComponent<Text>();
        if (chestNum == 0)
            WS = 1;
        else if (chestNum == 1)
            WS = 3;
        else if (chestNum == 2)
            WS = 6;
        else if (chestNum == 3)
            WS = 9;
        else
            WS = 12;
    }

    private void OnEnable()
    {
        int Arena = (ChestCode.FindPPOfMostUsedShape() / ShapeConstants.ArenaPP) + 1;
        theChest = new Chest(WS, Arena);

        transform.Find("CoinT").GetComponent<Text>().text = theChest.LowerBoundGold + " ~ " + theChest.UpperBoundGold;
        transform.Find("RedboltT").GetComponent<Text>().text = theChest.LowerBoundRedBolts + " ~ " + theChest.UpperBoundRedBolts;

        if (theChest.Diamonds != 0)
            transform.Find("DiamondsT").GetComponent<Text>().text = theChest.LowerBoundDiamonds + " ~ " + theChest.UpperBoundDiamonds;
        else
            transform.Find("DiamondsT").GetComponent<Text>().text = "0";

        transform.Find("AbilitiesT").GetComponent<Text>().text = "<color=#808080>" + theChest.CommonChance + " </color><color=#C836B0>" + theChest.RareChance + " </color><color=#FF8000>" + theChest.MythicChance + " </color><color=#FFFF00>" + theChest.MajesticChance + " </color>(%)";
        transform.Find("ShapesT").GetComponent<Text>().text = theChest.NewShapeChance + "%";
    }
}
