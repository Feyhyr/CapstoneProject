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
    public int runeIndex;

    public Text spellText;

    public GameObject[] buttonObjs;

    public int charMaxHealth;
    public int charCurrentHealth;

    public GameObject enemyPrefab;
    public Transform enemyLocation;

    GameObject ePrefab;

    public bool playerAttacked = false;

    bool isPoisoned = false;
    int poisonTurnCount = 2;
    bool isExposed = false;
    int exposedTurnCount = 1;
    bool isMelt = false;
    int meltTurnCount = 3;
    bool isFreeze = false;
    int freezeTurnCount = 2;
    bool isBurn = false;
    int burnTurnCount = 3;
    bool isReverb = false;
    int reverbCount = 1;
    bool isCrystalize = false;
    int crystalTurnCount = 2;

    private void Start()
    {
        //Random.InitState((int)System.DateTime.Now.Ticks);

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

        buttonObjs = GameObject.FindGameObjectsWithTag("Button");

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
        foreach (GameObject Button in buttonObjs)
        {
            Button.GetComponent<Button>().interactable = true;
        }

        if (isReverb)
        {
            buttonObjs[runeIndex].GetComponent<Button>().interactable = true;
            isReverb = false;
        }
        else
        {
            reverbCount = 1;
            buttonObjs[runeIndex].GetComponent<Button>().interactable = false;
        }

        rune1 = "";
        rune2 = "";

        if (isFreeze && freezeTurnCount > 0)
        {
            Debug.Log("Enemy cannot move");
            freezeTurnCount--;
            if (freezeTurnCount == 0)
            {
                isFreeze = false;
            }
            battleState = BattleState.PLAYERTURN;
            yield return PlayerTurn();
        }

        else if (ePrefab.GetComponent<EnemyController>().enemCurrentHealth <= 0)
        {
            battleState = BattleState.WIN;
            yield return EndBattle();
        }

        else
        {
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
            EnemyDamage(3);
            poisonTurnCount--;
            if (poisonTurnCount <= 0)
            {
                isPoisoned = false;
            }
        }

        if (isMelt)
        {
            EnemyDamage(1);
            meltTurnCount--;
            if (meltTurnCount <= 0)
            {
                isMelt = false;
            }
        }

        if (isBurn)
        {
            EnemyDamage(1);
            burnTurnCount--;
            if (burnTurnCount <= 0)
            {
                isBurn = false;
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
        if (Immunity())
        {
            damage = 0;
        }
        else
        {
            if (Weakness() && !CheckNeutral())
            {
                Debug.Log("Damage double");
                damage *= 2;
            }
            else if (Resistant() && !CheckNeutral())
            {
                Debug.Log("Damage half");
                damage /= 2;
            }
            else
            {
                Debug.Log("Damage normal");
            }

            if ((isMelt) && ((rune1 == "Fire") || (rune2 == "Fire")) && (meltTurnCount <= 2))
            {
                damage += 2;
            }

            if ((isBurn) && ((rune1 == "Wind") || (rune2 == "Wind")) && ((rune1 != "Water") && (rune2 != "Water")) && (burnTurnCount <= 2))
            {
                damage += 2;
            }

            if (isReverb)
            {
                damage += reverbCount;
                reverbCount++;
            }
        }
        
        Debug.Log("Player deals " + damage + " dmg");
        EnemyDamage(damage);
        playerAttacked = true;
    }

    public void EnemyAttack()
    {
        int enemyDmg = ePrefab.GetComponent<EnemyController>().atk;
        if (isCrystalize)
        {
            enemyDmg /= 2;
            crystalTurnCount--;
            if (crystalTurnCount <= 0)
            {
                isCrystalize = false;
            }
        }

        charCurrentHealth -= enemyDmg;
        Debug.Log("Enemy deals " + enemyDmg + " dmg\n Player has " + charCurrentHealth + " health left");
    }

    public bool Weakness()
    {
        List<string> weakHolder = ePrefab.GetComponent<EnemyController>().weak;

        if ((weakHolder.Contains(rune1) || (weakHolder.Contains(rune2))))
        {
            return true;
        }

        return false;
    }

    public bool Resistant()
    {
        List<string> resistHolder = ePrefab.GetComponent<EnemyController>().resist;

        if ((resistHolder.Contains(rune1) || (resistHolder.Contains(rune2))))
        {
            return true;
        }
        return false;
    }

    public bool Immunity()
    {
        List<string> immuneHolder = ePrefab.GetComponent<EnemyController>().immune;

        if ((immuneHolder.Contains(rune1) || (immuneHolder.Contains(rune2))))
        {
            return true;
        }
        return false;
    }

    public bool CheckNeutral()
    {
        if (Weakness() && Resistant())
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
            if (!Resistant())
            {
                isExposed = true;
                exposedTurnCount = 1;
                Debug.Log("Enemy has become vulnerable");
            }
        }
        else if (rune1 == "Water" && rune2 == "Fire")
        {
            index = 3;
            isMelt = true;
            meltTurnCount = 3;
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
            freezeTurnCount = 2;
            Debug.Log("Enemy is frozen");
            //if (ChanceStatusEffect(0.7f))
            //{

            //}
        }
        else if (rune1 == "Fire" && rune2 == "Wind")
        {
            index = 7;
            isBurn = true;
            burnTurnCount = 3;
            Debug.Log("Enemy has been burned");
        }
        else if (rune1 == "Earth" && rune2 == "Wind")
        {
            index = 8;
            isReverb = true;
            Debug.Log("Reverb effect " + reverbCount);
        }
        else if (rune1 == "Fire" && rune2 == "Water")
        {
            index = 9;
            charCurrentHealth += 3;
            if (charCurrentHealth > 50)
            {
                charCurrentHealth = 50;
            }
            Debug.Log("Character Health: " + charCurrentHealth);
        }
        else if (rune1 == "Earth" && rune2 == "Water")
        {
            index = 10;
        }
        else if (rune1 == "Earth" && rune2 == "Fire")
        {
            index = 11;
            isCrystalize = true;
            crystalTurnCount = 2;
            Debug.Log("You crystalized");
        }

        GameObject sPrefab;

        sPrefab = Instantiate(spellPrefab[index], spellLocation);
        sPrefab.GetComponent<SpellCreation>().spell = spellList[index];
        sPrefab.GetComponent<SpellCreation>().spellName = spellList[index].sName;
        sPrefab.GetComponent<SpellCreation>().damage = spellList[index].sDamage;

        spellText.text = "You created " + sPrefab.GetComponent<SpellCreation>().spellName;

        if ((isExposed) && (exposedTurnCount <= 0))
        {
            if (!Weakness() && CheckNeutral())
            {
                sPrefab.GetComponent<SpellCreation>().damage *= 2;
            }
            isExposed = false;
        }
        else if ((isExposed) && (exposedTurnCount > 0))
        {
            exposedTurnCount--;
        }

        PlayerAttack(sPrefab.GetComponent<SpellCreation>().damage);
    }

    public void TriggerAttack()
    {
        CreateSpell();
        StartCoroutine(PAttackPhase());
    }

    public void EnemyDamage(int damage)
    {
        ePrefab.GetComponent<EnemyController>().enemCurrentHealth -= damage;
        Debug.Log("Enemy current health: " + ePrefab.GetComponent<EnemyController>().enemCurrentHealth);
    }

    public bool ChanceStatusEffect(float chance)
    {
        if (Random.value >= chance)
        {
            return true;
        }
        return false;
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