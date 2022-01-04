using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioController : MonoBehaviour
{
    public Audio[] audio_tracks;
    // Start is called before the first frame update
    void Awake()
    {
        foreach (Audio a in audio_tracks)
        {
            a.source = gameObject.AddComponent<AudioSource>();
            a.source.clip = a.clip;
            a.source.volume = a.volume;
            a.source.pitch = a.pitch;
            a.source.loop = a.loop;
        }
    }

    public void Play(string name)
    {
        Audio a = Array.Find(audio_tracks, audio => audio.name == name);
        if (a == null)
        {
            Debug.LogWarning("Audio: " + name + " not found!");
            return;
        }
        a.source.Play();

    }
}
