using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    int tempToggleValue;
    float tempSFXValue;
    float tempBGMValue;

    public void NewGame(string sceneName)
    {
        tempToggleValue = PlayerPrefs.GetInt("instantSpell", 0);
        tempSFXValue = PlayerPrefs.GetFloat("SFXvalue", 1f);
        tempBGMValue = PlayerPrefs.GetFloat("BGMvalue", 1f);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("instantSpell", tempToggleValue);
        PlayerPrefs.SetFloat("SFXvalue", tempSFXValue);
        PlayerPrefs.SetFloat("BGMvalue", tempBGMValue);
        SceneManager.LoadScene(sceneName);
    }

    public void SwitchToScene(string sceneName)
    {
        StopAllCoroutines();
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Game has quit :3");
        Application.Quit();
    }

    public void AlphaStart(string sceneName)
    {
        PlayerPrefs.SetInt("floorsUnlocked", 5);
        SceneManager.LoadScene(sceneName);
    }
}