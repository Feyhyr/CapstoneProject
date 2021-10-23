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

        enemyState = currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().currentState;

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

        enemyState = currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().currentState;

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
        currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().targetSelected.SetActive(true);
    }

    IEnumerator PAttackPhase()
    {
        yield return new WaitForSeconds(0.5f);
        enemyState = "Idle";
        currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().SetCharacterState(enemyState);

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
        currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().targetSelected.SetActive(false);
        runeCover.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        enemyTurnUX.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        enemyTurnUX.SetActive(false);
        yield return new WaitForSeconds(1);
        for (int i = 0; i < currentEnemyList.Count; i++)
        {
            if (charHealthSlider.value <= 0)
            {
                battleState = BattleState.LOSE;
                if (bossBattle)
                {
                    EndBossBattle();
                }
                else
                {
                    EndNormalBattle();
                }
            }

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

            if (charHealthSlider.value <= 0)
            {
                battleState = BattleState.LOSE;
                if (bossBattle)
                {
                    EndBossBattle();
                }
                else
                {
                    EndNormalBattle();
                }
            }

            if (currentEnemyList[i].GetComponentInChildren<EnemyController>().isPoisoned)
            {
                EnemyDamage(3, i);
                DamagePopup(currentEnemyList[i].transform, 3, "normal", false);
                currentEnemyList[i].GetComponentInChildren<EnemyController>().poisonTurnCount--;
                if (currentEnemyList[i].GetComponentInChildren<EnemyController>().poisonTurnCount <= 0)
                {
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().isPoisoned = false;
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().poisoned.SetActive(false);
                }
            }

            if (currentEnemyList[i].GetComponentInChildren<EnemyController>().isScald)
            {
                EnemyDamage(1, i);
                DamagePopup(currentEnemyList[i].transform, 1, "normal", false);
                currentEnemyList[i].GetComponentInChildren<EnemyController>().scaldTurnCount--;
                if (currentEnemyList[i].GetComponentInChildren<EnemyController>().scaldTurnCount <= 0)
                {
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().isScald = false;
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().scalded.SetActive(false);
                }
            }

            if (currentEnemyList[i].GetComponentInChildren<EnemyController>().isBurn)
            {
                EnemyDamage(1, i);
                DamagePopup(currentEnemyList[i].transform, 1, "normal", false);
                currentEnemyList[i].GetComponentInChildren<EnemyController>().burnTurnCount--;
                if (currentEnemyList[i].GetComponentInChildren<EnemyController>().burnTurnCount <= 0)
                {
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().isBurn = false;
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().burned.SetActive(false);
                }
            }

            currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyAttacking = false;

            if (currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyHealthSlider.value <= 0)
            {
                yield return new WaitForSeconds(1);
                currentEnemyList[i].SetActive(false);
                currentEnemyList.RemoveAt(i);

                if (IsAllEnemiesDead())
                {
                    battleState = BattleState.WIN;
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
                    currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().targetSelected.SetActive(true);
                    enemyIndex = 0;

                    for (int j = 0; j < currentEnemyList.Count; j++)
                    {
                        currentEnemyList[j].GetComponentInChildren<EnemyController>().enemyId = enemyIndex;
                        enemyIndex++;
                    }
                }
            }
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
            waveCount++;
            PlayerPrefs.SetInt(prefWave, waveCount);
        }
        else if (battleState == BattleState.LOSE)
        {
            gameLoseScreen.SetActive(true);
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

        if (Immunity())
        {
            damage = 0;
        }
        else
        {
            if (Weakness() && !CheckNeutral())
            {
                state = "critical";
                if (isDrowned)
                {
                    Debug.Log("Damage triple");
                    damage *= 3;
                    isDrowned = false;
                }
                else
                {
                    Debug.Log("Damage double");
                    damage *= 2;
                }
            }
            else if (Resistant() && !CheckNeutral())
            {
                state = "weak";
                Debug.Log("Damage half");
                damage /= 2;
            }
            else
            {
                Debug.Log("Damage normal");
            }

            if (isMelt)
            {
                if ((charHealthSlider.value / charMaxHealth) <= 0.1)
                {
                    Debug.Log("Attack is doubled from melting point");
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

            if ((currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().isScald) && ((rune1 == "Fire") || (rune2 == "Fire")) && (currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().scaldTurnCount <= 2))
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
        EnemyDamage(damage, targetEnemy);
        DamagePopup(currentEnemyList[targetEnemy].transform, damage, state, false);
        enemyState = "Damage";
        currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().SetCharacterState(enemyState);
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

    #region Weakness Check
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
            Debug.Log("Player gains an extra turn");
            extraTurn = true;
        }
        else if (ChooseSpell() == 1)
        {
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().isPoisoned = true;
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().poisonTurnCount = 2;
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().poisoned.SetActive(true);
            Debug.Log(currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().eText.text + " has been poisoned");
        }
        else if (ChooseSpell() == 2)
        {
            if (!Resistant())
            {
                currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().isExposed = true;
                currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().exposedTurnCount = 2;
                currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().exposed.SetActive(true);
                Debug.Log(currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().eText.text + " has become vulnerable");
            }
        }
        else if (ChooseSpell() == 3)
        {
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().isScald = true;
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().scaldTurnCount = 3;
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().scalded.SetActive(true);
            Debug.Log(currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().eText.text + " has been scalded");
        }
        else if (ChooseSpell() == 4)
        {
            isDrowned = true;
            Debug.Log(currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().eText.text + " has drowned");
        }
        else if (ChooseSpell() == 5)
        {
            isMelt = true;
        }
        else if (ChooseSpell() == 6)
        {
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().isFreeze = true;
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().freezeTurnCount = 2;
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().frozen.SetActive(true);
            Debug.Log(currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().eText.text + " is frozen");
            //if (ChanceStatusEffect(0.7f))
            //{

            //}
        }
        else if (ChooseSpell() == 7)
        {
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().isBurn = true;
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().burnTurnCount = 3;
            currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().burned.SetActive(true);
            Debug.Log(currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().eText.text + " has been burned");
        }
        else if (ChooseSpell() == 8)
        {
            isReverb = true;
            reverb.SetActive(true);
            Debug.Log("Reverb effect " + reverbTurnCount);
        }
        else if (ChooseSpell() == 9)
        {
            charHealthSlider.value += 3;
            if (charHealthSlider.value > 500)
            {
                charHealthSlider.value = 500;
            }
            DamagePopup(playerLocation, 3, "normal", true);
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
            Debug.Log("You crystalized");
        }

        GameObject sPrefab;

        sPrefab = Instantiate(spellPrefab[ChooseSpell()], currentEnemyList[targetEnemy].transform);
        sPrefab.GetComponent<SpellCreation>().spell = spellList[ChooseSpell()];
        sPrefab.GetComponent<SpellCreation>().spellName = spellList[ChooseSpell()].sName;
        sPrefab.GetComponent<SpellCreation>().damage = spellList[ChooseSpell()].sDamage;

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
                if (whaleCount >= 2)
                {
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().eText.text = "Whale " + whaleTag.ToString();
                    whaleTag++;
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
            else if (currentEnemyList[i].tag == "Tree")
            {
                if (treeCount >= 2)
                {
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().eText.text = "Tree " + treeTag.ToString();
                    treeTag++;
                }
            }
            else if (currentEnemyList[i].tag == "Human")
            {
                if (humanCount >= 2)
                {
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().eText.text = "Human " + humanTag.ToString();
                    humanTag++;
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

    public bool ChanceStatusEffect(float chance)
    {
        if (Random.value >= chance)
        {
            return true;
        }
        return false;
    }

    public void DamagePopup(Transform location, int damage, string state, bool isHeal)
    {
        GameObject damagePopup = Instantiate(numberPopupObj, location);
        damagePopup.GetComponent<NumberPopupController>().Setup(damage, state, isHeal);
    }
}