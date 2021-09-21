using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : Singleton<MusicManager>
{
    public new AudioSource audio;

    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip gameOverMusic;
    public AudioClip gameWinMusic;

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
        if (scene.name == "MainMenuScene")
        {
            Play(menuMusic);
        }
        else if (scene.name == "GameOverScene")
        {
            Play(gameOverMusic);
        }
        else if (scene.name == "GameWinScene")
        {
            Play(gameWinMusic);
        }
        else
        {
            Play(gameMusic);
        }
    }
}
