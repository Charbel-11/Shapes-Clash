using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetGold : MonoBehaviour
{
    private Text GoldValue;

    public void UpdateGold(int goldGained)
    {
        GoldValue = gameObject.GetComponent<Text>();
        GoldValue.text = PlayerPrefs.GetInt("Gold").ToString();
        transform.parent.Find("Gold Gained").GetComponent<Text>().text = "+ " + goldGained;

        GameObject AdPanel = transform.parent.parent.Find("AdPanel").gameObject;
        AdPanel.transform.Find("Gold Gained").GetComponent<Text>().text = "+ " + goldGained;
        AdPanel.transform.Find("Gold Gained 2").GetComponent<Text>().text = "+ " + (goldGained * 2);
    }
}
