using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FloorManager : Singleton<FloorManager>
{
    public string prefFloor = "prefFloor";
    public Text floorCounterText;
    public int floorCount;

    public string prefWave = "prefWave";
    public Text waveCounterText;
    public int waveCount;

    private new void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainGameScene" || scene.name == "BattleScene" || scene.name == "GameWinScene" || scene.name == "GameOverScene")
        {
            floorCounterText = GameObject.Find("FloorCounterText").GetComponent<Text>();
            waveCounterText = GameObject.Find("WaveCounterText").GetComponent<Text>();
            CheckCount(prefFloor, ref floorCount, ref floorCounterText, "Floor", "Floor 5");
            CheckCount(prefWave, ref waveCount, ref waveCounterText, "Wave", "Boss Wave");
        }
    }

    private void CheckCount(string key, ref int count, ref Text text, string prefix, string specPrefix)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetInt(key, 1);
        }
        count = PlayerPrefs.GetInt(key);
        if (count > 5)
        {
            count = 1;
            text.text = prefix + " " + count;
        }
        else if (count == 5)
        {
            text.text = specPrefix;
        }
        else
        {
            text.text = prefix + " " + count;
        }
    }

    public void AddCount(ref int count, string key)
    {
        count++;
        PlayerPrefs.SetInt(key, count);
    }
}
