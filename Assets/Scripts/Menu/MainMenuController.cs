using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public static bool JustRegistered = false;
    public GameObject MainMenuPanel, ShapeChoicePanel;

    void Awake()
    {
        Set();
    }
    public void Set()
    {
        MainMenuPanel.SetActive(!JustRegistered);
        ShapeChoicePanel.SetActive(JustRegistered);
    }
}
