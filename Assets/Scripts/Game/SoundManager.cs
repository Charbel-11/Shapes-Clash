using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance = null;

    public List<AudioClip> Clips = new List<AudioClip>();
    public List<AudioClip> Clips100 = new List<AudioClip>();
    public List<AudioClip> Clips200 = new List<AudioClip>();

    private AudioSource SoundEffectAudio;
    void Start()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        AudioSource[] Sources = GetComponents<AudioSource>();

        foreach (AudioSource Source in Sources)
        {
            if (Source.clip == null)
                SoundEffectAudio = Source;
        }

        SoundEffectAudio.volume = PlayerPrefs.GetFloat("AbVolume");
    }
    public void PlayOneShot(AudioClip Clip)
    {
        SoundEffectAudio.PlayOneShot(Clip);
    }

}
