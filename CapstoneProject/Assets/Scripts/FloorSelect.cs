using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSelect : MonoBehaviour
{
    private FloorManager floor;
    public string prefFloorSelect = "floorReached";
    public int floorReached;

    private void Start()
    {
        floor = GameObject.Find("FloorManager").GetComponent<FloorManager>();

        floorReached = PlayerPrefs.GetInt(prefFloorSelect, 1);

        for (int i = 0; i < floor.floorButtons.Count; i++)
        {
            if (i + 1 > floorReached)
            {
                floor.floorButtons[i].interactable = false;
            }
        }
    }

    public void ChangeFloor(int floorNum)
    {
        floor.floorCount = floorNum;
        PlayerPrefs.SetInt(floor.prefWave, 1);
    }

    public void SaveFloorCount()
    {
        PlayerPrefs.SetInt(floor.prefFloor, floor.floorCount);
    }
}
