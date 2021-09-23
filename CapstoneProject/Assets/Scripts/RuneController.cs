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
        if ((bm.battleState == BattleManager.BattleState.PLAYERTURN) && (!bm.playerAttacked))
        {
            if (bm.rune1 == "")
            {
                bm.rune1 = rune.runeName;
                gameObject.GetComponent<Button>().interactable = false;
            }
            else if ((bm.rune1 != "") && (bm.rune2 == ""))
            {
                bm.rune2 = rune.runeName;
                bm.runeIndex = rune.runeId;
                Debug.Log("You have selected the " + bm.rune1 + " " + bm.rune2 + " rune");
                gameObject.GetComponent<Button>().interactable = false;
                bm.TriggerAttack();
            }
        }
    }
}
