using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyShape : MonoBehaviour
{
    public Sprite[] Shapes;     //Must be in the order Cube/pyramid/...

    private GameMaster GM;
    private int shapeIndex;

    public void UpdateShape(bool p1 = true)
    {
        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();

        shapeIndex = p1 ? GM.shapeID1 : GM.shapeID12;

        gameObject.GetComponent<Image>().sprite = Shapes[shapeIndex];
    }
}
