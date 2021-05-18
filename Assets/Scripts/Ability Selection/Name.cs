using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Name : MonoBehaviour {
    public bool P1;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Choose2PlayersScreen")
        {
            GetComponent<Text>().text = "Player " + (P1 ? "1" : "2");
        }
        else
        {
            GetComponent<Text>().text = PlayerPrefs.GetString("Username");
        }
    }
}
