using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorSelect : MonoBehaviour
{
    private FloorManager floor;
    public string prefFloorSelect = "floorReached";
    public int floorReached;
    public List<Button> floorButtons;
    public List<GameObject> lockSymbol;
    public Text loreText;
    [TextArea]
    public List<string> floorLoreList;

    private void Start()
    {
        floor = GameObject.Find("FloorManager").GetComponent<FloorManager>();

        floorReached = PlayerPrefs.GetInt(prefFloorSelect, 1);

        for (int i = 0; i < floorButtons.Count; i++)
        {
            if (i + 1 > floorReached)
            {
                floorButtons[i].interactable = false;
                lockSymbol[i].SetActive(true);
            }
        }
    }

    public void ChangeFloor(int floorNum)
    {
        floor.floorCount = floorNum;
        loreText.text = floorLoreList[floorNum - 1];
        PlayerPrefs.SetInt(floor.prefWave, 1);
    }

    public void SaveFloorCount()
    {
        PlayerPrefs.SetInt(floor.prefFloor, floor.floorCount);
    }
}
