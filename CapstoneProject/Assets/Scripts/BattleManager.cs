using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public enum BattleState { START, PLAYERTURN, ENEMYTURN, WIN, LOSE }
    public BattleState battleState;
    public Text turnPhaseText;

    public GameObject runePrefab;
    public Transform runeLocation;

    public List<GameObject> spellPrefab;
    //public Transform spellLocation;

    public List<RuneSO> runeList;
    public List<SpellSO> spellList;
    public List<EnemySO> enemyList;

    public string rune1;
    public string rune2;
    public int runeIndex;
    int sealedRuneIndex;

    public Text spellText;

    public GameObject[] buttonObjs;

    public int charMaxHealth;
    //public int charCurrentHealth;
    public Slider charHealthSlider;

    int randomEnemyCount;
    int randomEnemy;
    int enemyIndex = 0;
    public List<GameObject> enemyPrefab;
    public List<GameObject> currentEnemyList;
    public GameObject targetEnemyPrefab;
    public int targetEnemy = 0;
    //public GameObject enemyPrefab;
    public Transform enemyLocation;

    string enemyState;

    public bool playerAttacked = false;

    public GameObject crystalize;
    public GameObject reverb;
    public GameObject seal;
    public GameObject curse;
    public GameObject playerPoison;

    bool isCrystalize = false;
    int crystalTurnCount = 2;
    bool isReverb = false;
    int reverbTurnCount = 1;
    bool isCharSealed = false;
    int charSealedTurnCount = 2;
    bool isCharCursed = false;
    int charCursedTurnCount = 2;
    bool isCharPoisoned = false;
    int charPoisonedTurnCount = 2;

    public const string prefWave = "prefWave";
    public Text waveCounterText;
    public int waveCount;

    private void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);

        battleState = BattleState.START;

        if (!(PlayerPrefs.HasKey(prefWave)))
        {
            PlayerPrefs.SetInt(prefWave, 1);
        }
        waveCount = PlayerPrefs.GetInt(prefWave);
        if (waveCount > 5)
        {
            waveCount = 1;
            waveCounterText.text = "Wave " + waveCount;
        }
        else if (waveCount == 5)
        {
            waveCounterText.text = "Boss Wave";
        }
        else
        {
            waveCounterText.text = "Wave " + waveCount;
        }
        
        waveCount++;
        PlayerPrefs.SetInt(prefWave, waveCount);

        charHealthSlider.value = charMaxHealth;
        GameObject rPrefab;

        for (int i = 0; i < runeList.Count; i++)
        {
            rPrefab = Instantiate(runePrefab, runeLocation);

            rPrefab.GetComponent<RuneController>().bm = this;
            rPrefab.GetComponent<RuneController>().rune = runeList[i];
            rPrefab.GetComponent<RuneController>().nameText.text = runeList[i].runeName;
            rPrefab.GetComponent<RuneController>().runeIcon = runeList[i].icon;
            rPrefab.GetComponent<Button>().image.sprite = rPrefab.GetComponent<RuneController>().runeIcon;
        }

        buttonObjs = GameObject.FindGameObjectsWithTag("Button");

        StartCoroutine(BeginNormalBattle());
    }

    IEnumerator BeginNormalBattle()
    {
        turnPhaseText.text = "";

        GameObject ePrefab;

        enemyIndex = 0;
        randomEnemyCount = Random.Range(1, 4);
        Debug.Log("Random enemy count: " + randomEnemyCount);

        for (int i = 0; i < randomEnemyCount; i++)
        {
            randomEnemy = Random.Range(0, enemyList.Count);
            ePrefab = Instantiate(enemyPrefab[randomEnemy], enemyLocation);

            ePrefab.GetComponentInChildren<EnemyController>().bm = this;
            ePrefab.GetComponentInChildren<EnemyController>().enemyId = enemyIndex;
            enemyIndex++;
            currentEnemyList.Add(ePrefab);
        }

        CheckMultipleEnemies();
        currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().targetSelected.SetActive(true);

        enemyState = currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().currentState;

        //ePrefab.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

        //yield return new WaitForSeconds(0.5f);

        // fade in our characters sprites
        //yield return FadeInOpponents();

        //yield return new WaitForSeconds(2);

        battleState = BattleState.PLAYERTURN;

        yield return PlayerTurn();
    }

    IEnumerator PlayerTurn()
    {
        yield return new WaitForSeconds(1);

        playerAttacked = false;
        turnPhaseText.text = "Player Turn";
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
            reverbTurnCount = 1;
            reverb.SetActive(false);
            buttonObjs[runeIndex].GetComponent<Button>().interactable = false;
        }

        if (isCharSealed)
        {
            EnemySeal();
        }

        rune1 = "";
        rune2 = "";

        if (currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().enemyHealthSlider.value <= 0)
        {
            yield return new WaitForSeconds(1);
            currentEnemyList[targetEnemy].SetActive(false);
            currentEnemyList.RemoveAt(targetEnemy);

            if (IsAllEnemiesDead())
            {
                battleState = BattleState.WIN;
                yield return EndBattle();
            }

            else
            {
                targetEnemy = 0;
                currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().targetSelected.SetActive(true);
                enemyIndex = 0;

                for (int i = 0; i < currentEnemyList.Count; i++)
                {
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyId = enemyIndex;
                    enemyIndex++;
                }
            }
        }

        else
        {
            if (isCharCursed)
            {
                EnemyCurse();
            }

            if (isCharPoisoned)
            {
                EnemyPoison();
            }

            if (charHealthSlider.value <= 0)
            {
                battleState = BattleState.LOSE;
                yield return EndBattle();
            }
        }
        battleState = BattleState.ENEMYTURN;
        yield return EnemyTurn();
    }

    IEnumerator EnemyTurn()
    {
        turnPhaseText.text = "Enemy Turn";
        yield return new WaitForSeconds(1);
        for (int i = 0; i < currentEnemyList.Count; i++)
        {
            if (charHealthSlider.value <= 0)
            {
                battleState = BattleState.LOSE;
                yield return EndBattle();
            }

            if (currentEnemyList[i].GetComponentInChildren<EnemyController>().isFreeze && currentEnemyList[i].GetComponentInChildren<EnemyController>().freezeTurnCount > 0)
            {
                Debug.Log(currentEnemyList[i].GetComponentInChildren<EnemyController>().eText.text + " cannot move");
                currentEnemyList[i].GetComponentInChildren<EnemyController>().freezeTurnCount--;
                if (currentEnemyList[i].GetComponentInChildren<EnemyController>().freezeTurnCount == 0)
                {
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().isFreeze = false;
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().frozen.SetActive(false);
                }
            }

            else
            {
                EnemyAttack(i);
                if (currentEnemyList[i].tag == "Whale")
                {
                    if (ChanceStatusEffect(0.7f))
                    {
                        Debug.Log("Player is cursed");
                        isCharCursed = true;
                        charCursedTurnCount = 2;
                        curse.SetActive(true);
                    }
                }
                else if (currentEnemyList[i].tag == "Hellfire")
                {
                    if (ChanceStatusEffect(0.7f))
                    {
                        Debug.Log("Player is poisoned");
                        isCharPoisoned = true;
                        charPoisonedTurnCount = 2;
                        playerPoison.SetActive(true);
                    }
                }
                else if (currentEnemyList[i].tag == "Cloud")
                {
                    if (!isCharSealed)
                    {
                        Debug.Log("Player is sealed");
                        sealedRuneIndex = Random.Range(0, 4);
                        while (sealedRuneIndex == runeIndex)
                        {
                            sealedRuneIndex = Random.Range(0, 4);
                        }
                        buttonObjs[sealedRuneIndex].GetComponent<Button>().interactable = false;
                        Debug.Log(buttonObjs[sealedRuneIndex].GetComponent<RuneController>().nameText.text + " rune has been sealed");
                        charSealedTurnCount = 2;
                        seal.SetActive(true);
                        isCharSealed = true;
                    }
                }
            }

            yield return new WaitForSeconds(1);
            enemyState = "Idle";
            currentEnemyList[i].GetComponentInChildren<EnemyController>().SetCharacterState(enemyState);

            if (charHealthSlider.value <= 0)
            {
                battleState = BattleState.LOSE;
                yield return EndBattle();
            }

            if (currentEnemyList[i].GetComponentInChildren<EnemyController>().isPoisoned)
            {
                EnemyDamage(3);
                currentEnemyList[i].GetComponentInChildren<EnemyController>().poisonTurnCount--;
                if (currentEnemyList[i].GetComponentInChildren<EnemyController>().poisonTurnCount <= 0)
                {
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().isPoisoned = false;
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().poisoned.SetActive(false);
                }
            }

            if (currentEnemyList[i].GetComponentInChildren<EnemyController>().isMelt)
            {
                EnemyDamage(1);
                currentEnemyList[i].GetComponentInChildren<EnemyController>().meltTurnCount--;
                if (currentEnemyList[i].GetComponentInChildren<EnemyController>().meltTurnCount <= 0)
                {
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().isMelt = false;
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().melted.SetActive(false);
                }
            }

            if (currentEnemyList[i].GetComponentInChildren<EnemyController>().isBurn)
            {
                EnemyDamage(1);
                currentEnemyList[i].GetComponentInChildren<EnemyController>().burnTurnCount--;
                if (currentEnemyList[i].GetComponentInChildren<EnemyController>().burnTurnCount <= 0)
                {
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().isBurn = false;
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().burned.SetActive(false);
                }
            }
        }

        if (currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().enemyHealthSlider.value <= 0)
        {
            yield return new WaitForSeconds(1);
            currentEnemyList[targetEnemy].SetActive(false);
            currentEnemyList.RemoveAt(targetEnemy);

            if (IsAllEnemiesDead())
            {
                battleState = BattleState.WIN;
                yield return EndBattle();
            }

            else
            {
                targetEnemy = 0;
                currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().targetSelected.SetActive(true);
                enemyIndex = 0;

                for (int i = 0; i < currentEnemyList.Count; i++)
                {
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyId = enemyIndex;
                    enemyIndex++;
                }
            }
        }

        battleState = BattleState.PLAYERTURN;
        yield return PlayerTurn();
    }

    IEnumerator EndBattle()
    {
        if (battleState == BattleState.WIN)
        {
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene("GameWinScene");
        }
        else if (battleState == BattleState.LOSE)
        {
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene("GameOverScene");
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

            if ((currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().isMelt) && ((rune1 == "Fire") || (rune2 == "Fire")) && (currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().meltTurnCount <= 2))
            {
                damage += 2;
            }

            if ((currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().isBurn) && ((rune1 == "Wind") || (rune2 == "Wind")) && ((rune1 != "Water") && (rune2 != "Water")) && (currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().burnTurnCount <= 2))
            {
                damage += 2;
            }

            if (isReverb)
            {
                damage += reverbTurnCount;
                reverbTurnCount++;
            }
        }
        
        Debug.Log("Player deals " + damage + " dmg");
        EnemyDamage(damage);
        enemyState = "Damage";
        currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().SetCharacterState(enemyState);
        playerAttacked = true;
    }

    public void EnemyAttack(int value)
    {
        int enemyDmg = currentEnemyList[value].GetComponentInChildren<EnemyController>().atk;
        if (isCrystalize)
        {
            enemyDmg /= 2;
            crystalTurnCount--;
            if (crystalTurnCount <= 0)
            {
                isCrystalize = false;
                crystalize.SetActive(false);
            }
        }

        charHealthSlider.value -= enemyDmg;
        enemyState = "Attack";
        currentEnemyList[value].GetComponentInChildren<EnemyController>().SetCharacterState(enemyState);
        Debug.Log(currentEnemyList[value].GetComponentInChildren<EnemyController>().eText.text + " deals " + enemyDmg + " dmg\n Player has " + charHealthSlider.value.ToString() + " health left");
    }

    public bool Weakness()
    {
        List<string> weakHolder = currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().weak;

        if ((weakHolder.Contains(rune1) || (weakHolder.Contains(rune2))))
        {
            return true;
        }

        return false;
    }

    public bool Resistant()
    {
        List<string> resistHolder = currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().resist;

        if ((resistHolder.Contains(rune1) || (resistHolder.Contains(rune2))))
        {
            return true;
        }
        return false;
    }
    
    public bool Immunity()
    {
        List<string> immuneHolder = currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().immune;

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
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().isPoisoned = true;
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().poisonTurnCount = 2;
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().poisoned.SetActive(true);
            Debug.Log(currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().eText.text + " has been poisoned");
        }
        else if (rune1 == "Wind" && rune2 == "Earth")
        {
            index = 2;
            if (!Resistant())
            {
                currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().isExposed = true;
                currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().exposedTurnCount = 2;
                currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().exposed.SetActive(true);
                Debug.Log(currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().eText.text + " has become vulnerable");
            }
        }
        else if (rune1 == "Water" && rune2 == "Fire")
        {
            index = 3;
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().isMelt = true;
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().meltTurnCount = 3;
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().melted.SetActive(true);
            Debug.Log(currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().eText.text + " has melted");
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
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().isFreeze = true;
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().freezeTurnCount = 2;
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().frozen.SetActive(true);
            Debug.Log(currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().eText.text + " is frozen");
            //if (ChanceStatusEffect(0.7f))
            //{

            //}
        }
        else if (rune1 == "Fire" && rune2 == "Wind")
        {
            index = 7;
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().isBurn = true;
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().burnTurnCount = 3;
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().burned.SetActive(true);
            Debug.Log(currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().eText.text + " has been burned");
        }
        else if (rune1 == "Earth" && rune2 == "Wind")
        {
            index = 8;
            isReverb = true;
            reverb.SetActive(true);
            Debug.Log("Reverb effect " + reverbTurnCount);
        }
        else if (rune1 == "Fire" && rune2 == "Water")
        {
            index = 9;
            charHealthSlider.value += 3;
            if (charHealthSlider.value > 50)
            {
                charHealthSlider.value = 50;
            }
            Debug.Log("Character Health: " + charHealthSlider.value.ToString());
        }
        else if (rune1 == "Earth" && rune2 == "Water")
        {
            index = 10;
        }
        else if (rune1 == "Earth" && rune2 == "Fire")
        {
            index = 11;
            isCrystalize = true;
            crystalize.SetActive(true);
            crystalTurnCount = 2;
            Debug.Log("You crystalized");
        }

        GameObject sPrefab;

        sPrefab = Instantiate(spellPrefab[index], currentEnemyList[targetEnemy].transform);
        sPrefab.GetComponent<SpellCreation>().spell = spellList[index];
        sPrefab.GetComponent<SpellCreation>().spellName = spellList[index].sName;
        sPrefab.GetComponent<SpellCreation>().damage = spellList[index].sDamage;

        spellText.text = "You created " + sPrefab.GetComponent<SpellCreation>().spellName;

        if ((currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().isExposed) && (currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().exposedTurnCount == 1))
        {
            if (!Weakness() || CheckNeutral())
            {
                sPrefab.GetComponent<SpellCreation>().damage *= 2;
            }
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().isExposed = false;
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().exposed.SetActive(false);
        }
        else
        {
            for (int i = 0; i < currentEnemyList.Count; i++)
            {
                if ((currentEnemyList[i].GetComponentInChildren<EnemyController>().isExposed) && (currentEnemyList[i].GetComponentInChildren<EnemyController>().exposedTurnCount > 0))
                {
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().exposedTurnCount--;
                    if (currentEnemyList[i].GetComponentInChildren<EnemyController>().exposedTurnCount <= 0)
                    {
                        currentEnemyList[i].GetComponentInChildren<EnemyController>().isExposed = false;
                        currentEnemyList[i].GetComponentInChildren<EnemyController>().exposed.SetActive(false);
                    }
                }
            }
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
        currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().enemyHealthSlider.value -= damage;
        Debug.Log(currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().eText.text + " current health: " + currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().enemyHealthSlider.value.ToString());
    }

    public bool ChanceStatusEffect(float chance)
    {
        if (Random.value >= chance)
        {
            return true;
        }
        return false;
    }

    public void CheckMultipleEnemies()
    {
        int cloudCount = 0;
        int hellfireCount = 0;
        int whaleCount = 0;

        int cloudTag = 1;
        int hellfireTag = 1;
        int whaleTag = 1;

        for (int i = 0; i < currentEnemyList.Count; i++)
        {
            if (currentEnemyList[i].tag == "Cloud")
            {
                cloudCount++;
            }
            else if (currentEnemyList[i].tag == "Hellfire")
            {
                hellfireCount++;
            }
            else if (currentEnemyList[i].tag == "Whale")
            {
                whaleCount++;
            }
        }

        for (int i = 0; i < currentEnemyList.Count; i++)
        {
            if (currentEnemyList[i].tag == "Cloud")
            {
                if (cloudCount >= 2)
                {
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().eText.text = "Cloud " + cloudTag.ToString();
                    cloudTag++;
                }
            }
            else if (currentEnemyList[i].tag == "Hellfire")
            {
                if (hellfireCount >= 2)
                {
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().eText.text = "Hellfire " + hellfireTag.ToString();
                    hellfireTag++;
                }
            }
            else if (currentEnemyList[i].tag == "Whale")
            {
                if (whaleCount >= 2)
                {
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().eText.text = "Whale " + whaleTag.ToString();
                    whaleTag++;
                }
            }
        }
    }

    public bool IsAllEnemiesDead()
    {
        if (currentEnemyList.Count.Equals(0))
        {
            return true;
        }
        return false;
    }

    public void EnemyPoison()
    {
        charHealthSlider.value -= 3;
        Debug.Log("Player took dmg from poison. \nPlayer Health: " + charHealthSlider.value);
        charPoisonedTurnCount--;
        if (charPoisonedTurnCount <= 0)
        {
            isCharPoisoned = false;
            playerPoison.SetActive(false);
        }
    }

    public void EnemyCurse()
    {
        float damage = charHealthSlider.value * (1f / 8f);
        if (damage <= 0)
        {
            damage = 1;
        }
        charHealthSlider.value -= damage;
        Debug.Log("Player took " + (int)damage + " dmg from curse. \nPlayer Health: " + charHealthSlider.value);
        charCursedTurnCount--;
        if (charCursedTurnCount <= 0)
        {
            isCharCursed = false;
            curse.SetActive(false);
        }
    }

    public void EnemySeal()
    {
        buttonObjs[sealedRuneIndex].GetComponent<Button>().interactable = false;
        charSealedTurnCount--;
        if (charSealedTurnCount <= 0)
        {
            isCharSealed = false;
            seal.SetActive(false);
            buttonObjs[sealedRuneIndex].GetComponent<Button>().interactable = true;
        }
    }

    public void ChangeTarget(int index)
    {
        currentEnemyList[index].GetComponentInChildren<EnemyController>().targetSelected.SetActive(false);
    }

    /*IEnumerator FadeInOpponents(int steps = 10)
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
    }*/
}