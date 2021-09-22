using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public enum BattleState { START, PLAYERTURN, ENEMYTURN, WIN, LOSE }
    public BattleState battleState;

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

    public int charMaxHealth;
    public int charCurrentHealth;

    public GameObject enemyPrefab;
    public Transform enemyLocation;

    GameObject ePrefab;

    public bool playerAttacked = false;

    private void Start()
    {
        battleState = BattleState.START;

        charCurrentHealth = charMaxHealth;

        GameObject rPrefab;

        for (int i = 0; i < runeList.Count; i++)
        {
            rPrefab = Instantiate(runePrefab, runeLocation);

            rPrefab.GetComponent<RuneController>().bm = this;
            rPrefab.GetComponent<RuneController>().rune = runeList[i];
            rPrefab.GetComponent<RuneController>().nameText.text = runeList[i].runeName;
        }

        objs = GameObject.FindGameObjectsWithTag("Button");

        StartCoroutine(BeginBattle());
    }

    IEnumerator BeginBattle()
    {
        ePrefab = Instantiate(enemyPrefab, enemyLocation);

        ePrefab.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

        yield return new WaitForSeconds(1);

        // fade in our characters sprites
        yield return StartCoroutine(FadeInOpponents());

        yield return new WaitForSeconds(2);

        battleState = BattleState.PLAYERTURN;

        yield return StartCoroutine(PlayerTurn());
    }

    IEnumerator PlayerTurn()
    {
        yield return new WaitForSeconds(1);

        playerAttacked = false;
    }

    IEnumerator PAttackPhase()
    {
        if (ePrefab.GetComponent<EnemyController>().enemCurrentHealth <= 0)
        {
            battleState = BattleState.WIN;
            yield return StartCoroutine(EndBattle());
        }

        else
        {
            rune1 = "";
            rune2 = "";

            foreach (GameObject Button in objs)
            {
                Button.GetComponent<Button>().interactable = true;
            }

            battleState = BattleState.ENEMYTURN;
            yield return StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        EnemyAttack();

        if (charCurrentHealth <= 0)
        {
            battleState = BattleState.LOSE;
            yield return StartCoroutine(EndBattle());
        }
        else
        {
            battleState = BattleState.PLAYERTURN;
            yield return StartCoroutine(PlayerTurn());
        }
    }

    IEnumerator EndBattle()
    {
        if (battleState == BattleState.WIN)
        {
            yield return new WaitForSeconds(1);
            Debug.Log("YOU WIN");
        }
        else if (battleState == BattleState.LOSE)
        {
            yield return new WaitForSeconds(1);
            Debug.Log("YOU LOSE");
        }
    }

    public void PlayerAttack()
    {
        ePrefab.GetComponent<EnemyController>().enemCurrentHealth -= 10;
        Debug.Log("Player deals 10 dmg\n Enemy has " + ePrefab.GetComponent<EnemyController>().enemCurrentHealth + " health left");
        playerAttacked = true;
    }

    public void EnemyAttack()
    {
        int enemyDmg = Random.Range(ePrefab.GetComponent<EnemyController>().lowestDmg, ePrefab.GetComponent<EnemyController>().highestDmg);
        charCurrentHealth -= enemyDmg;
        Debug.Log("Enemy deals " + enemyDmg + " dmg\n Player has " + charCurrentHealth + " health left");
    }

    public void CreateSpell()
    {
        int index = 0;

        if (rune1 == "Wind" && rune2 == "Water")
        {
            index = 0;
        }
        else if (rune1 == "Wind" && rune2 == "Fire")
        {
            index = 1;
        }
        else if (rune1 == "Wind" && rune2 == "Earth")
        {
            index = 2;
        }
        else if (rune1 == "Water" && rune2 == "Fire")
        {
            index = 3;
        }
        else if (rune1 == "Water" && rune2 == "Earth")
        {
            index = 4;
        }
        else if (rune1 == "Fire" && rune2 == "Earth")
        {
            index = 5;
        }
        else if (rune1 == "Water" && rune2 == "Wind")
        {
            index = 6;
        }
        else if (rune1 == "Fire" && rune2 == "Wind")
        {
            index = 7;
        }
        else if (rune1 == "Earth" && rune2 == "Wind")
        {
            index = 8;
        }
        else if (rune1 == "Fire" && rune2 == "Water")
        {
            index = 9;
        }
        else if (rune1 == "Earth" && rune2 == "Water")
        {
            index = 10;
        }
        else if (rune1 == "Earth" && rune2 == "Fire")
        {
            index = 11;
        }

        GameObject sPrefab;

        sPrefab = Instantiate(spellPrefab[index], spellLocation);
        sPrefab.GetComponent<SpellCreation>().spell = spellList[index];
        sPrefab.GetComponent<SpellCreation>().sName = spellList[index].spellName;

        spellText.text = "You created " + sPrefab.GetComponent<SpellCreation>().sName;
    }

    private void LateUpdate()
    {
        if ((battleState == BattleState.PLAYERTURN) && (playerAttacked == false))
        {
            if ((rune1 != "") && (rune2 != ""))
            {
                CreateSpell();
                PlayerAttack();
                StartCoroutine(PAttackPhase());
            }
        }
    }

    IEnumerator FadeInOpponents(int steps = 10)
    {
        float totalTransparencyPerStep = 1 / (float)steps;

        for (int i = 0; i < steps; i++)
        {
            setSpriteOpacity(ePrefab, totalTransparencyPerStep);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void setSpriteOpacity(GameObject ob, float transPerStep)
    {
        Color currColor = ob.GetComponent<SpriteRenderer>().color;
        float alpha = currColor.a;
        alpha += transPerStep;
        ob.GetComponent<SpriteRenderer>().color = new Color(currColor.r, currColor.g, currColor.b, alpha);
    }
}
