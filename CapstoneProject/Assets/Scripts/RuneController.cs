using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuneController : MonoBehaviour
{
    public RuneSO rune;
    public Text nameText;
    public Sprite runeIcon;

    public BattleManager bm;

    public void runeSelect()
    {
        if ((bm.battleState == BattleManager.BattleState.PLAYERTURN) && (!bm.playerAttacked))
        {
            if (bm.rune1 == "")
            {
                bm.rune1 = rune.runeName;
                gameObject.GetComponent<Button>().interactable = false;
                bm.firstRune.color = ChangeColour(bm.rune1);
            }
            else if ((bm.rune1 != "") && (bm.rune2 == ""))
            {
                bm.rune2 = rune.runeName;
                bm.runeIndex = rune.runeId;
                gameObject.GetComponent<Button>().interactable = false;
                bm.secondRune.color = ChangeColour(bm.rune2);
                bm.ChooseSpell();
                StartCoroutine(bm.SpellChosen());
            }
        }
    }

    private Color ChangeColour(string name)
    {
        Color element = Color.white;

        if (name == "Fire")
        {
            element = new Color32(134, 36, 35, 255);
        }
        else if (name == "Earth")
        {
            element = new Color32(115, 73, 46, 255);
        }
        else if (name == "Water")
        {
            element = new Color32(35, 73, 123, 255);
        }
        else if (name == "Wind")
        {
            element = new Color32(101, 185, 126, 255);
        }

        return element;
    }
}
