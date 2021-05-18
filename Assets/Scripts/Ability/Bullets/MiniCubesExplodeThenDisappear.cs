using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniCubesExplodeThenDisappear : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100)));
        Destroy(gameObject, 1f);
    }
}
