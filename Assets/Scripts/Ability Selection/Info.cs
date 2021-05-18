using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Info : MonoBehaviour {

    public Transform infoP;
    private Button but;
    private bool alt;

    private void Start()
    {
        but = GetComponent<Button>();
        but.onClick.AddListener(showP);
        alt = true;

        if (transform.parent.name == "Game Modes")
            infoP.GetComponentInChildren<Text>().text = "For more info, long press on a game mode";
        else if (transform.root.name != "Canvas1" && transform.root.name != "Canvas2")
            infoP.GetComponentInChildren<Text>().text = "For more info, long press on an ability";
    }

    public void showP()
    {
        infoP.gameObject.SetActive(alt);
        alt = !alt;
    }
}
