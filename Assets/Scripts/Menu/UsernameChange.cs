using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UsernameChange : MonoBehaviour {

    private Text Username;

    void Start()
    {
        Username = gameObject.GetComponent<Text>();
        if (gameObject.name != "UsernameP2" && gameObject.name!= "UsernameP1Spec")
        {
            Username.text = PlayerPrefs.GetString("Username");
        }
        else if(gameObject.name == "UsernameP1Spec")
        {
            Username.text = TempOpponent.Opponent.Username2;
        }
        else
        {
            Username.text = TempOpponent.Opponent.Username;
        }
    }
}
