using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectate : MonoBehaviour
{
	public void OnClick()
    {
        string Username = transform.GetComponentInParent<FriendsButton>().GetUsername();
        ClientTCP.PACKAGE_Spectate(Username);
    }
}
