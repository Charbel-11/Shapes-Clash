using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ParticleCol : MonoBehaviour {
    ParticleSystem System;
    ParticleSystem.Particle[] Particles;
    BoxCollider Col;
    float LifeTime;
    int i;
    public bool PassedIt;
    public bool First;
    public float Time;
	void Start ()
    {
        System = gameObject.GetComponent<ParticleSystem>();
        Col = gameObject.GetComponent<BoxCollider>();
        Particles = new ParticleSystem.Particle[System.main.maxParticles];
	}
	
	void FixedUpdate ()
    {
        if (!PassedIt)
        {
            System.GetParticles(Particles);
            if (Particles.Length != 0)
            {
                if (First)
                {
                    foreach (ParticleSystem.Particle P in Particles)
                    {
                        if(P.remainingLifetime>= System.main.startLifetime.constant - 0.1f)
                        {
                            i = Array.IndexOf(Particles, P);
                        }
                    }
                    First = false;
                }
                LifeTime = Particles[i].remainingLifetime;
                if (LifeTime < Time)
                {
                    PassedIt = true;
                    /*foreach (ParticleSystem.Particle P in Particles)
                    {
                        if (P.remainingLifetime <= Time +0.05 && P.remainingLifetime >= Time-0.05)
                        {
                            i = Array.IndexOf(Particles, P);
                            break;
                        }
                    }*/
                }
                Col.center = Particles[i].position;
            }
        }
	}
    private void OnEnable()
    {
        //i = 0;
        First = true;
        PassedIt = false;
        Particles = new ParticleSystem.Particle[System.main.maxParticles];
        System.SetParticles(Particles, System.main.maxParticles);
    }
    private void OnDisable()
    {
        PassedIt = false;
        First = true;
    }
}
