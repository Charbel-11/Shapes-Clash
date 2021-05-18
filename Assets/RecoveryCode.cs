using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecoveryCode : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Text>().text = "Recovery Code : " + PlayerPrefs.GetString("SecretCode");
    }
}
