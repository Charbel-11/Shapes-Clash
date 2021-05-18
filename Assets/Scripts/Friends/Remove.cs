using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Remove : MonoBehaviour
{
    public int state;
    private Friends Manager;
    private void OnEnable()
    {
        GetComponent<Button>().interactable = true;
        state = 0;
        GetComponentInChildren<Text>().text = "Remove Friend";
    }
    public void RemoveFriend()
    {
        if (state == 0)
        {
            GetComponentInChildren<Text>().text = "Click again to confirm";
            state = 1;
        }
        else
        {
            ClientTCP.PACKAGE_DEBUG("RF", PlayerPrefs.GetString("Username") + ":" + transform.GetComponentInParent<FriendsButton>().GetUsername());
            Manager = transform.parent.parent.parent.parent.parent.parent.GetComponentInChildren<Friends>();
            StartCoroutine(Manager.WaitforFL());
            GetComponent<Button>().interactable = false;
        }
    }

}
