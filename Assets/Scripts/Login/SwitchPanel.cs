using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPanel : MonoBehaviour {
    public GameObject RegisterPanel;
    public GameObject LoginPanel;
    public void SwitchPanels()
    {
        if (LoginPanel.activeSelf == true)
        {
            RegisterPanel.SetActive(true);
            LoginPanel.SetActive(false);
        }
        else
        {
            LoginPanel.SetActive(true);
            RegisterPanel.SetActive(false);
        }
    }
}
