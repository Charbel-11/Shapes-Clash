using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeVolume : MonoBehaviour {

    private void Start()
    {
        ChangeIt();
    }
    public void ChangeIt ()
    {
        gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Volume");
	}
	
}
