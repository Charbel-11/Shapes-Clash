using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColor : MonoBehaviour
{
    private Color[] ColorArray = new Color[4] { new Color(1, 0, 0), new Color(0, 1, 0), new Color(0, 0, 1), new Color(0.65f, 0.45f, 0.25f) };
    private Text TextComponent;
    private int Index = 0;
    void Start()
    {
        TextComponent = gameObject.GetComponent<Text>();
        Change();
    }
    private void Change()
    {
        TextComponent.color = ColorArray[Index];
        if (Index < 3)
            Index++;
        else
            Index = 0;
        Invoke("Change", 1f);
    }
}
