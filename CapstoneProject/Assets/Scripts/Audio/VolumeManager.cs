using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeManager : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    public AudioMixer sfxMixer;
    public AudioMixer bgmMixer;

    private void Start()
    {
        sfxSlider.value = PlayerPrefs.GetFloat("SFXvalue", 1f);
        sfxMixer.SetFloat("SFXVolume", Mathf.Log10(sfxSlider.value) * 20);
        bgmSlider.value = PlayerPrefs.GetFloat("BGMvalue", 1f);
        bgmMixer.SetFloat("BGMVolume", Mathf.Log10(sfxSlider.value) * 20);
    }

    public void SfxChange(float value)
    {
        float volume = value;
        PlayerPrefs.SetFloat("SFXvalue", volume);
        sfxMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    public void BgmChange(float value)
    {
        float volume = value;
        PlayerPrefs.SetFloat("BGMvalue", volume);
        bgmMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
    }
}
