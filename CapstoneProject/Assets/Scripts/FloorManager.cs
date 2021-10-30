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
    public string prefFloorUnlock = "floorsUnlocked";
    public int floorsUnlocked;

    public string prefWave = "prefWave";
    public Text waveCounterText;
    public int waveCount;

    public List<Sprite> floorBackgrounds;
    private BattleManager bm;

    private new void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleScene")
        {
            bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
            bm.floorBackground.sprite = floorBackgrounds[floorCount - 1];
        }
        if (scene.name == "MainGameScene" || scene.name == "BattleScene")
        {
            floorCounterText = GameObject.Find("FloorCounterText").GetComponent<Text>();
            waveCounterText = GameObject.Find("WaveCounterText").GetComponent<Text>();
            CheckCount(prefFloor, ref floorCount, ref floorCounterText, "Floor", "Floor 5");
            CheckCount(prefWave, ref waveCount, ref waveCounterText, "Wave", "Boss Wave");

            floorsUnlocked = PlayerPrefs.GetInt(prefFloorUnlock, 1);

            if (floorsUnlocked > 5)
            {
                floorsUnlocked = 5;
            }
        }
        else if (scene.name == "GameWinScene" || scene.name == "GameOverScene")
        {
            waveCount = PlayerPrefs.GetInt(prefWave, 1);
        }
    }

    private void CheckCount(string key, ref int count, ref Text text, string prefix, string specPrefix)
    {
        count = PlayerPrefs.GetInt(key, 1);
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
        if (count > 5)
        {
            count = 1;
        }
        PlayerPrefs.SetInt(key, count);
    }
}
