using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField] InputField username;   //Codes start with '/'
    public Button But;
    public GameObject constraints;
    public GameObject alreadyTaken;
    public GameObject wrongCode;
    public GameObject ac;

    public void LoginAccount()
    {
        But.interactable = false;
        if (username.text == string.Empty || username.text == "N")
        {
            But.interactable = true;
            return;
        }
        else if (username.text.Length > 12 || (!CheckUsername(username.text) && username.text[0] != '/'))
        {
            showConstraints();
            return;
        }
        ClientTCP.PACKAGE_NewAccount(username.text);
    }

    private bool CheckUsername(string username)
    {
        foreach(char c in username)
        {
            if (!(char.IsLetterOrDigit(c)))
                return false;
        }
        return true;
    }

    public void showConstraints()
    {
        But.interactable = true;
        alreadyTaken.SetActive(false);
        constraints.SetActive(true);
        wrongCode.SetActive(false);
    }

    public void taken()
    {
        But.interactable = true;
        alreadyTaken.SetActive(true);
        constraints.SetActive(false);
        wrongCode.SetActive(false);
    }

    public void WrongCode()
    {
        But.interactable = true;
        constraints.SetActive(false);
        alreadyTaken.SetActive(false);
        wrongCode.SetActive(true);
    }

    public void accepted()
    {
        alreadyTaken.SetActive(false);
        constraints.SetActive(false);
        ac.SetActive(true);
        wrongCode.SetActive(false);
        But.interactable = false;
    }
}
