using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyLevel : MonoBehaviour
{
    public Sprite[] ShapeXP;

    private GameMaster GM;
    private int[] shapeLvls;
    private int[] shapeExp;

    public void UpdateLevel(int XPGained)
    { 
        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();

        shapeLvls = PlayerPrefsX.GetIntArray("Level");
        shapeExp = PlayerPrefsX.GetIntArray("XP");

        transform.parent.Find("Exp Gained").GetComponent<Text>().text = "+ " + XPGained.ToString();
        transform.parent.transform.Find("XPPic").GetComponent<Image>().sprite = ShapeXP[GM.shapeID1];
        Transform Shape = transform.Find("Shape");
        Shape.Find("Level").GetComponent<Text>().text = shapeLvls[GM.shapeID1].ToString();
        Shape.GetComponent<Image>().sprite = ShapeXP[GM.shapeID1];
        if (GM.shapeID1 == 0)
        {
            Shape.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            Shape.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f);
            Shape.Find("Level").GetComponent<RectTransform>().anchorMin = new Vector2(.2f, .3f);
            Shape.Find("Level").GetComponent<RectTransform>().anchorMax = new Vector2(.8f, .7f);
        }
        else if (GM.shapeID1 == 1)
        {
            Shape.transform.localScale = new Vector3(1f, 1f, 1f);
            Shape.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0.1f);
            Shape.Find("Level").GetComponent<RectTransform>().anchorMin = new Vector2(.225f, .15f);
            Shape.Find("Level").GetComponent<RectTransform>().anchorMax = new Vector2(.775f, .65f);
        }
        else if (GM.shapeID1 == 2)
        {
            Shape.transform.localScale = new Vector3(1f, 1f, 1f);
            Shape.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0.1f);
            Shape.Find("Level").GetComponent<RectTransform>().anchorMin = new Vector2(.3f, .25f);
            Shape.Find("Level").GetComponent<RectTransform>().anchorMax = new Vector2(.7f, .7f);
        }
        else if (GM.shapeID1 == 3)
        {
            Shape.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            Shape.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f);
            Shape.Find("Level").GetComponent<RectTransform>().anchorMin = new Vector2(.2f, .3f);
            Shape.Find("Level").GetComponent<RectTransform>().anchorMax = new Vector2(.8f, .7f);
        }

        transform.Find("Bar").GetComponent<Slider>().maxValue = (shapeLvls[GM.shapeID1] == ShapeConstants.maxLevel ? shapeExp[GM.shapeID1] : GameMaster.levelStats[shapeLvls[GM.shapeID1] - 1][4][0]);
        transform.Find("Bar").GetComponent<Slider>().value = shapeExp[GM.shapeID1];
        transform.Find("Bar").transform.Find("Text").GetComponent<Text>().text = shapeExp[GM.shapeID1].ToString() + (shapeLvls[GM.shapeID1] == ShapeConstants.maxLevel ? "" : "/" + GameMaster.levelStats[shapeLvls[GM.shapeID1] - 1][4][0].ToString());
    }
}
