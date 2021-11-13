using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Spine.Unity;

public class BattleManager : MonoBehaviour
{
    #region Variables
    public enum BattleState { START, PLAYERTURN, ENEMYTURN, WIN, LOSE }
    public BattleState battleState;

    public Transform playerLocation;
    public Transform playerLocation2;
    public GameObject playerShakeObject;

    public GameObject spellButtonPrefab;
    public GameObject runePrefab;
    public Transform runeLocation;

    public List<GameObject> spellPrefab;

    public List<RuneSO> runeList;
    public List<GameObject> spellBTNList;

    public Transform spellButtonLocation;

    public string rune1;
    public string rune2;
    public int runeIndex;
    public int sealedRuneIndex;

    public GameObject[] runeObjs;

    public int charMaxHealth;
    public Slider charHealthSlider;
    public Text charHealthText;

    int randomEnemyCount;
    int randomEnemy;
    int enemyIndex = 0;
    public List<BasicEnemyList> enemyScriptables = new List<BasicEnemyList>();
    public List<EnemySO> bossScriptables;
    public List<EnemySO> enemySummonScriptables;
    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    public List<GameObject> currentEnemyList;
    public int targetEnemy = 0;
    public Transform enemyLocation;

    string enemyState;

    public bool playerAttacked = false;
    GameObject sPrefab;
    bool debuffing = false;

    public GameObject enemyNumPopupObj;
    public GameObject playerNumPopupObj;

    public GameObject crystalize;
    public GameObject reverb;
    public GameObject seal;
    public GameObject curse;
    public GameObject playerPoison;
    public GameObject charStuck;

    public bool isCrystalize = false;
    int crystalTurnCount = 2;
    bool isReverb = false;
    int reverbTurnCount = 2;
    public bool isCharSealed = false;
    public int charSealedTurnCount = 2;
    public bool isCharCursed = false;
    public int charCursedTurnCount = 2;
    public bool isCharPoisoned = false;
    public int charPoisonedTurnCount = 2;
    bool extraTurn = false;
    bool isDrowned = false;
    bool isMelt = false;
    bool debrisHit = false;
    bool isAOE = false;
    bool isCreatingSpell = false;
    public bool pCannotHeal = false;
    public int charStuckTurnCount = 1;
    public bool isCharStuck = false;

    bool bossBattle;

    public GameObject gameWinScreen;
    public GameObject gameLoseScreen;
    public GameObject battleStartUX;
    public Text battleStartUXText;
    public GameObject playerTurnUX;
    public GameObject enemyTurnUX;

    public GameObject creationPrefab;
    public GameObject freezePrefab;
    public GameObject stuckPrefab;
    public GameObject cancelBTN;
    public GameObject spellOnCDObj;

    public GameObject debuffCanvas;
    public GameObject buffCanvas;

    EnemyController enemy;
    bool startCheckEnemy;

    private SpellBookMngr unlock;
    public FloorManager floor;
    public Image floorBackground;

    public AudioClip spellConfirm;
    public bool isAudioPlaying;

    public GameObject fadeInCanvas;
    public GameObject endWaveCanvas;
    #endregion

    private void Awake()
    {
        StartCoroutine(WaveStart());
    }

