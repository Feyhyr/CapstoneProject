using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToggleSettings : Singleton<ToggleSettings>
{
    public bool instantSpell;
    public string instantSpellKey = "instantSpell";

    private new void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleScene")
        {
            GetInstantSpellKey();
        }
        else if (scene.name == "MainGameScene")
        {
            GetInstantSpellKey();
        }
        else if (scene.name == "MainMenuScene")
        {
            GetInstantSpellKey();
        }
    }

    void GetInstantSpellKey()
    {
        instantSpell = IntToBool(PlayerPrefs.GetInt(instantSpellKey, 0));
    }

    public void SetInstantSpellKey(bool state)
    {
        instantSpell = state;
        PlayerPrefs.SetInt(instantSpellKey, BoolToInt(state));
        PlayerPrefs.Save();
    }

    int BoolToInt(bool key)
    {
        if (key)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    bool IntToBool(int key)
    {
        if (key != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
