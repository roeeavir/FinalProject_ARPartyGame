using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name; // The name of the sound
    public AudioClip clip; // The audio clip
    [Range(0f, 1f)]
    public float volume;  // The volume of the sound
    [Range(.1f, 3f)]
    public float pitch; // The pitch of the sound
    [HideInInspector]
    public AudioSource source; // The audio source
    [Range(0f, 1f)]
    public float spatialBlend; // The spatial blend of the sound

    public bool loop; // Whether or not the sound should loop
}
