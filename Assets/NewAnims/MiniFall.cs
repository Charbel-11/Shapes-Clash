using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniFall : MonoBehaviour {
    public FallInPieces Fall;

    private void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            Fall.Fall();
        }
    }
}
