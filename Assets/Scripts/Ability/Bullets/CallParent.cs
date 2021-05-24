using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallParent : MonoBehaviour {
    BulletPlayer B;

    void Start() {
        B = GetComponentInParent<BulletPlayer>();
    }

    private void OnParticleCollision(GameObject other) {
        B.OnParticleCollision(other);
    }
}
