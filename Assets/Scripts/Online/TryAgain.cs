using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryAgain : MonoBehaviour
{
    public ClientManager Manager;
    
    public void Try()
    {
        Manager.TryAgain();
        transform.parent.gameObject.SetActive(false);
    }
	
}
