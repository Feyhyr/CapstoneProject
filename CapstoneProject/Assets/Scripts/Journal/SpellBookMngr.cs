using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SpellBookMngr : Singleton<SpellBookMngr>
{
    public string[] spellKeys = { "cyclone", "smog", "sandblast", "boil", "tsunami", "erupt", "ice storm", "blaze", "reverb", "steam", "lahar", "crystalize" };
    public string[] enemyKeys = { "f1", "f2", "f3", "f4", "v1", "v2", "v3", "v4", "o1", "o2", "o3", "o4", "o5", "p1", "p2", "p3", "p4", "p5", "n1", "n2", "n3", "n4", "n5" };
    public List<bool> unlockSpellList;
    public List<bool> unlockEnemyList;
    public List<GameObject> spellBookObj;
    public List<GameObject> enemyListObj;
    public List<EnemySO> enemySOList;
    public List<Sprite> iconList;
    public Sprite unknownIcon;
    public GameObject journalCanvas;
    public bool spellBookState;
    public PauseGame pause;
    public GameObject spellTextbox;
    public GameObject statusTextBox;
    public GameObject enemyTextBox;
    public Transform spellContainer;
    public Transform statusContainer;
    public Transform enemyContainer;
    public Transform spellTextContainer;
    public Transform statusTextContainer;
    public Transform enemyTextContainer;
    public bool canOpen;

    public List<GameObject> enemyBiomesList;
    public List<bool> enemyBiomeState;

    [TextArea(15, 100)]
    public List<string> spellInfoList;
    [TextArea(5, 10)]
    public List<string> statusInfoList;
    [TextArea(15, 100)]
    public List<string> enemyInfoList;

    public ScrollRect enemyScrollRect;
    public List<GameObject> floorBTNs;

    public List<Texture> enemyImages;
    public Texture unknownEnemy;

    private new void Awake()
    {
        base.Awake();
        //SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void Start()
    {
        PlayerPrefs.SetInt("GameData", 1);
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenuScene")
        {
            canOpen = false;
        }
        else if (scene.name == "BattleScene")
        {
            canOpen = true;
            pause = GameObject.Find("OverlayCanvas").GetComponent<PauseGame>();
        }
        else if (scene.name == "MainGameScene")
        {
            canOpen = true;
            pause = GameObject.Find("MainGameCanvas").GetComponent<PauseGame>();
            CheckSpellBook();
            CheckBeastiary();
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
                    spellTextbox.SetActive(false);
                    statusTextBox.SetActive(false);
                    enemyTextBox.SetActive(false);
                    ChangeState();
                }
            }
            else
            {
                if (!pause.gamePaused)
                {
                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        spellTextbox.SetActive(false);
                        statusTextBox.SetActive(false);
                        enemyTextBox.SetActive(false);
                        ChangeState();
                    }
                }
            }
        }
    }

    #region Open Description
    public void OpenSpellDescription(int index)
    {
        if (EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite == unknownIcon)
        {
            spellTextbox.GetComponentInChildren<Text>().text = "You have not unlocked this spell yet.";
        }
        else
        {
            spellTextbox.GetComponentInChildren<Text>().text = spellInfoList[index];
        }
        spellTextContainer.localPosition = new Vector3(0, -90, 0);
        spellTextbox.SetActive(true);
    }

    public void OpenStatusDescription(int index)
    {
        statusTextBox.GetComponentInChildren<Text>().text = statusInfoList[index];
        statusTextContainer.localPosition = new Vector3(0, -90, 0);
        statusTextBox.SetActive(true);
    }

    public void OpenEnemyDescription(int index)
    {
        if (EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text == "???")
        {
            enemyTextBox.GetComponentInChildren<RawImage>().texture = unknownEnemy;
            enemyTextBox.GetComponentInChildren<Text>().text = "You have not encountered this enemy yet.";
        }
        else
        {
            enemyTextBox.GetComponentInChildren<RawImage>().texture = enemyImages[index];
            enemyTextBox.GetComponentInChildren<Text>().text = enemyInfoList[index];
        }
        enemyTextContainer.localPosition = new Vector3(0, -245, 0);
        enemyTextBox.SetActive(true);
    }
    #endregion

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
            spellContainer.localPosition = new Vector3(0, -490, 0);
            statusContainer.localPosition = new Vector3(0, -640, 0);
            enemyContainer.localPosition = new Vector3(0, 0, 0);
            for (int i = 0; i < enemyBiomesList.Count; i++)
            {
                enemyBiomesList[i].SetActive(false);
                floorBTNs[i].SetActive(true);
                enemyBiomeState[i] = false;
            }
            Time.timeScale = 0f;
            journalCanvas.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            spellTextbox.SetActive(false);
            enemyTextBox.SetActive(false);
            journalCanvas.SetActive(false);
        }
    }

    public void SetPrefKey(string[] key, ref List<bool> unlockList, int index, bool state)
    {
        unlockList[index] = state;
        PlayerPrefs.SetInt(key[index], BoolToInt(unlockList[index]));
        PlayerPrefs.Save();
    }

    #region SpellBook
    public void CheckSpellBook()
    {
        for (int i = 0; i < spellKeys.Length; i++)
        {
            if (!PlayerPrefs.HasKey(spellKeys[i]))
            {
                PlayerPrefs.SetInt(spellKeys[i], BoolToInt(false));
            }
            unlockSpellList[i] = IntToBool(PlayerPrefs.GetInt(spellKeys[i]));
            if (unlockSpellList[i])
            {
                spellBookObj[i].GetComponent<Image>().sprite = iconList[i];
            }
            else
            {
                spellBookObj[i].GetComponent<Image>().sprite = unknownIcon;
            }
        }
    }
    #endregion

    #region Beastiary
    public void OpenBiomeInfoState(int index)
    {
        enemyBiomeState[index] = !enemyBiomeState[index];
        OpenBiomeList(index);
        StartCoroutine(ScrollToBTN(index));
    }

    public void OpenBiomeList(int index)
    {
        if (enemyBiomeState[index])
        {
            enemyTextBox.SetActive(false);
            enemyBiomesList[index].SetActive(true);
        }
        else
        {
            enemyTextBox.SetActive(false);
            enemyBiomesList[index].SetActive(false);
        }
    }

    public void CheckBeastiary()
    {
        for (int i = 0; i < enemyKeys.Length; i++)
        {
            if (!PlayerPrefs.HasKey(enemyKeys[i]))
            {
                PlayerPrefs.SetInt(enemyKeys[i], BoolToInt(false));
            }
            unlockEnemyList[i] = IntToBool(PlayerPrefs.GetInt(enemyKeys[i]));
            enemySOList[i].hasSeen = unlockEnemyList[i];
            if (unlockEnemyList[i])
            {
                enemyListObj[i].GetComponentInChildren<Text>().text = enemySOList[i].enemyName;
            }
            else
            {
                enemyListObj[i].GetComponentInChildren<Text>().text = "???";
            }
        }
    }

    IEnumerator ScrollToBTN(int btnNum)
    {
        for (int i = 0; i < floorBTNs.Count; i++)
        {
            if (i < btnNum)
            {
                if (enemyBiomeState[btnNum])
                {
                    floorBTNs[i].SetActive(false);
                    enemyBiomesList[i].SetActive(false);
                    enemyBiomeState[i] = false;
                }
                else
                {
                    floorBTNs[i].SetActive(true);
                }
            }
            else if (i > btnNum)
            {
                floorBTNs[i].SetActive(true);
            }
        }
        yield return new WaitForSecondsRealtime(0.1f);
        enemyScrollRect.verticalNormalizedPosition = 1;
        
        /*float totalHeight = (5 - btnNum) * 150 + 10 * (5 - btnNum - 1) + 150 * enemyBiomesList[btnNum].transform.childCount + 5 * enemyBiomesList[btnNum].transform.childCount;

        float normalizedPos = totalHeight / enemyScrollRect.content.sizeDelta.y;

        enemyScrollRect.verticalNormalizedPosition = normalizedPos;
        Debug.Log("normal " + normalizedPos);
        Debug.Log("scrollrect " + enemyScrollRect.content.sizeDelta.y);
        Debug.Log("height " + totalHeight);*/
    }
    #endregion

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