    private void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);

        unlock = GameObject.Find("SpellUnlockMngr").GetComponent<SpellBookMngr>();
        floor = GameObject.Find("FloorManager").GetComponent<FloorManager>();

        battleState = BattleState.START;

        if (floor.waveCount == 5)
        {
            bossBattle = true;
        }
        charHealthSlider.value = charMaxHealth;

        ChangeSpellDisabled();

        battleStartUXText.text = "Wave " + floor.waveCount.ToString() + "/5";

        if (bossBattle)
        {
            StartCoroutine(BeginBossBattle());
        }
        else
        {
            StartCoroutine(BeginNormalBattle());
        }
    }
    
    /*TO BE DELETED*/
    public void WinNormalBattle()
    {
        battleState = BattleState.WIN;
        EndNormalBattle();
    }
    /*TO BE DELETED*/
    public void WinBossBattle()
    {
        battleState = BattleState.WIN;
        EndBossBattle();
    }

    #region Turn Phases
    IEnumerator BeginNormalBattle()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        battleStartUX.SetActive(true);
        yield return new WaitForSeconds(2.3f);
        battleStartUX.SetActive(false);

        GameObject ePrefab;

        enemyIndex = 0;
        randomEnemyCount = Random.Range(1, 4);

        for (int i = 0; i < randomEnemyCount; i++)
        {
            randomEnemy = Random.Range(0, enemyScriptables[floor.floorCount - 1].enemyList.Count - 1);
            ePrefab = Instantiate(enemyPrefab, enemyLocation);

            ePrefab.GetComponentInChildren<EnemyController>().bm = this;
            ePrefab.GetComponentInChildren<EnemyController>().enemy = enemyScriptables[floor.floorCount - 1].enemyList[randomEnemy];
            ePrefab.GetComponentInChildren<EnemyController>().enemyId = enemyIndex;
            ePrefab.tag = ePrefab.GetComponentInChildren<EnemyController>().enemy.tagName;
            foreach (Transform t in ePrefab.transform)
            {
                t.gameObject.tag = ePrefab.tag;
            }
            ePrefab.GetComponentInChildren<EnemyController>().eText.text = ePrefab.GetComponentInChildren<EnemyController>().enemy.enemyName;
            enemyIndex++;
            currentEnemyList.Add(ePrefab);
        }

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
        yield return new WaitForSeconds(2.3f);
        battleStartUX.SetActive(false);

        GameObject ePrefab;

        ePrefab = Instantiate(bossPrefab, enemyLocation);

        ePrefab.GetComponentInChildren<EnemyController>().bm = this;
        ePrefab.GetComponentInChildren<EnemyController>().enemy = bossScriptables[floor.floorCount - 1];
        ePrefab.GetComponentInChildren<EnemyController>().enemyId = 0;
        ePrefab.tag = ePrefab.GetComponentInChildren<EnemyController>().enemy.tagName;
        ePrefab.GetComponentInChildren<EnemyController>().eText.text = ePrefab.GetComponentInChildren<EnemyController>().enemy.enemyName;
        currentEnemyList.Add(ePrefab);

        startCheckEnemy = true;
        yield return new WaitForSeconds(0.1f);
        enemyState = enemy.currentState;

        battleState = BattleState.PLAYERTURN;

        yield return PlayerTurn();
    }

    IEnumerator PlayerTurn()
    {
        isAudioPlaying = false;
        if (isCrystalize)
        {
            crystalTurnCount--;
            crystalize.GetComponentInChildren<Text>().text = crystalTurnCount.ToString();
            if (crystalTurnCount <= 0)
            {
                isCrystalize = false;
                crystalize.SetActive(false);
            }
        }

        for (int i = 0; i < currentEnemyList.Count; i++)
        {
            if ((currentEnemyList[i].GetComponentInChildren<EnemyController>().isExposed) && (currentEnemyList[i].GetComponentInChildren<EnemyController>().exposedTurnCount > 0))
            {
                StatusTurnChange(0, i, ref currentEnemyList[i].GetComponentInChildren<EnemyController>().exposedTurnCount, ref currentEnemyList[i].GetComponentInChildren<EnemyController>().isExposed, currentEnemyList[i].GetComponentInChildren<EnemyController>().exposed);
            }
        }

        for (int i = 0; i < spellBTNList.Count; i++)
        {
            if (spellBTNList[i].GetComponent<SpellController>().onCD)
            {
                spellBTNList[i].GetComponent<SpellController>().currentCD--;
                spellBTNList[i].GetComponent<SpellController>().counterCDText.GetComponent<Text>().text = spellBTNList[i].GetComponent<SpellController>().currentCD.ToString();
                if (spellBTNList[i].GetComponent<SpellController>().currentCD <= 0)
                {
                    spellBTNList[i].GetComponent<SpellController>().counterCDText.SetActive(false);
                    spellBTNList[i].GetComponent<SpellController>().onCD = false;
                    spellBTNList[i].GetComponent<SpellController>().CDCover.SetActive(false);
                }
            }
        }
        enemy.targetSelected.SetActive(true);
        extraTurn = false;
        isCreatingSpell = false;
        
        if (isCharStuck)
        {
            GameObject fPrefab = Instantiate(stuckPrefab, playerLocation2);
            yield return new WaitForSeconds(1);
            Destroy(fPrefab);

            if (isCharPoisoned)
            {
                yield return new WaitForSeconds(1);
                EnemyPoison();
            }

            CheckPlayerDeath();

            battleState = BattleState.ENEMYTURN;
            yield return EnemyTurn();
        }

        for (int i = 0; i < runeObjs.Length; i++)
        {
            runeObjs[i].GetComponent<RuneController>().canvasGroup.interactable = true;
            runeObjs[i].GetComponent<RuneController>().canvasGroup.alpha = 1;
        }
        if (isCharSealed)
        {
            runeObjs[sealedRuneIndex].GetComponent<RuneController>().canvasGroup.interactable = false;
            runeObjs[sealedRuneIndex].GetComponent<RuneController>().canvasGroup.alpha = 0.2f;
        }

        playerAttacked = false;
        playerTurnUX.SetActive(true);
        yield return new WaitForSeconds(1.6f);
        playerTurnUX.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    IEnumerator PAttackPhase()
    {
        sPrefab.GetComponent<SpellCreation>().DestroySpell();

        for (int i = 0; i < runeObjs.Length; i++)
        {
            runeObjs[i].GetComponent<RuneController>().transform.position = runeObjs[i].GetComponent<RuneController>().defaultPos;
            runeObjs[i].GetComponent<RuneController>().droppedOnSlot = false;
            runeObjs[i].GetComponent<RuneController>().onFirstSlot = false;
            runeObjs[i].GetComponent<RuneController>().onSecondSlot = false;
        }

        if (isReverb)
        {
            isReverb = false;
        }
        else
        {
            reverbTurnCount = 2;
            reverb.GetComponentInChildren<Text>().text = reverbTurnCount.ToString();
            reverb.SetActive(false);
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
            yield return new WaitForSeconds(1);
            EnemyCurse();
        }

        if (isCharPoisoned)
        {
            yield return new WaitForSeconds(1);
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
        isCharStuck = false;
        charStuck.SetActive(false);
        enemy.targetSelected.SetActive(false);
        enemyTurnUX.SetActive(true);
        yield return new WaitForSeconds(1.6f);
        enemyTurnUX.SetActive(false);
        yield return new WaitForSeconds(1);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        for (int i = 0; i < currentEnemyList.Count; i++)
        {
            CheckPlayerDeath();
            yield return currentEnemyList[i].GetComponentInChildren<EnemyController>().AttackPattern();
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
            StartCoroutine(EndWave());
            /*gameWinScreen.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            floor.AddCount(ref floor.waveCount, floor.prefWave);*/
        }
        else if (battleState == BattleState.LOSE)
        {
            PlayerPrefs.SetInt(floor.prefWave, 1);
            gameLoseScreen.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    IEnumerator WaveStart()
    {
        fadeInCanvas.SetActive(true);
        yield return new WaitForSeconds(1.8f);
        fadeInCanvas.SetActive(false);
    }

    IEnumerator EndWave()
    {
        endWaveCanvas.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        floor.AddCount(ref floor.waveCount, floor.prefWave);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("BattleScene");
    }

    private void EndBossBattle()
    {
        StopAllCoroutines();
        if (battleState == BattleState.WIN)
        {
            PlayerPrefs.SetInt(floor.prefWave, 1);
            if (floor.floorsUnlocked <= floor.floorCount)
            {
                floor.floorsUnlocked++;
                if (floor.floorsUnlocked > 5)
                {
                    floor.floorsUnlocked = 5;
                }
                PlayerPrefs.SetInt(floor.prefFloorUnlock, floor.floorsUnlocked);
                PlayerPrefs.SetInt("floorReached", floor.floorsUnlocked);
            }
            floor.AddCount(ref floor.floorCount, floor.prefFloor);
            SceneManager.LoadScene("GameWinScene");
        }
        else if (battleState == BattleState.LOSE)
        {
            PlayerPrefs.SetInt(floor.prefWave, 1);
            gameLoseScreen.SetActive(true);
        }
    }
    #endregion

    #region Attack Functions
    public IEnumerator PlayerAttack(float damage)
    {
        enemy.targetSelected.SetActive(false);
        if (isAOE)
        {
            yield return DamageCheckAOE(damage);
            isAOE = false;
        }
        else
        {
            yield return DamageCheckSingle(damage);
        }
        yield return PAttackPhase();
    }

    public IEnumerator DamageCheckSingle(float damage)
    {
        string state = "normalEnemy";

        if (CheckWeakness(enemy.immune))
        {
            damage = 0;
        }
        else
        {
            if ((enemy.isExposed) && (enemy.exposedTurnCount == 1))
            {
                damage *= 2;
                enemy.isExposed = false;
                enemy.exposed.SetActive(false);
            }

            if (CheckWeakness(enemy.weak) && !CheckNeutral(enemy.weak, enemy.resist))
            {
                state = "criticalEnemy";
                damage *= 2;
            }
            else if (CheckWeakness(enemy.resist) && !CheckNeutral(enemy.weak, enemy.resist))
            {
                state = "weakEnemy";
                damage /= 2;
            }

            if (debrisHit)
            {
                float debrisDmg = Random.Range(1f, 2.5f);
                damage *= debrisDmg;
                debrisHit = false;
            }

            if ((enemy.isScald) && ((rune1 == "Fire") || (rune2 == "Fire")) && (enemy.scaldTurnCount <= 3))
            {
                damage *= 1.5f;
            }

            if ((enemy.isBurn) && ((rune1 == "Wind") || (rune2 == "Wind")) && ((rune1 != "Water") && (rune2 != "Water")) && (enemy.burnTurnCount <= 2))
            {
                damage += 2;
            }

            if (isReverb)
            {
                damage *= reverbTurnCount;
                reverbTurnCount *= 2;
                reverb.GetComponentInChildren<Text>().text = reverbTurnCount.ToString();
            }
        }

        AudioManager.Instance.Play(enemy.damageSFX);
        enemyState = "Damage";
        enemy.SetCharacterState(enemyState);
        currentEnemyList[targetEnemy].GetComponentInChildren<ScreenShake>().TriggerShake();
        yield return new WaitForSeconds(0.5f);
        EDamagePopup(currentEnemyList[targetEnemy].transform, (int)damage, state, false, enemyNumPopupObj);
        EnemyDamage((int)damage, targetEnemy);
        yield return new WaitForSeconds(1f);

        enemyState = "Idle";
        enemy.SetCharacterState(enemyState);

        playerAttacked = true;
    }

    public IEnumerator DamageCheckAOE(float damage)
    {
        EnemyController currentEnemy;
        float originalDmg = damage;

        for (int i = 0; i < currentEnemyList.Count; i++)
        {
            float targetDmg = originalDmg;
            string state = "normalEnemy";
            currentEnemy = currentEnemyList[i].GetComponentInChildren<EnemyController>();

            if (CheckWeakness(currentEnemy.immune))
            {
                targetDmg = 0;
            }
            else
            {
                if ((currentEnemy.isExposed) && (currentEnemy.exposedTurnCount == 1))
                {
                    targetDmg *= 2;
                    currentEnemy.isExposed = false;
                    currentEnemy.exposed.SetActive(false);
                }

                if (CheckWeakness(currentEnemy.weak) && !CheckNeutral(currentEnemy.weak, currentEnemy.resist))
                {
                    state = "criticalEnemy";
                    if (isDrowned)
                    {
                        targetDmg *= 3;
                    }
                    else
                    {
                        targetDmg *= 2;
                    }
                }
                else if (CheckWeakness(currentEnemy.resist) && !CheckNeutral(currentEnemy.weak, currentEnemy.resist))
                {
                    state = "weakEnemy";
                    targetDmg /= 2;
                }

                if (isMelt)
                {
                    if ((charHealthSlider.value / charMaxHealth) <= 0.25)
                    {
                        targetDmg *= 2;
                    }
                }

                if ((currentEnemy.isScald) && ((rune1 == "Fire") || (rune2 == "Fire")) && (currentEnemy.scaldTurnCount <= 3))
                {
                    targetDmg *= 1.5f;
                }

                if ((currentEnemy.isBurn) && ((rune1 == "Wind") || (rune2 == "Wind")) && ((rune1 != "Water") && (rune2 != "Water")) && (currentEnemy.burnTurnCount <= 2))
                {
                    targetDmg += 2;
                }

                if (isReverb)
                {
                    targetDmg += reverbTurnCount;
                    reverbTurnCount++;
                    reverb.GetComponentInChildren<Text>().text = reverbTurnCount.ToString();
                }
            }

            AudioManager.Instance.Play(currentEnemy.damageSFX);
            enemyState = "Damage";
            currentEnemy.SetCharacterState(enemyState);
            currentEnemyList[i].GetComponentInChildren<ScreenShake>().TriggerShake();
            yield return new WaitForSeconds(0.5f);
            EDamagePopup(currentEnemyList[i].transform, (int)targetDmg, state, false, enemyNumPopupObj);
            EnemyDamage((int)targetDmg, i);
            yield return new WaitForSeconds(1f);

            enemyState = "Idle";
            currentEnemy.SetCharacterState(enemyState);

            if (ChooseSpell() == 1)
            {
                if (ChanceStatusEffect(0.5f) && currentEnemy.enemyType != "LavaGolem")
                {
                    currentEnemy.eDebuffCanvas.SetActive(true);
                    yield return new WaitForSeconds(1f);
                    currentEnemy.eDebuffCanvas.SetActive(false);
                    currentEnemy.isPoisoned = true;
                    currentEnemy.poisonTurnCount = 2;
                    currentEnemy.poisoned.GetComponentInChildren<Text>().text = currentEnemy.poisonTurnCount.ToString();
                    currentEnemy.poisoned.SetActive(true);
                    currentEnemy.eCannotHeal = true;
                    yield return new WaitForSeconds(0.5f);
                }
            }
            else if (ChooseSpell() == 6)
            {
                if (ChanceStatusEffect(0.7f))
                {
                    currentEnemy.eDebuffCanvas.SetActive(true);
                    yield return new WaitForSeconds(1f);
                    currentEnemy.eDebuffCanvas.SetActive(false);
                    currentEnemy.isFreeze = true;
                    currentEnemy.freezeTurnCount = 2;
                    currentEnemy.frozen.GetComponentInChildren<Text>().text = currentEnemy.freezeTurnCount.ToString();
                    currentEnemy.frozen.SetActive(true);
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        isDrowned = false;
        isMelt = false;
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
        PDamagePopup(playerLocation, enemyDmg, "normalPlayer", false, playerNumPopupObj);
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

    public bool CheckNeutral(List<string> weak, List<string> resist)
    {
        if (CheckWeakness(weak) && CheckWeakness(resist))
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
            EDamagePopup(currentEnemyList[index].transform, dmg, "normalEnemy", false, enemyNumPopupObj);
        }
        count--;
        obj.GetComponentInChildren<Text>().text = count.ToString();
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

    public void ResetSpell()
    {
        spellBTNList[ChooseSpell()].GetComponent<Button>().interactable = false;
        spellBTNList[ChooseSpell()].GetComponent<SpellController>().selectedState.SetActive(false);

        for (int i = 0; i < runeObjs.Length; i++)
        {
            runeObjs[i].GetComponent<RuneController>().transform.position = runeObjs[i].GetComponent<RuneController>().defaultPos;
            runeObjs[i].GetComponent<RuneController>().droppedOnSlot = false;
            runeObjs[i].GetComponent<RuneController>().onFirstSlot = false;
            runeObjs[i].GetComponent<RuneController>().onSecondSlot = false;
            runeObjs[i].GetComponent<RuneController>().canvasGroup.interactable = true;
            runeObjs[i].GetComponent<RuneController>().canvasGroup.alpha = 1;
        }

        if (isCharSealed)
        {
            runeObjs[sealedRuneIndex].GetComponent<CanvasGroup>().interactable = false;
            runeObjs[sealedRuneIndex].GetComponent<RuneController>().canvasGroup.alpha = 0.2f;
        }

        rune1 = "";
        rune2 = "";
        isCreatingSpell = false;
    }
    #endregion

    #region Spell Creation
    public void CheckSpell()
    {
        isCreatingSpell = true;
        cancelBTN.SetActive(true);
        for (int i = 0; i < runeObjs.Length; i++)
        {
            runeObjs[i].GetComponent<RuneController>().canvasGroup.interactable = false;
            runeObjs[i].GetComponent<RuneController>().canvasGroup.alpha = 0.2f;
        }
        ChooseSpell();
        SpellChosen();
    }

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
        unlock.SetSpellKey(unlock.keys, ref unlock.unlockList, index, true);
        unlock.CheckSpellBook();
        ChangeSpellDisabled();
        return index;
    }

    public void SpellChosen()
    {
        if (!spellBTNList[ChooseSpell()].GetComponent<SpellController>().onCD)
        {
            if (isAudioPlaying)
            {
                AudioManager.Instance.Play(spellConfirm);
                isAudioPlaying = false;
            }
            spellBTNList[ChooseSpell()].GetComponent<Button>().interactable = true;
            spellBTNList[ChooseSpell()].GetComponent<SpellController>().selectedState.SetActive(true);
        }
        else
        {
            spellOnCDObj.SetActive(true);
        }
    }

    public IEnumerator CreateSpell()
    {
        debuffing = false;

        if (ChooseSpell() == 0)
        {
            extraTurn = true;
        }
        else if (ChooseSpell() == 1)
        {
            debuffing = true;
            sPrefab = Instantiate(spellPrefab[ChooseSpell()], currentEnemyList[targetEnemy].transform);
            sPrefab.GetComponent<SpellCreation>().spell = spellBTNList[ChooseSpell()].GetComponent<SpellController>().spell;
            sPrefab.GetComponent<SpellCreation>().damage = spellBTNList[ChooseSpell()].GetComponent<SpellController>().spell.sDamage;

            yield return new WaitForSeconds(sPrefab.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length + sPrefab.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);
            isAOE = true;
        }
        else if (ChooseSpell() == 2)
        {
             enemy.isExposed = true;
             enemy.exposedTurnCount = 2;
             enemy.exposed.GetComponentInChildren<Text>().text = enemy.exposedTurnCount.ToString();
             enemy.exposed.SetActive(true);
        }
        else if (ChooseSpell() == 3)
        {
            enemy.isScald = true;
            enemy.scaldTurnCount = 4;
            enemy.scalded.GetComponentInChildren<Text>().text = enemy.scaldTurnCount.ToString();
            enemy.scalded.SetActive(true);
        }
        else if (ChooseSpell() == 4)
        {
            isDrowned = true;
            isAOE = true;
        }
        else if (ChooseSpell() == 5)
        {
            isMelt = true;
            isAOE = true;
        }
        else if (ChooseSpell() == 6)
        {
            debuffing = true;
            sPrefab = Instantiate(spellPrefab[ChooseSpell()], currentEnemyList[targetEnemy].transform);
            sPrefab.GetComponent<SpellCreation>().spell = spellBTNList[ChooseSpell()].GetComponent<SpellController>().spell;
            sPrefab.GetComponent<SpellCreation>().damage = spellBTNList[ChooseSpell()].GetComponent<SpellController>().spell.sDamage;

            yield return new WaitForSeconds(sPrefab.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length + sPrefab.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);
            isAOE = true;
        }
        else if (ChooseSpell() == 7)
        {
            if (ChanceStatusEffect(0.7f))
            {
                enemy.isBurn = true;
                enemy.burnTurnCount = 3;
                enemy.burned.GetComponentInChildren<Text>().text = enemy.burnTurnCount.ToString();
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
            float healAmount = charMaxHealth * 0.2f;

            sPrefab = Instantiate(spellPrefab[ChooseSpell()], playerLocation2);
            sPrefab.GetComponent<SpellCreation>().spell = spellBTNList[ChooseSpell()].GetComponent<SpellController>().spell;
            sPrefab.GetComponent<SpellCreation>().damage = spellBTNList[ChooseSpell()].GetComponent<SpellController>().spell.sDamage;

            yield return new WaitForSeconds(sPrefab.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length + sPrefab.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);

            if (!pCannotHeal)
            {
                charHealthSlider.value += healAmount;
                PDamagePopup(playerLocation, (int)healAmount, "normalPlayer", true, playerNumPopupObj);
            }
            else if (pCannotHeal)
            {
                healAmount = charMaxHealth * 0.15f;
                charHealthSlider.value += healAmount;
                PDamagePopup(playerLocation, (int)healAmount, "normalPlayer", true, playerNumPopupObj);
            }
            if (isCharSealed)
            {
                isCharSealed = false;
                seal.SetActive(false);
            }
            if (isCharCursed)
            {
                isCharCursed = false;
                curse.SetActive(false);
            }
            if (isCharPoisoned)
            {
                isCharPoisoned = false;
                playerPoison.SetActive(false);
                pCannotHeal = false;
            }
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
            crystalize.GetComponentInChildren<Text>().text = crystalTurnCount.ToString();
        }

        if (!debuffing)
        {
            if (ChooseSpell() != 9)
            {
                if (ChooseSpell() == 11)
                {
                    sPrefab = Instantiate(spellPrefab[ChooseSpell()], playerLocation2);
                }
                else
                {
                    sPrefab = Instantiate(spellPrefab[ChooseSpell()], currentEnemyList[targetEnemy].transform);
                }
                sPrefab.GetComponent<SpellCreation>().spell = spellBTNList[ChooseSpell()].GetComponent<SpellController>().spell;
                sPrefab.GetComponent<SpellCreation>().damage = spellBTNList[ChooseSpell()].GetComponent<SpellController>().spell.sDamage;
            }

            if (ChooseSpell() != 2 || ChooseSpell() != 9)
            { 
                yield return new WaitForSeconds(sPrefab.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length + sPrefab.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);

                if (ChooseSpell() == 11)
                {
                    buffCanvas.SetActive(true);
                    yield return new WaitForSeconds(0.8f);
                    buffCanvas.SetActive(false);
                }
            }
        }

        if (sPrefab.GetComponent<SpellCreation>().damage != 0)
        {
            yield return PlayerAttack(sPrefab.GetComponent<SpellCreation>().damage);
        }
        else if (enemy.isExposed)
        {
            enemyState = "Damage";
            enemy.SetCharacterState(enemyState);
            yield return new WaitForSeconds(0.5f);
            enemyState = "Idle";
            enemy.SetCharacterState(enemyState);
            playerAttacked = true;
            yield return PAttackPhase();
        }
        else
        {
            playerAttacked = true;
            yield return PAttackPhase();
        }
    }

    public void TriggerAttack()
    {
        StartCoroutine(CreateSpell());
    }
    #endregion

    #region Enemy Status
    public void EnemyDamage(int damage, int target)
    {
        currentEnemyList[target].GetComponentInChildren<EnemyController>().enemyHealthSlider.value -= damage;
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
        float damage = charMaxHealth * 0.08f;
        playerShakeObject.GetComponent<ScreenShake>().TriggerShake();
        charHealthSlider.value -= damage;
        PDamagePopup(playerLocation, (int)damage, "normalPlayer", false, playerNumPopupObj);
        charPoisonedTurnCount--;
        playerPoison.GetComponentInChildren<Text>().text = charPoisonedTurnCount.ToString();
        if (charPoisonedTurnCount <= 0)
        {
            isCharPoisoned = false;
            playerPoison.SetActive(false);
            pCannotHeal = false;
        }
    }

    public void EnemyCurse()
    {
        float damage = charHealthSlider.value * (1f / 12f);
        if (damage <= 0)
        {
            damage = 1;
        }
        playerShakeObject.GetComponent<ScreenShake>().TriggerShake();
        charHealthSlider.value -= damage;
        PDamagePopup(playerLocation, (int)damage, "normalPlayer", false, playerNumPopupObj);
        charCursedTurnCount--;
        curse.GetComponentInChildren<Text>().text = charCursedTurnCount.ToString();
        if (charCursedTurnCount <= 0)
        {
            isCharCursed = false;
            curse.SetActive(false);
        }
    }

    public void EnemySeal()
    {
        runeObjs[sealedRuneIndex].GetComponent<CanvasGroup>().interactable = false;
        runeObjs[sealedRuneIndex].GetComponent<CanvasGroup>().alpha = 0.2f;
        charSealedTurnCount--;
        seal.GetComponentInChildren<Text>().text = charSealedTurnCount.ToString();
        if (charSealedTurnCount <= 0)
        {
            isCharSealed = false;
            seal.SetActive(false);
            runeObjs[sealedRuneIndex].GetComponent<CanvasGroup>().interactable = true;
            runeObjs[sealedRuneIndex].GetComponent<CanvasGroup>().alpha = 1;
        }
    }
    #endregion

    #region Visuals
    public void PDamagePopup(Transform location, int damage, string state, bool isHeal, GameObject popup)
    {
        GameObject damagePopup = Instantiate(popup, location);
        /*if (isHeal)
        {
            damagePopup.transform.localPosition += new Vector3(0, 80, 0);
        }*/
        damagePopup.GetComponent<PlayerNumberPopup>().Setup(damage, state, isHeal);
    }
    public void EDamagePopup(Transform location, int damage, string state, bool isHeal, GameObject popup)
    {
        GameObject damagePopup = Instantiate(popup, location);
        if (isHeal)
        {
            damagePopup.transform.localPosition -= new Vector3(0, 1, 0);
        }
        damagePopup.GetComponent<EnemyNumberPopup>().Setup(damage, state, isHeal);
    }

    public void ChangeSpellDisabled()
    {
        for (int i = 0; i < spellBTNList.Count; i++)
        {
            if (unlock.unlockList[i])
            {
                ColorBlock cb = spellBTNList[i].GetComponent<Button>().colors;
                cb.disabledColor = Color.white;
                spellBTNList[i].GetComponent<Button>().colors = cb;
                foreach (Transform child in spellBTNList[i].transform.parent)
                {
                    if (child.tag == "Lock")
                    {
                        child.gameObject.SetActive(false);
                    }
                }
                //foreach (GameObject child in s)
                //{
                //    if (child.name == "Lock")
                //    {
                //        child.SetActive(false);
                //    }
                //}
            }
        }
    }
    #endregion

    private void Update()
    {
        charHealthText.text = charHealthSlider.value.ToString();
        if (startCheckEnemy)
        {
            enemy = currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>();
        }

        if ((battleState == BattleState.PLAYERTURN) && (!playerAttacked) && !isCreatingSpell)
        {
            if (rune1 != "" && rune2 != "")
            {
                CheckSpell();
            }
        }
    }
}