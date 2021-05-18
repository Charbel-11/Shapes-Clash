using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFalseScript : MonoBehaviour
{
    private float Time = 0;

    private void OnEnable()
    {
        if (gameObject.name == "ShieldAppearance")
            Time = 2f;
        else if (gameObject.name == "ShieldDisappearance")
            Time = 3f;

        Invoke("SetFalse", Time);
    }

    private void SetFalse()
    {
        gameObject.SetActive(false);
    }
}
