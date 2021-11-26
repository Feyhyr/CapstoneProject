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

    private AudioClip previousClip;

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
            previousClip = audioSource.clip;
            floorsUnlocked = PlayerPrefs.GetInt(prefFloorUnlock, 1);

            if (floorsUnlocked > 5)
            {
                floorsUnlocked = 5;
            }

            bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
            waveCounterText = GameObject.Find("WaveCounterText").GetComponent<Text>();
            CheckCount(prefWave, ref waveCount, ref waveCounterText, "Wave");
            bm.floorBackground.sprite = floorBackgrounds[floorCount - 1];
            audioSource.clip = floorBGM[floorCount - 1];
            if (previousClip != audioSource.clip)
            {
                audioSource.Play();
            }
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
        }
        text.text = prefix + " " + count + "/" + (bm.enemyScriptables[floorCount - 1].enemyWaveList.Count + 1).ToString();
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
