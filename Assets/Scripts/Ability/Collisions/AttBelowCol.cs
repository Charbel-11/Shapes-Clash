using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttBelowCol : MonoBehaviour {
    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.root.name != "Player1" && col.transform.root.name != "Player2") { return; }

        string escape = col.transform.root.GetComponent<Shape_Player>().GetEscapeType();
        if (escape == "Straight" || escape == "Above") { return; }

        if (escape == "Below") {
            col.transform.root.GetComponent<Animator>().SetInteger("ID", -1);
        }
        col.transform.root.GetComponent<Animator>().SetBool("Hit", true);
    }
}