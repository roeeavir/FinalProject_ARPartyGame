using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySounds : MonoBehaviour
{
    public void PlayFirstSound()
    {
        FindObjectOfType<AudioManager>().Play("FirstSound");
    }
}
