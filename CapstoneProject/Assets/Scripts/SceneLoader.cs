using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void NewGame(string sceneName)
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(sceneName);
    }

    public void SwitchToScene(string sceneName)
    {
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