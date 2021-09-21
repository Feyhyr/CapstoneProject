using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuneController : MonoBehaviour
{
    public RuneSO rune;
    public Text nameText;

    public BattleManager bm;

    public void runeSelect()
    {
        Debug.Log("You have selected the " + rune.runeName + " rune");

        if (bm.rune1 == "")
        {
            bm.rune1 = rune.runeName;
        }
        else if ((bm.rune1 != "") && (bm.rune2 == ""))
        {
            bm.rune2 = rune.runeName;
        }
    }
}
