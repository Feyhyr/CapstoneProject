using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject pauseCanvas;
    public bool gamePaused;
    public SpellBookMngr spellUnlock;

    private void Start()
    {
        spellUnlock = GameObject.Find("SpellUnlockMngr").GetComponent<SpellBookMngr>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeState();
            spellUnlock.spellBookCanvas.SetActive(false);
            spellUnlock.spellBookState = false;
        }
    }

    public void ChangeState()
    {
        gamePaused = !gamePaused;
        Pause();
    }

    public void Pause()
    {
        if (gamePaused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
            pauseCanvas.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pauseCanvas.SetActive(false);
        }
    }

    public void TimeScaleNormal()
    {
        Time.timeScale = 1f;
    }
}
