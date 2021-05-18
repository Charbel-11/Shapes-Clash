using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedShape : MonoBehaviour
{
    public Sprite[] Cube, Pyramid, Star, Sphere;
    public bool mainMenu;

    private SelectionManager SM;
    private int selectedShapeIndex;
    private int curIndex;

    void Start()
    {
        UpdateShape();
    }

    public void UpdateShape()
    {
        if (!mainMenu)
        {
            SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
            selectedShapeIndex = SM.finalShapeIndex;
        }
        else
        {
            selectedShapeIndex = ValuesChange.selectedShapeIndex;
        }

        if (selectedShapeIndex == 0)
        {
            gameObject.GetComponent<Image>().sprite = Cube[0];
        }
        else if (selectedShapeIndex == 1)
        {
            gameObject.GetComponent<Image>().sprite = Pyramid[0];
        }
        else if (selectedShapeIndex == 2)
        {
            gameObject.GetComponent<Image>().sprite = Star[0];
        }
        else if (selectedShapeIndex == 3)
        {
            gameObject.GetComponent<Image>().sprite = Sphere[0];
        }
    }

    public void changePic()
    {
        int nextIndex = 0;

        if (selectedShapeIndex == 0)
        {
            nextIndex = Random.Range(0, Cube.Length-1);
            if (nextIndex == curIndex)
            {
                nextIndex++; nextIndex %= Cube.Length;
            }
            gameObject.GetComponent<Image>().sprite = Cube[nextIndex];
        }
        else if (selectedShapeIndex == 1)
        {
            nextIndex = Random.Range(0, Pyramid.Length - 1);
            if (nextIndex == curIndex)
            {
                nextIndex++; nextIndex %= Pyramid.Length;
            }
            gameObject.GetComponent<Image>().sprite = Pyramid[nextIndex];
        }
        else if (selectedShapeIndex == 2)
        {
            nextIndex = Random.Range(0, Star.Length - 1);
            if (nextIndex == curIndex)
            {
                nextIndex++; nextIndex %= Star.Length;
            }
            gameObject.GetComponent<Image>().sprite = Star[nextIndex];
        }
        else if (selectedShapeIndex == 3)
        {
            nextIndex = Random.Range(0, Sphere.Length - 1);
            if (nextIndex == curIndex)
            {
                nextIndex++; nextIndex %= Sphere.Length;
            }
            gameObject.GetComponent<Image>().sprite = Sphere[nextIndex];
        }

        curIndex = nextIndex;
    }
}
