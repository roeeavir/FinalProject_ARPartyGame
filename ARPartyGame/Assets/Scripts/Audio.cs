using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;
    [HideInInspector]
    public AudioSource source;
    [Range(0f, 1f)]
    public float spatialBlend;

    public bool loop;

}
