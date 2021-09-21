using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public GameObject runePrefab;
    public Transform runeLocation;

    public List<GameObject> spellPrefab;
    public Transform spellLocation;

    public List<RuneSO> runeList;
    public List<SpellSO> spellList;

    public string rune1;
    public string rune2;

    public Text spellText;

    public GameObject[] objs;

    private void Start()
    {
        GameObject rPrefab;

        for (int i = 0; i < runeList.Count; i++)
        {
            rPrefab = Instantiate(runePrefab, runeLocation);

            rPrefab.GetComponent<RuneController>().bm = this;
            rPrefab.GetComponent<RuneController>().rune = runeList[i];
            rPrefab.GetComponent<RuneController>().nameText.text = runeList[i].runeName;
        }

        objs = GameObject.FindGameObjectsWithTag("Button");
    }

    public void CreateSpell()
    {
        int index = 0;

        if ((rune1 == "FIRE" && rune2 == "WATER") || (rune1 == "WATER" && rune2 == "FIRE"))
        {
            index = 0;
        }
        else if ((rune1 == "FIRE" && rune2 == "POISON") || (rune1 == "POISON" && rune2 == "FIRE"))
        {
            index = 1;
        }
        else if ((rune1 == "POISON" && rune2 == "WATER") || (rune1 == "WATER" && rune2 == "POISON"))
        {
            index = 2;
        }
        else if ((rune1 == "POISON" && rune2 == "WIND") || (rune1 == "WIND" && rune2 == "POISON"))
        {
            index = 3;
        }
        else if ((rune1 == "WATER" && rune2 == "WIND") || (rune1 == "WIND" && rune2 == "WATER"))
        {
            index = 4;
        }
        else if ((rune1 == "FIRE" && rune2 == "WIND") || (rune1 == "WIND" && rune2 == "FIRE"))
        {
            index = 5;
        }

        GameObject sPrefab;

        sPrefab = Instantiate(spellPrefab[index], spellLocation);
        sPrefab.GetComponent<SpellCreation>().spell = spellList[index];
        sPrefab.GetComponent<SpellCreation>().sName = spellList[index].spellName;

        spellText.text = "You created " + sPrefab.GetComponent<SpellCreation>().sName;
    }

    private void LateUpdate()
    {
        if ((rune1 != "") && (rune2 != ""))
        {
            CreateSpell();

            rune1 = "";
            rune2 = "";

            foreach (GameObject Button in objs)
            {
                Button.GetComponent<Button>().interactable = true;
            }
        }
    }
}
