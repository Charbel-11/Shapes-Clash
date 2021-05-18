using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallParent : MonoBehaviour {

    BulletPlayer1 B1;
    BulletPlayer2 B2;
	void Start ()
    {
		if(gameObject.layer == 10 || gameObject.layer == 12 || gameObject.layer == 13 || gameObject.layer == 14 || gameObject.layer == 18)
        {
            B1 = GetComponentInParent<BulletPlayer1>();
        }
        else
        {
            B2 = GetComponentInParent<BulletPlayer2>();
        }
	}

    private void OnParticleCollision(GameObject other)
    {
        if (gameObject.layer == 10 || gameObject.layer == 12 || gameObject.layer == 13 || gameObject.layer == 14 || gameObject.layer == 18)
        {
            B1.OnParticleCollision(other);
        }
        else
        {
            B2.OnParticleCollision(other);
        }
    }
}
