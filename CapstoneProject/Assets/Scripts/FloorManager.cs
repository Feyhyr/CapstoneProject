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

    public AudioSource audioSource;
    public List<AudioClip> floorBGM;
    private BattleManager bm;

    private new void Awake()
    {
        base.Awake();
        //SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainGameScene")
        {
            audioSource.Stop();
            floorsUnlocked = PlayerPrefs.GetInt(prefFloorUnlock, 1);

            if (floorsUnlocked > 5)
            {
                floorsUnlocked = 5;
            }

            floorCounterText = GameObject.Find("FloorCounterText").GetComponent<Text>();
            floorCounterText.text = "Floor " + floorsUnlocked.ToString();
        }
        if (scene.name == "BattleScene")
        {
            floorsUnlocked = PlayerPrefs.GetInt(prefFloorUnlock, 1);

            if (floorsUnlocked > 5)
            {
                floorsUnlocked = 5;
            }

            waveCounterText = GameObject.Find("WaveCounterText").GetComponent<Text>();
            CheckCount(prefWave, ref waveCount, ref waveCounterText, "Wave");
            bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
            bm.floorBackground.sprite = floorBackgrounds[floorCount - 1];
            audioSource.clip = floorBGM[floorCount - 1];
            audioSource.Play();
        }
        else if (scene.name == "GameWinScene" || scene.name == "GameOverScene")
        {
            audioSource.Stop();
            waveCount = PlayerPrefs.GetInt(prefWave, 1);
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void CheckCount(string key, ref int count, ref Text text, string prefix)
    {
        count = PlayerPrefs.GetInt(key, 1);
        if (count > 5)
        {
            count = 1;
            text.text = prefix + " " + count + "/5";
        }
        else
        {
            text.text = prefix + " " + count + "/5";
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
