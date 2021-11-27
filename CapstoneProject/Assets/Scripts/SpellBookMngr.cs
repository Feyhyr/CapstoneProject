using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SpellBookMngr : Singleton<SpellBookMngr>
{
    public string[] keys = { "cyclone", "smog", "sandblast", "boil", "tsunami", "erupt", "ice storm", "blaze", "reverb", "steam", "lahar", "crystalize" };
    public List<bool> unlockList;
    public List<GameObject> spellBookObj;
    public List<Sprite> iconList;
    public Sprite unknownIcon;
    public GameObject spellBookCanvas;
    public bool spellBookState;
    public PauseGame pause;
    public GameObject textbox;
    public Transform spellContainer;
    public Transform textContainer;
    public bool canOpen;
    [TextArea(15, 100)]
    public List<string> spellInfoList;

    private new void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnLevelLoaded;
        CheckSpellBook();
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleScene")
        {
            canOpen = true;
            pause = GameObject.Find("OverlayCanvas").GetComponent<PauseGame>();
        }
        else if (scene.name == "MainGameScene")
        {
            canOpen = true;
            pause = GameObject.Find("MainGameCanvas").GetComponent<PauseGame>();
        }
        else if (scene.name == "MainMenuScene")
        {
            canOpen = false;
        }
    }

    private void Update()
    {
        if (canOpen)
        {
            if (pause == null)
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    ChangeState();
                }
            }
            else
            {
                if (!pause.gamePaused)
                {
                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        ChangeState();
                    }
                }
            }
        }
    }

    public void OpenDescription(int index)
    {
        if (EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite == unknownIcon)
        {
            textbox.GetComponentInChildren<Text>().text = "You have not unlocked this spell yet.";
        }
        else
        {
            textbox.GetComponentInChildren<Text>().text = spellInfoList[index];
        }
        textContainer.position = new Vector3(textContainer.position.x, -450, textContainer.position.z);
        textbox.SetActive(true);
    }

    public void ChangeState()
    {
        spellBookState = !spellBookState;
        OpenSpellBook();
    }

    public void OpenSpellBook()
    {
        if (spellBookState)
        {
            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
            spellBookCanvas.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            textbox.SetActive(false);
            spellBookCanvas.SetActive(false);
        }
        spellContainer.position = new Vector3(spellContainer.position.x, -450, spellContainer.position.z);
    }

    public void SetSpellKey(string[] key, ref List<bool> spellUnlock, int index, bool state)
    {
        spellUnlock[index] = state;
        PlayerPrefs.SetInt(key[index], BoolToInt(spellUnlock[index]));
        PlayerPrefs.Save();
    }

    public void CheckSpellBook()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            if (!PlayerPrefs.HasKey(keys[i]))
            {
                PlayerPrefs.SetInt(keys[i], BoolToInt(false));
            }
            unlockList[i] = IntToBool(PlayerPrefs.GetInt(keys[i]));
            if (unlockList[i])
            {
                spellBookObj[i].GetComponent<Image>().sprite = iconList[i];
            }
            else
            {
                spellBookObj[i].GetComponent<Image>().sprite = unknownIcon;
            }
        }
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

    public void TimeScaleNormal()
    {
        Time.timeScale = 1;
    }
}
