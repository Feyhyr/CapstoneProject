using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : Singleton<MusicManager>
{
    public new AudioSource audio;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    public void Play(AudioClip sfxToPlay)
    {
        if (audio.isPlaying)
        {
            audio.Stop();
        }

        audio.clip = sfxToPlay;
        audio.Play();
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleScene")
        {
            //Play(menuMusic);
        }
        else
        {
            //Play(gameMusic);
        }
    }
}
