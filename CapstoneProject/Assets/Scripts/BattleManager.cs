using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    #region Variables
    public enum BattleState { START, PLAYERTURN, ENEMYTURN, WIN, LOSE }
    public BattleState battleState;

    public Transform playerLocation;

    public GameObject spellButtonPrefab;
    public GameObject runePrefab;
    public Transform runeLocation;
    public GameObject runeCover;

    public List<GameObject> spellPrefab;

    public List<RuneSO> runeList;
    public List<SpellSO> spellList;

    public GameObject spellMaker;
    public Image firstRune;
    public Image secondRune;
    public Transform spellButtonLocation;

    public string rune1;
    public string rune2;
    public int runeIndex;
    int sealedRuneIndex;

    public GameObject[] buttonObjs;

    public int charMaxHealth;
    public Slider charHealthSlider;

    int randomEnemyCount;
    int randomEnemy;
    int enemyIndex = 0;
    public List<GameObject> enemyPrefab;
    public List<GameObject> currentEnemyList;
    public GameObject targetEnemyPrefab;
    public int targetEnemy = 0;
    public Transform enemyLocation;

    string enemyState;

    public bool playerAttacked = false;

    public GameObject numberPopupObj;

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
    bool extraTurn = false;
    bool isDrowned = false;
    bool isMelt = false;
    bool debrisHit = false;

    bool bossBattle;

    public const string prefWave = "prefWave";
    public Text waveCounterText;
    public int waveCount;

    public GameObject gameWinScreen;
    public GameObject gameLoseScreen;
    public GameObject battleStartUX;
    public GameObject playerTurnUX;
    public GameObject enemyTurnUX;

    public GameObject creationPrefab;

    public GameObject freezePrefab;

    public GameObject cameraObject;

    EnemyController enemy;
    bool startCheckEnemy;
    #endregion

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
            bossBattle = true;
            waveCounterText.text = "Boss Wave";
        }
        else
        {
            waveCounterText.text = "Wave " + waveCount;
        }

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

        if (bossBattle)
        {
            StartCoroutine(BeginBossBattle());
        }
        else
        {
            StartCoroutine(BeginNormalBattle());
        }
    }
    
    #region Turn Phases
    IEnumerator BeginNormalBattle()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        battleStartUX.SetActive(true);
        yield return new WaitForSeconds(1.8f);
        battleStartUX.SetActive(false);

        GameObject ePrefab;

        enemyIndex = 0;
        randomEnemyCount = Random.Range(1, 4);
        Debug.Log("Random enemy count: " + randomEnemyCount);

        for (int i = 0; i < randomEnemyCount; i++)
        {
            randomEnemy = Random.Range(0, enemyPrefab.Count - 1);
            ePrefab = Instantiate(enemyPrefab[randomEnemy], enemyLocation);

            ePrefab.GetComponentInChildren<EnemyController>().bm = this;
            ePrefab.GetComponentInChildren<EnemyController>().enemyId = enemyIndex;
            enemyIndex++;
            currentEnemyList.Add(ePrefab);
        }

        CheckMultipleEnemies();

        startCheckEnemy = true;
        yield return new WaitForSeconds(0.1f);
        enemyState = enemy.currentState;

        battleState = BattleState.PLAYERTURN;

        yield return PlayerTurn();
    }
    
    IEnumerator BeginBossBattle()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        battleStartUX.SetActive(true);
        yield return new WaitForSeconds(1.8f);
        battleStartUX.SetActive(false);

        GameObject ePrefab;

        ePrefab = Instantiate(enemyPrefab[enemyPrefab.Count - 1], enemyLocation);

        ePrefab.GetComponentInChildren<EnemyController>().bm = this;
        ePrefab.GetComponentInChildren<EnemyController>().enemyId = 0;
        currentEnemyList.Add(ePrefab);

        startCheckEnemy = true;
        yield return new WaitForSeconds(0.1f);
        enemyState = enemy.currentState;

        battleState = BattleState.PLAYERTURN;

        yield return PlayerTurn();
    }

    IEnumerator PlayerTurn()
    {
        if (extraTurn && isCrystalize)
        {
            crystalTurnCount--;
            if (crystalTurnCount <= 0)
            {
                isCrystalize = false;
                crystalize.SetActive(false);
            }
        }
        extraTurn = false;
        yield return new WaitForSeconds(1);

        playerAttacked = false;
        playerTurnUX.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        playerTurnUX.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        firstRune.color = Color.white;
        secondRune.color = Color.white;
        spellMaker.SetActive(true);
        runeCover.SetActive(false);
        enemy.targetSelected.SetActive(true);
    }

    IEnumerator PAttackPhase()
    {
        yield return new WaitForSeconds(0.5f);
        enemyState = "Idle";
        enemy.SetCharacterState(enemyState);

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

        yield return new WaitForSeconds(1);
        CheckEnemyDeath(targetEnemy);

        if (isCharCursed)
        {
            EnemyCurse();
        }

        if (isCharPoisoned)
        {
            EnemyPoison();
        }

        CheckPlayerDeath();

        if (extraTurn)
        {
            battleState = BattleState.PLAYERTURN;
            StartCoroutine(PlayerTurn());
        }
        else
        {
            battleState = BattleState.ENEMYTURN;
            yield return EnemyTurn();
        }
    }

    IEnumerator EnemyTurn()
    {
        enemy.targetSelected.SetActive(false);
        runeCover.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        enemyTurnUX.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        enemyTurnUX.SetActive(false);
        yield return new WaitForSeconds(1);
        for (int i = 0; i < currentEnemyList.Count; i++)
        {
            CheckPlayerDeath();

            if (currentEnemyList[i].GetComponentInChildren<EnemyController>().isFreeze && currentEnemyList[i].GetComponentInChildren<EnemyController>().freezeTurnCount > 0)
            {
                Debug.Log(currentEnemyList[i].GetComponentInChildren<EnemyController>().eText.text + " cannot move");
                GameObject fPrefab = Instantiate(freezePrefab, currentEnemyList[i].transform);
                yield return new WaitForSeconds(2);
                Destroy(fPrefab);
                currentEnemyList[i].GetComponentInChildren<EnemyController>().freezeTurnCount--;
                if (currentEnemyList[i].GetComponentInChildren<EnemyController>().freezeTurnCount == 0)
                {
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().isFreeze = false;
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().frozen.SetActive(false);
                }
            }

            else
            {
                currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyAttacking = true;
                cameraObject.GetComponent<ScreenShake>().TriggerShake();
                EnemyAttack(i);
                if (currentEnemyList[i].tag == "Human" || currentEnemyList[i].tag == "Boss")
                {
                    if (ChanceStatusEffect(0.7f))
                    {
                        Debug.Log("Player is cursed");
                        isCharCursed = true;
                        charCursedTurnCount = 2;
                        curse.SetActive(true);
                    }
                }
                else if (currentEnemyList[i].tag == "Tree")
                {
                    if (ChanceStatusEffect(0.7f))
                    {
                        Debug.Log("Player is poisoned");
                        isCharPoisoned = true;
                        charPoisonedTurnCount = 2;
                        playerPoison.SetActive(true);
                    }
                }
                else if (currentEnemyList[i].tag == "Human" || currentEnemyList[i].tag == "Boss")
                {
                    if (!isCharSealed)
                    {
                        if (ChanceStatusEffect(0.7f))
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
            }

            yield return new WaitForSeconds(1);
            enemyState = "Idle";
            currentEnemyList[i].GetComponentInChildren<EnemyController>().SetCharacterState(enemyState);

            CheckPlayerDeath();

            if (currentEnemyList[i].GetComponentInChildren<EnemyController>().isPoisoned)
            {
                StatusTurnChange(3, i, ref currentEnemyList[i].GetComponentInChildren<EnemyController>().poisonTurnCount, ref currentEnemyList[i].GetComponentInChildren<EnemyController>().isPoisoned, currentEnemyList[i].GetComponentInChildren<EnemyController>().poisoned);
            }

            if (currentEnemyList[i].GetComponentInChildren<EnemyController>().isScald)
            {
                StatusTurnChange(0, i, ref currentEnemyList[i].GetComponentInChildren<EnemyController>().scaldTurnCount, ref currentEnemyList[i].GetComponentInChildren<EnemyController>().isScald, currentEnemyList[i].GetComponentInChildren<EnemyController>().scalded);
            }

            if (currentEnemyList[i].GetComponentInChildren<EnemyController>().isBurn)
            {
                StatusTurnChange(1, i, ref currentEnemyList[i].GetComponentInChildren<EnemyController>().burnTurnCount, ref currentEnemyList[i].GetComponentInChildren<EnemyController>().isBurn, currentEnemyList[i].GetComponentInChildren<EnemyController>().burned);
            }

            currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyAttacking = false;

            yield return new WaitForSeconds(1);
            CheckEnemyDeath(i);
        }

        if (isCrystalize)
        {
            crystalTurnCount--;
            if (crystalTurnCount <= 0)
            {
                isCrystalize = false;
                crystalize.SetActive(false);
            }
        }
        
        battleState = BattleState.PLAYERTURN;
        yield return PlayerTurn();
    }
    #endregion

    #region End Battles
    private void EndNormalBattle()
    {
        StopAllCoroutines();
        if (battleState == BattleState.WIN)
        {
            gameWinScreen.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            waveCount++;
            PlayerPrefs.SetInt(prefWave, waveCount);
        }
        else if (battleState == BattleState.LOSE)
        {
            gameLoseScreen.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void EndBossBattle()
    {
        StopAllCoroutines();
        if (battleState == BattleState.WIN)
        {
            waveCount++;
            PlayerPrefs.SetInt(prefWave, waveCount);
            SceneManager.LoadScene("GameWinScene");
        }
        else if (battleState == BattleState.LOSE)
        {
            gameLoseScreen.SetActive(true);
        }
    }
    #endregion

    #region Attack Functions
    public void PlayerAttack(int damage)
    {
        string state = "normal";

        if (CheckWeakness(enemy.immune))
        {
            damage = 0;
        }
        else
        {
            if (CheckWeakness(enemy.weak) && !CheckNeutral())
            {
                state = "critical";
                if (isDrowned)
                {
                    damage *= 3;
                    isDrowned = false;
                }
                else
                {
                    damage *= 2;
                }
            }
            else if (CheckWeakness(enemy.resist) && !CheckNeutral())
            {
                state = "weak";
                damage /= 2;
            }

            if (isMelt)
            {
                if ((charHealthSlider.value / charMaxHealth) <= 0.1)
                {
                    damage *= 2;
                }
                isMelt = false;
            }

            if (debrisHit)
            {
                int debrisDmg = Random.Range(2, 11);
                damage += debrisDmg;
                debrisHit = false;
            }

            if ((enemy.isScald) && ((rune1 == "Fire") || (rune2 == "Fire")) && (enemy.scaldTurnCount <= 3))
            {
                damage += 2;
            }

            if ((enemy.isBurn) && ((rune1 == "Wind") || (rune2 == "Wind")) && ((rune1 != "Water") && (rune2 != "Water")) && (enemy.burnTurnCount <= 2))
            {
                damage += 2;
            }

            if (isReverb)
            {
                damage += reverbTurnCount;
                reverbTurnCount++;
            }
        }
        EnemyDamage(damage, targetEnemy);
        DamagePopup(currentEnemyList[targetEnemy].transform, damage, state, false);
        enemyState = "Damage";
        enemy.SetCharacterState(enemyState);
        playerAttacked = true;
    }

    public void EnemyAttack(int value)
    {
        AudioManager.Instance.Play(currentEnemyList[value].GetComponentInChildren<EnemyController>().attackSFX);
        int enemyDmg = currentEnemyList[value].GetComponentInChildren<EnemyController>().atk;
        if (isCrystalize)
        {
            enemyDmg /= 2;
        }

        charHealthSlider.value -= enemyDmg;
        DamagePopup(playerLocation, enemyDmg, "normal", false);
        enemyState = "Attack";
        currentEnemyList[value].GetComponentInChildren<EnemyController>().SetCharacterState(enemyState);
    }

    public void ChangeTarget(int index)
    {
        currentEnemyList[index].GetComponentInChildren<EnemyController>().targetSelected.SetActive(false);
    }
    #endregion

    #region State Check
    public bool CheckWeakness(List<string> type)
    {
        List<string> statusHolder = type;

        if ((statusHolder.Contains(rune1) || (statusHolder.Contains(rune2))))
        {
            return true;
        }
        return false;
    }

    public bool CheckNeutral()
    {
        if (CheckWeakness(enemy.weak) && CheckWeakness(enemy.resist))
        {
            return true;
        }
        return false;
    }

    public bool ChanceStatusEffect(float chance)
    {
        if (Random.value >= chance)
        {
            return true;
        }
        return false;
    }

    public void StatusTurnChange(int dmg, int index, ref int count, ref bool status, GameObject obj)
    {
        if (dmg > 0)
        {
            EnemyDamage(dmg, index);
            DamagePopup(currentEnemyList[index].transform, dmg, "normal", false);
        }
        count--;
        if (count <= 0)
        {
            status = false;
            obj.SetActive(false);
        }
    }

    public void CheckPlayerDeath()
    {
        if (charHealthSlider.value <= 0)
        {
            battleState = BattleState.LOSE;
            startCheckEnemy = false;
            if (bossBattle)
            {
                EndBossBattle();
            }
            else
            {
                EndNormalBattle();
            }
        }
    }

    public void CheckEnemyDeath(int index)
    {
        if (currentEnemyList[index].GetComponentInChildren<EnemyController>().enemyHealthSlider.value <= 0)
        {
            currentEnemyList[index].SetActive(false);
            currentEnemyList.RemoveAt(index);

            if (IsAllEnemiesDead())
            {
                battleState = BattleState.WIN;
                startCheckEnemy = false;
                if (bossBattle)
                {
                    EndBossBattle();
                }
                else
                {
                    EndNormalBattle();
                }
            }
            else
            {
                targetEnemy = 0;
                enemy.targetSelected.SetActive(true);
                enemyIndex = 0;

                for (int i = 0; i < currentEnemyList.Count; i++)
                {
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyId = enemyIndex;
                    enemyIndex++;
                }
            }
        }
    }
    #endregion

    #region Spell Creation
    public int ChooseSpell()
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

        return index;
    }

    public IEnumerator SpellChosen()
    {
        GameObject createObj = Instantiate(creationPrefab, spellButtonLocation);
        yield return new WaitForSeconds(1f);
        Destroy(createObj);
        spellMaker.SetActive(false);
        GameObject sbPrefab = Instantiate(spellButtonPrefab, spellButtonLocation);
        sbPrefab.GetComponent<SpellController>().bm = this;
        sbPrefab.GetComponent<SpellController>().nameText.text = spellList[ChooseSpell()].sName;
        sbPrefab.GetComponent<SpellController>().spellIcon = spellList[ChooseSpell()].icon;
        sbPrefab.GetComponent<Button>().image.sprite = sbPrefab.GetComponent<SpellController>().spellIcon;
        sbPrefab.GetComponent<SpellController>().spellDescription = spellList[ChooseSpell()].description;
    }

    public void CreateSpell()
    {
        if (ChooseSpell() == 0)
        {
            extraTurn = true;
        }
        else if (ChooseSpell() == 1)
        {
            enemy.isPoisoned = true;
            enemy.poisonTurnCount = 2;
            enemy.poisoned.SetActive(true);
        }
        else if (ChooseSpell() == 2)
        {
            if (!CheckWeakness(enemy.resist))
            {
                enemy.isExposed = true;
                enemy.exposedTurnCount = 2;
                enemy.exposed.SetActive(true);
            }
        }
        else if (ChooseSpell() == 3)
        {
            enemy.isScald = true;
            enemy.scaldTurnCount = 4;
            enemy.scalded.SetActive(true);
        }
        else if (ChooseSpell() == 4)
        {
            isDrowned = true;
        }
        else if (ChooseSpell() == 5)
        {
            isMelt = true;
        }
        else if (ChooseSpell() == 6)
        {
            if (ChanceStatusEffect(0.8f))
            {
                enemy.isFreeze = true;
                enemy.freezeTurnCount = 2;
                enemy.frozen.SetActive(true);
            }
        }
        else if (ChooseSpell() == 7)
        {
            if (ChanceStatusEffect(0.7f))
            {
                enemy.isBurn = true;
                enemy.burnTurnCount = 3;
                enemy.burned.SetActive(true);
            }
        }
        else if (ChooseSpell() == 8)
        {
            isReverb = true;
            reverb.SetActive(true);
        }
        else if (ChooseSpell() == 9)
        {
            charHealthSlider.value += 3;
            if (charHealthSlider.value > 500)
            {
                charHealthSlider.value = 500;
            }
            DamagePopup(playerLocation, 3, "normal", true);
            isCharSealed = false;
            isCharCursed = false;
            isCharPoisoned = false;
            seal.SetActive(false);
            curse.SetActive(false);
            playerPoison.SetActive(false);
        }
        else if (ChooseSpell() == 10)
        {
            debrisHit = true;
        }
        else if (ChooseSpell() == 11)
        {
            isCrystalize = true;
            crystalize.SetActive(true);
            crystalTurnCount = 2;
        }

        GameObject sPrefab;

        sPrefab = Instantiate(spellPrefab[ChooseSpell()], currentEnemyList[targetEnemy].transform);
        sPrefab.GetComponent<SpellCreation>().spell = spellList[ChooseSpell()];
        sPrefab.GetComponent<SpellCreation>().spellName = spellList[ChooseSpell()].sName;
        sPrefab.GetComponent<SpellCreation>().damage = spellList[ChooseSpell()].sDamage;

        if ((enemy.isExposed) && (enemy.exposedTurnCount == 1))
        {
            if (!CheckWeakness(enemy.weak) || CheckNeutral())
            {
                sPrefab.GetComponent<SpellCreation>().damage *= 2;
            }
            enemy.isExposed = false;
            enemy.exposed.SetActive(false);
        }
        else
        {
            for (int i = 0; i < currentEnemyList.Count; i++)
            {
                if ((currentEnemyList[i].GetComponentInChildren<EnemyController>().isExposed) && (currentEnemyList[i].GetComponentInChildren<EnemyController>().exposedTurnCount > 0))
                {
                    StatusTurnChange(0, i, ref currentEnemyList[i].GetComponentInChildren<EnemyController>().exposedTurnCount, ref currentEnemyList[i].GetComponentInChildren<EnemyController>().isExposed, currentEnemyList[i].GetComponentInChildren<EnemyController>().exposed);
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
    #endregion

    #region Enemy Status
    public void EnemyDamage(int damage, int target)
    {
        currentEnemyList[target].GetComponentInChildren<EnemyController>().enemyHealthSlider.value -= damage;
    }

    public void CheckMultipleEnemies()
    {
        int whaleCount = 0;
        int hellfireCount = 0;
        int treeCount = 0;
        int humanCount = 0;

        int whaleTag = 1;
        int hellfireTag = 1;
        int treeTag = 1;
        int humanTag = 1;


        for (int i = 0; i < currentEnemyList.Count; i++)
        {
            if (currentEnemyList[i].tag == "Whale")
            {
                whaleCount++;
            }
            else if (currentEnemyList[i].tag == "Hellfire")
            {
                hellfireCount++;
            }
            else if (currentEnemyList[i].tag == "Tree")
            {
                treeCount++;
            }
            else if (currentEnemyList[i].tag == "Human")
            {
                humanCount++;
            }
        }

        for (int i = 0; i < currentEnemyList.Count; i++)
        {
            if (currentEnemyList[i].tag == "Whale")
            {
                CheckEnemyCount(ref whaleCount, ref whaleTag, i, "Whale");
            }
            else if (currentEnemyList[i].tag == "Hellfire")
            {
                CheckEnemyCount(ref hellfireCount, ref hellfireTag, i, "Hellfire");
            }
            else if (currentEnemyList[i].tag == "Tree")
            {
                CheckEnemyCount(ref treeCount, ref treeTag, i, "Tree");
            }
            else if (currentEnemyList[i].tag == "Human")
            {
                CheckEnemyCount(ref humanCount, ref humanTag, i, "Human");
            }
        }
    }

    public void CheckEnemyCount(ref int count, ref int tag, int index, string name)
    {
        if (count >= 2)
        {
            currentEnemyList[index].GetComponentInChildren<EnemyController>().eText.text = name + " " + tag.ToString();
            tag++;
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
        DamagePopup(playerLocation, 3, "normal", false);
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
        DamagePopup(playerLocation, (int)damage, "normal", false);
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
    #endregion

    #region Visuals
    public void DamagePopup(Transform location, int damage, string state, bool isHeal)
    {
        GameObject damagePopup = Instantiate(numberPopupObj, location);
        damagePopup.GetComponent<NumberPopupController>().Setup(damage, state, isHeal);
    }
    #endregion

    private void Update()
    {
        if (startCheckEnemy)
        {
            enemy = currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>();
        }
    }
}