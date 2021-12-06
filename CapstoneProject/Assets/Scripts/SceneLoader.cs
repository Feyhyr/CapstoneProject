using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    int tempToggleValue;

    public void NewGame(string sceneName)
    {
        tempToggleValue = PlayerPrefs.GetInt("instantSpell", 0);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("instantSpell", tempToggleValue);
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