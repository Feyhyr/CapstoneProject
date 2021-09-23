using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    bool isPoisoned = false;
    int poisonTurnCount = 2;
    bool isExposed = false;
    bool isMelt = false;
    bool isFreeze = false;
    bool isBurn = false;
    bool isReverb = false;
    bool isCrystalize = false;

    bool isWeak = false;
    bool isResist = false;

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

        yield return new WaitForSeconds(0.5f);

        // fade in our characters sprites
        yield return FadeInOpponents();

        //yield return new WaitForSeconds(2);

        battleState = BattleState.PLAYERTURN;

        yield return PlayerTurn();
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
            yield return EndBattle();
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
            yield return EnemyTurn();
        }
    }

    IEnumerator EnemyTurn()
    {
        EnemyAttack();

        if (charCurrentHealth <= 0)
        {
            battleState = BattleState.LOSE;
            yield return EndBattle();
        }
        else
        {
            battleState = BattleState.PLAYERTURN;
            yield return PlayerTurn();
        }

        if (isPoisoned)
        {
            ePrefab.GetComponent<EnemyController>().enemCurrentHealth -= 3;
            poisonTurnCount--;
            Debug.Log("Enemy current health: " + ePrefab.GetComponent<EnemyController>().enemCurrentHealth);

            if (poisonTurnCount <= 0)
            {
                isPoisoned = false;
            }
        }

        if (ePrefab.GetComponent<EnemyController>().enemCurrentHealth <= 0)
        {
            battleState = BattleState.WIN;
            yield return EndBattle();
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

    public void PlayerAttack(int damage)
    {
        ePrefab.GetComponent<EnemyController>().enemCurrentHealth -= damage;
        Debug.Log("Player deals " + damage + " dmg\n Enemy has " + ePrefab.GetComponent<EnemyController>().enemCurrentHealth + " health left");
        playerAttacked = true;
    }

    public void EnemyAttack()
    {
        int enemyDmg = ePrefab.GetComponent<EnemyController>().atk;
        //int enemyDmg = Random.Range(ePrefab.GetComponent<EnemyController>().lowestDmg, ePrefab.GetComponent<EnemyController>().highestDmg);
        charCurrentHealth -= enemyDmg;
        Debug.Log("Enemy deals " + enemyDmg + " dmg\n Player has " + charCurrentHealth + " health left");
    }

    public bool Weakness()
    {
        if ((ePrefab.GetComponent<EnemyController>().weak.Contains(rune1) || (ePrefab.GetComponent<EnemyController>().weak.Contains(rune2))))
        {
            return true;
        }

        return false;
    }

    public bool Resistent()
    {
        if ((ePrefab.GetComponent<EnemyController>().resist.Contains(rune1) || (ePrefab.GetComponent<EnemyController>().resist.Contains(rune2))))
        {
            return true;
        }

        return false;
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
            isPoisoned = true;
            poisonTurnCount = 2;
            Debug.Log("Enemy has been poisoned");
        }
        else if (rune1 == "Wind" && rune2 == "Earth")
        {
            index = 2;
            isExposed = true;
            Debug.Log("Enemy has become vulnerable");
        }
        else if (rune1 == "Water" && rune2 == "Fire")
        {
            index = 3;
            isMelt = true;
            Debug.Log("Enemy has melted");
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
            isFreeze = true;
            Debug.Log("Enemy is frozen");
        }
        else if (rune1 == "Fire" && rune2 == "Wind")
        {
            index = 7;
            isBurn = true;
            Debug.Log("Enemy has been burned");
        }
        else if (rune1 == "Earth" && rune2 == "Wind")
        {
            index = 8;
            isReverb = true;
            Debug.Log("Reverb effect");
        }
        else if (rune1 == "Fire" && rune2 == "Water")
        {
            index = 9;
            charCurrentHealth += 3;
            if (charCurrentHealth > 50)
            {
                charCurrentHealth = 50;
            }
        }
        else if (rune1 == "Earth" && rune2 == "Water")
        {
            index = 10;
        }
        else if (rune1 == "Earth" && rune2 == "Fire")
        {
            index = 11;
            isCrystalize = true;
            Debug.Log("You crystalized");
        }

        GameObject sPrefab;

        sPrefab = Instantiate(spellPrefab[index], spellLocation);
        sPrefab.GetComponent<SpellCreation>().spell = spellList[index];
        sPrefab.GetComponent<SpellCreation>().spellName = spellList[index].sName;
        sPrefab.GetComponent<SpellCreation>().damage = spellList[index].sDamage;

        spellText.text = "You created " + sPrefab.GetComponent<SpellCreation>().spellName;

        PlayerAttack(sPrefab.GetComponent<SpellCreation>().damage);
    }

    public void TriggerAttack()
    {
        //if ((battleState == BattleState.PLAYERTURN) && (playerAttacked == false))
        //{
            //if ((rune1 != "") && (rune2 != ""))
            //{
       CreateSpell();
       StartCoroutine(PAttackPhase());
            //}
        //}
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