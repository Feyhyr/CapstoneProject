using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckContinue : MonoBehaviour
{
    public SceneLoader sceneload;

    public GameObject continueBTN;
    public GameObject newGameConfirmPanel;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("GameData"))
        {
            continueBTN.SetActive(false);
        }
    }

    public void CheckNewGame()
    {
        if (!PlayerPrefs.HasKey("GameData"))
        {
            sceneload.NewGame("IntroScene");
        }
        else
        {
            newGameConfirmPanel.SetActive(true);
        }
    }
}
