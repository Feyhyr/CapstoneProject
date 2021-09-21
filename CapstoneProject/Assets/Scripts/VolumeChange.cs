using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VolumeChange : MonoBehaviour
{
    public Slider sfxSlider;
    public Slider musicSlider;

    private float valueSfx;
    private float valueMusic;

    AudioManager audioM;
    MusicManager musicM;

    public const string prefSFXvalue = "prefSFXvalue";
    public const string prefMusicValue = "prefMusicValue";

    private void Awake()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void Start()
    {
        valueSfx = PlayerPrefs.GetFloat(prefSFXvalue, 1f);
        sfxSlider.value = valueSfx;
        audioM.audio.volume = sfxSlider.value;

        valueMusic = PlayerPrefs.GetFloat(prefMusicValue, 1f);
        musicSlider.value = valueMusic;
        musicM.audio.volume = musicSlider.value;

        gameObject.SetActive(false);
    }

    public void SfxChange()
    {
        audioM.audio.volume = sfxSlider.value;
        valueSfx = sfxSlider.value;
        PlayerPrefs.SetFloat(prefSFXvalue, valueSfx);
    }

    public void MusicChange()
    {
        musicM.audio.volume = musicSlider.value;
        valueMusic = musicSlider.value;
        PlayerPrefs.SetFloat(prefMusicValue, valueMusic);
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenuScene")
        {
            audioM = GameObject.Find("AudioManager").GetComponent<AudioManager>();
            musicM = GameObject.Find("MusicManager").GetComponent<MusicManager>();
        }
    }
}
