using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseManager : MonoBehaviour
{

    void Awake()
    {
        if (!TempOpponent.Opponent.Replay)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }

}
