using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyPP : MonoBehaviour
{
    public bool added;
    private GameMaster GM; 

    public void UpdatePP(bool p1 = true)
    {
        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();

        Text t = GetComponent<Text>();
        int shapeIndex = p1 ? GM.shapeID1 : GM.shapeID12;
        int num = PlayerPrefsX.GetIntArray("PP")[shapeIndex];

        if (added)
        { 
            int diff = 0;
            if (p1) { diff = num - GM.previousPP; }
            else { diff = num - GM.GetComponent<GameMasterOnline>().previousPP2; }
            if (diff < 0)
                t.text = "- " + (-diff).ToString();
            else
                t.text = "+ " + diff.ToString();
        }
        else
        {
            t.text = num.ToString();
            int z = 0;
            while (z < ShapeConstants.PPRange.Length && ShapeConstants.PPRange[z] <= num) { z++; }
            t.color = ShapeConstants.PPColors[z];
        }
    }
}
