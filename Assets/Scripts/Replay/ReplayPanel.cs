using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayPanel : MonoBehaviour
{
    public void Initialize()
    {
        Invoke("InitializeIt", 0.01f);        
    }
    private void InitializeIt()
    {
        transform.Find("Replays").GetComponent<ReplayController>().Initialize();
    }
}
