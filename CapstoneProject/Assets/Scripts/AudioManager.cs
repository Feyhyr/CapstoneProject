using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : Singleton<AudioManager>
{
    public new AudioSource audio;


    public void Play(AudioClip sfxToPlay)
    {
        if (audio.isPlaying)
        {
            audio.Stop();
        }

        audio.clip = sfxToPlay;
        audio.Play();
    }
}
