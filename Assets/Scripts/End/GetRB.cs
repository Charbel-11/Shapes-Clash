using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetRB : MonoBehaviour
{
    private Text RBValue;

    public void UpdateRB(int rbGained)
    {
        RBValue = gameObject.GetComponent<Text>();
        RBValue.text = PlayerPrefs.GetInt("Redbolts").ToString();
        transform.parent.Find("RB Gained").GetComponent<Text>().text = "+ " + rbGained;

        GameObject AdPanel = transform.parent.parent.Find("AdPanel").gameObject;
        AdPanel.transform.Find("RB Gained").GetComponent<Text>().text = "+ " + rbGained;
        AdPanel.transform.Find("RB Gained 2").GetComponent<Text>().text = "+ " + (rbGained * 2);
    }
}
