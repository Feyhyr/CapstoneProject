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

    public RuneSlot slotRune1;
    public RuneSlot slotRune2;
    public string rune1;
    public string rune2;
    public int runeIndex;
    public int sealedRuneIndex;

    public GameObject[] runeObjs;

    public int charMaxHealth;
    public Slider charHealthSlider;
    public Text charHealthText;

    int enemyIndex = 0;
    public List<WaveList> enemyScriptables = new List<WaveList>();
    public List<EnemySO> bossScriptables;
    public List<EnemySO> enemySummonScriptables;
    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    public List<GameObject> currentEnemyList;
    public int targetEnemy = 0;
    public Transform enemyLocation;
    public Transform aoeSpellLocation;

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
    public GameObject charExposed;

    public bool isCrystalize = false;
    int crystalTurnCount = 2;
    public GameObject reverbMissedMessage;
    bool isReverb = false;
    bool reverbMissed = false;
    float reverbBoost = 1.5f;
    int reverbStacks = 0;
    float reverbAccuracy = 1f;
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
    public bool isCreatingSpell = false;
    public bool isSpellCasting = false;
    public bool isCharBound = false;
    public bool isCharExposed = false;
    public int charExposedTurnCount = 2;
    public bool isSteamGuard = false;
    bool statusRemoved = false;
    public bool isCharTaunted = false;
    public GameObject taunt;

    bool bossBattle;

    public GameObject gameWinScreen;
    public GameObject gameLoseScreen;
    public GameObject battleStartUX;
    public Text battleStartUXText;
    public GameObject playerTurnUX;
    public GameObject enemyTurnUX;

    public GameObject creationPrefab;
    public GameObject freezePrefab;
    public GameObject bound;
    public GameObject steamGuard;
    public GameObject spellOnCDObj;

    public GameObject debuffCanvas;
    public GameObject shieldBuffCanvas;
    public GameObject healBuffCanvas;

    EnemyController enemy;
    bool startCheckEnemy;

    private SpellBookMngr unlock;
    public FloorManager floor;
    public Image floorBackground;

    public AudioClip spellConfirm;
    public bool isAudioPlaying;

    public GameObject fadeInCanvas;
    public GameObject endWaveCanvas;

    public GameObject fishCover;
    public Transform lanternLocation;
    public GameObject noEffectText;
    GameObject tempEnemyObject;

    public GameObject tutorialUX;
    public GameObject tutorialSelectUX;

    public GameObject jokerDiceDisplay;

    ToggleSettings toggleSetting;

    public GameObject timelinePanel;
    public GameObject dialoguePanel;
    public List<GameObject> floorCinematics;
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
        toggleSetting = GameObject.Find("ToggleSetting").GetComponent<ToggleSettings>();

        battleState = BattleState.START;

        if (floor.waveCount == 1 && floor.floorCount == 1)
        {
            tutorialUX.SetActive(true);
        }

        if (floor.waveCount == enemyScriptables[floor.floorCount - 1].enemyWaveList.Count + 1)
        {
            bossBattle = true;
        }
        charHealthSlider.value = charMaxHealth;

        ChangeSpellDisabled();

        if (floor.waveCount == 1)
        {
            StartCoroutine(BeginCinematic());
        }
        else
        {
            StartCoroutine(BattleType());
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

    #region Cinematics
    IEnumerator BeginCinematic()
    {
        floorCinematics[floor.floorCount - 1].SetActive(true);
        yield return new WaitForSeconds(16f);
        yield return BattleType();
    }
    #endregion

    IEnumerator BattleType()
    {
        floorCinematics[floor.floorCount - 1].SetActive(false);
        timelinePanel.SetActive(false);
        dialoguePanel.SetActive(false);
        if (bossBattle)
        {
            battleStartUXText.text = "Boss Wave";
            yield return BeginBossBattle();
        }
        else
        {
            battleStartUXText.text = "Wave " + floor.waveCount.ToString() + "/" + (enemyScriptables[floor.floorCount - 1].enemyWaveList.Count + 1).ToString();
            yield return BeginNormalBattle();
        }
    }

    public void SkipCinematic()
    {
        StopAllCoroutines();
        StartCoroutine(BattleType());
    }

    #region Turn Phases
    IEnumerator BeginNormalBattle()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        battleStartUX.SetActive(true);
        yield return new WaitForSeconds(2.3f);
        battleStartUX.SetActive(false);

        GameObject ePrefab;

        enemyIndex = 0;
        //randomEnemyCount = Random.Range(1, 4);

        for (int i = 0; i < enemyScriptables[floor.floorCount - 1].enemyWaveList[floor.waveCount - 1].enemyList.Count; i++)
        {
            //randomEnemy = Random.Range(0, enemyScriptables[floor.floorCount - 1].enemyList.Count);
            ePrefab = Instantiate(enemyPrefab, enemyLocation);

            ePrefab.GetComponentInChildren<EnemyController>().bm = this;
            ePrefab.GetComponentInChildren<EnemyController>().enemy = enemyScriptables[floor.floorCount - 1].enemyWaveList[floor.waveCount - 1].enemyList[i];
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

        if (floor.waveCount == 2 && floor.floorCount == 1)
        {
            tutorialSelectUX.SetActive(true);
        }

        battleState = BattleState.PLAYERTURN;

        yield return PlayerTurn();
    }
    
    IEnumerator BeginBossBattle()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
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

        if (ePrefab.GetComponentInChildren<EnemyController>().enemyType == "AnglerFish")
        {
            yield return ePrefab.GetComponentInChildren<Boss>().SpawnLantern();
        }

        battleState = BattleState.PLAYERTURN;

        yield return PlayerTurn();
    }

    IEnumerator PlayerTurn()
    {
        isAudioPlaying = false;
        isCreatingSpell = false;
        extraTurn = false;

        if (currentEnemyList[0].GetComponentInChildren<EnemyController>().enemyType == "AnglerFish")
        {
            if (currentEnemyList[0].GetComponentInChildren<Boss>().hiddenFishTurnCount >= 2 && currentEnemyList.Count < 2)
            {
                yield return currentEnemyList[0].GetComponentInChildren<Boss>().SpawnLantern();
                currentEnemyList[0].GetComponentInChildren<Boss>().hiddenFishTurnCount = 0;
            }
        }

        if (isCrystalize)
        {
            crystalTurnCount--;
            crystalize.GetComponentInChildren<Text>().text = crystalTurnCount.ToString();
            if (crystalTurnCount < 0)
            {
                isCrystalize = false;
                crystalize.SetActive(false);
            }
        }

        if (isSteamGuard)
        {
            steamGuard.SetActive(false);
        }

        if (isCharExposed)
        {
            charExposedTurnCount--;
            charExposed.GetComponentInChildren<Text>().text = charExposedTurnCount.ToString();
            if (charExposedTurnCount <= 0)
            {
                isCharExposed = false;
                charExposed.SetActive(false);
            }
        }

        for (int i = 0; i < currentEnemyList.Count; i++)
        {
            if ((currentEnemyList[i].GetComponentInChildren<EnemyController>().isExposed) && (currentEnemyList[i].GetComponentInChildren<EnemyController>().exposedTurnCount >= 0))
            {
                PStatusTurnChange(0, i, ref currentEnemyList[i].GetComponentInChildren<EnemyController>().exposedTurnCount, ref currentEnemyList[i].GetComponentInChildren<EnemyController>().isExposed, currentEnemyList[i].GetComponentInChildren<EnemyController>().exposed);
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

        if (isCharBound)
        {
            //GameObject fPrefab = Instantiate(stuckPrefab, playerLocation2);
            //yield return new WaitForSeconds(1);
            //Destroy(fPrefab);

            if (isCharPoisoned)
            {
                yield return new WaitForSeconds(1);
                PlayerPoison();
            }

            CheckPlayerDeath();

            battleState = BattleState.ENEMYTURN;
            yield return EnemyTurn();
        }

        else
        {
            playerAttacked = false;
            playerTurnUX.SetActive(true);
            yield return new WaitForSeconds(1.6f);
            playerTurnUX.SetActive(false);
            isSpellCasting = false;
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

            if (toggleSetting.instantSpell)
            {
                for (int i = 0; i < spellBTNList.Count; i++)
                {
                    if (!spellBTNList[i].GetComponent<SpellController>().onCD && unlock.unlockSpellList[i])
                    {
                        spellBTNList[i].GetComponent<Button>().interactable = true;
                        spellBTNList[i].GetComponent<SpellController>().selectedState.SetActive(true);
                    }
                }
            }
            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
        }
    }

    IEnumerator PAttackPhase()
    {
        if (sPrefab != null)
        {
            sPrefab.GetComponent<SpellCreation>().DestroySpell();
        }

        for (int i = 0; i < runeObjs.Length; i++)
        {
            runeObjs[i].GetComponent<RuneController>().transform.position = runeObjs[i].GetComponent<RuneController>().defaultPos;
            runeObjs[i].GetComponent<RuneController>().droppedOnSlot = false;
            runeObjs[i].GetComponent<RuneController>().onFirstSlot = false;
            runeObjs[i].GetComponent<RuneController>().onSecondSlot = false;
            runeObjs[i].GetComponent<RuneController>().slotted = "";
        }

        slotRune1.currentlyDroppedObj = null;
        slotRune2.currentlyDroppedObj = null;

        if (isReverb)
        {
            isReverb = false;
        }
        else
        {
            reverbBoost = 1.5f;
            reverbAccuracy = 1f;
            reverbStacks = 0;
            reverb.GetComponentInChildren<Text>().text = reverbStacks.ToString();
            reverb.SetActive(false);
        }

        if (isCharSealed)
        {
            PlayerSeal();
        }

        if (isCharTaunted)
        {
            taunt.SetActive(false);
            isCharTaunted = false;
        }

        rune1 = "";
        rune2 = "";

        if (isCharCursed)
        {
            yield return new WaitForSeconds(1);
            PlayerCurse();
        }

        if (isCharPoisoned)
        {
            yield return new WaitForSeconds(1);
            PlayerPoison();
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
        if (isCharBound)
        {
            isCharBound = false;
            bound.SetActive(false);
            //charStuck.SetActive(false);
        }

        enemy.targetSelected.SetActive(false);
        enemyTurnUX.SetActive(true);
        yield return new WaitForSeconds(1.6f);
        enemyTurnUX.SetActive(false);
        yield return new WaitForSeconds(1);
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

        for (int i = currentEnemyList.Count - 1; i >= 0; i--)
        {
            CheckPlayerDeath();
            if (currentEnemyList[i].GetComponentInChildren<EnemyController>().tag != "Lantern")
            { 
                yield return currentEnemyList[i].GetComponentInChildren<EnemyController>().AttackPattern();
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
            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
        }
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
            gameWinScreen.SetActive(true);
            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
            //SceneManager.LoadScene("GameWinScene");
        }
        else if (battleState == BattleState.LOSE)
        {
            PlayerPrefs.SetInt(floor.prefWave, 1);
            gameLoseScreen.SetActive(true);
            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
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
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        floor.AddCount(ref floor.waveCount, floor.prefWave);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("BattleScene");
    }
    #endregion

    #region Attack Functions
    public IEnumerator PlayerAttack(float damage)
    {
        enemy.targetSelected.SetActive(false);
        if (isAOE && !isCharTaunted)
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

        if (enemy.isJuggernautShieldOn && (rune1 != "Earth") && (rune2 != "Earth"))
        {
            damage = 0;
        }
        else if (CheckWeakness(enemy.immune) && !enemy.isJuggernautShieldOn)
        {
            damage = 0;
        }
        else if (reverbMissed)
        {
            damage = 0;
            reverbMissedMessage.SetActive(true);
            reverbMissed = false;
        }
        else
        {
            if (enemy.isExposed)
            {
                damage *= 2;
                //enemy.isExposed = false;
                //enemy.exposed.SetActive(false);
            }

            if (CheckWeakness(enemy.weak) && !CheckNeutral(enemy.weak, enemy.resist) && !enemy.isJuggernautShieldOn)
            {
                state = "criticalEnemy";
                damage *= 2;
            }
            else if (CheckWeakness(enemy.resist) && !CheckNeutral(enemy.weak, enemy.resist) && !enemy.isJuggernautShieldOn)
            {
                state = "weakEnemy";
                damage /= 2;
            }

            if (isMelt)
            {
                if ((charHealthSlider.value / charMaxHealth) <= 0.5)
                {
                    damage *= 2;
                }
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

            //if ((enemy.isBurn) && ((rune1 == "Wind") || (rune2 == "Wind")) && ((rune1 != "Water") && (rune2 != "Water")) && (enemy.burnTurnCount <= 2))
            //{
            //    damage += 2;
            //}

            if (isReverb && reverbStacks > 1)
            {
                damage *= reverbBoost;
                reverbBoost *= 1.5f;
                reverbAccuracy -= 0.1f;
                reverb.GetComponentInChildren<Text>().text = reverbStacks.ToString();
            }

            if (enemy.isSorrow)
            {
                damage *= 4;
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
        reverbMissedMessage.SetActive(false);
        enemyState = "Idle";
        enemy.SetCharacterState(enemyState);

        if (enemy.enemyHealthSlider.value > 0 && enemy.enemyType != "Lantern")
        {
            float accuracy = 1f;

            if (ChooseSpell() == 1)
            {
                if (ChanceStatusEffect(accuracy) && enemy.enemyType != "LavaGolem" && !enemy.isJuggernautShieldOn && enemy.enemyType != "HellSpire")
                {
                    enemy.eDebuffCanvas.SetActive(true);
                    yield return new WaitForSeconds(1f);
                    enemy.eDebuffCanvas.SetActive(false);
                    enemy.isPoisoned = true;
                    enemy.poisonTurnCount = 2;
                    enemy.poisoned.GetComponentInChildren<Text>().text = enemy.poisonTurnCount.ToString();
                    enemy.poisoned.SetActive(true);
                    enemy.eCannotHeal = true;
                    yield return new WaitForSeconds(0.5f);
                }
            }
            else if (ChooseSpell() == 6)
            {
                if (ChanceStatusEffect(accuracy) && !enemy.isJuggernautShieldOn)
                {
                    enemy.eDebuffCanvas.SetActive(true);
                    yield return new WaitForSeconds(1f);
                    enemy.eDebuffCanvas.SetActive(false);
                    enemy.isFreeze = true;
                    enemy.freezeTurnCount = 2;
                    enemy.frozen.GetComponentInChildren<Text>().text = enemy.freezeTurnCount.ToString();
                    enemy.frozen.SetActive(true);
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        //if (currentEnemyList[targetEnemy].GetComponentInChildren<EnemyController>().enemyHealthSlider.value <= 0)
        //{
        //    currentEnemyList[targetEnemy].SetActive(false);
        //}
        yield return CheckEnemyDeath(targetEnemy);
        playerAttacked = true;
    }

    public IEnumerator DamageCheckAOE(float damage)
    {
        EnemyController currentEnemy;
        float originalDmg = damage;

        int selectedTarget = targetEnemy;

        for (int i = currentEnemyList.Count - 1; i >= 0; i--)
        {
            float targetDmg = originalDmg;
            string state = "normalEnemy";
            currentEnemy = currentEnemyList[i].GetComponentInChildren<EnemyController>();

            if (currentEnemy.isJuggernautShieldOn && ((rune1 != "Earth") && (rune2 != "Earth")))
            {
                targetDmg = 0;
            }
            else if (CheckWeakness(currentEnemy.immune) && !currentEnemy.isJuggernautShieldOn)
            {
                targetDmg = 0;
            }
            else
            {
                if (currentEnemy.isExposed)
                {
                    targetDmg *= 2;
                    //currentEnemy.isExposed = false;
                    //currentEnemy.exposed.SetActive(false);
                }

                if (CheckWeakness(currentEnemy.weak) && !CheckNeutral(currentEnemy.weak, currentEnemy.resist) && !currentEnemy.isJuggernautShieldOn)
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
                else if (CheckWeakness(currentEnemy.resist) && !CheckNeutral(currentEnemy.weak, currentEnemy.resist) && !currentEnemy.isJuggernautShieldOn)
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

                if (currentEnemy.isSorrow)
                {
                    targetDmg *= 4;
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

            if (currentEnemy.enemyHealthSlider.value > 0 && currentEnemy.enemyType != "Lantern")
            {
                float accuracy = 0.8f;
                if (currentEnemyList.Count > 1)
                {
                    if (i == selectedTarget)
                    {
                        accuracy = 1f;
                    }
                }

                if (ChooseSpell() == 1)
                {
                    if (ChanceStatusEffect(accuracy) && currentEnemy.enemyType != "LavaGolem" && !currentEnemy.isJuggernautShieldOn && currentEnemy.enemyType != "HellSpire")
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
                    if (ChanceStatusEffect(accuracy) && !currentEnemy.isJuggernautShieldOn)
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
            yield return CheckEnemyDeath(i);
        }
        isDrowned = false;
        isMelt = false;
        playerAttacked = true;
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
        if (Random.value <= chance)
        {
            return true;
        }
        return false;
    }

    public void PStatusTurnChange(int dmg, int index, ref int count, ref bool status, GameObject obj)
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

    public IEnumerator CheckEnemyDeath(int index)
    {
        if (currentEnemyList[index].GetComponentInChildren<EnemyController>().enemyHealthSlider.value <= 0 && currentEnemyList[index].GetComponentInChildren<EnemyController>().isJuggernautShieldOn)
        {
            currentEnemyList[index].GetComponentInChildren<EnemyController>().eDebuffCanvas.SetActive(true);
            yield return new WaitForSeconds(1);
            currentEnemyList[index].GetComponentInChildren<EnemyController>().eDebuffCanvas.SetActive(false);
            currentEnemyList[index].GetComponentInChildren<EnemyController>().juggernautShield.SetActive(false);
            currentEnemyList[index].GetComponentInChildren<EnemyController>().enemyHealthSlider.maxValue = currentEnemyList[index].GetComponentInChildren<EnemyController>().enemy.maxHealth;
            currentEnemyList[index].GetComponentInChildren<EnemyController>().enemyHealthSlider.value = currentEnemyList[index].GetComponentInChildren<EnemyController>().enemyHealthSlider.maxValue;
            currentEnemyList[index].GetComponentInChildren<EnemyController>().enemyHealthColor.color = new Color32(44, 108, 44, 255);
            currentEnemyList[index].GetComponentInChildren<EnemyController>().isJuggernautShieldOn = false;
            currentEnemyList[index].GetComponentInChildren<EnemyController>().shield.SetActive(false);
        }

        else if (currentEnemyList[index].GetComponentInChildren<EnemyController>().enemyHealthSlider.value <= 0 && !currentEnemyList[index].GetComponentInChildren<EnemyController>().isJuggernautShieldOn)
        {
            tempEnemyObject = currentEnemyList[index];
            currentEnemyList[index].SetActive(false);

            if (currentEnemyList[index].tag == "Kindred")
            {
                for (int i = 0; i < currentEnemyList.Count; i++)
                {
                    if (currentEnemyList[i].GetComponentInChildren<EnemyController>().isKindred)
                    {
                        currentEnemyList[i].GetComponentInChildren<EnemyController>().isKindred = false;
                        currentEnemyList[i].GetComponentInChildren<EnemyController>().kindred.SetActive(false);
                        currentEnemyList[i].GetComponentInChildren<EnemyController>().eDebuffCanvas.SetActive(true);
                        yield return new WaitForSeconds(1f);
                        currentEnemyList[i].GetComponentInChildren<EnemyController>().eDebuffCanvas.SetActive(false);
                        currentEnemyList[i].GetComponentInChildren<EnemyController>().isSorrow = true;
                        currentEnemyList[i].GetComponentInChildren<EnemyController>().sorrow.SetActive(true);
                    }
                }
            }

            if (currentEnemyList[index].GetComponentInChildren<EnemyController>().enemyType == "AnglerFish")
            {
                battleState = BattleState.WIN;
                startCheckEnemy = false;
                EndBossBattle();
                yield break;
            }
            if (currentEnemyList[index].tag == "Lantern")
            {
                yield return FadeOutFishCover(fishCover.GetComponent<Image>());
            }
            currentEnemyList.RemoveAt(index);
            Destroy(tempEnemyObject);

            if (IsAllEnemiesDead())
            {
                battleState = BattleState.WIN;
                startCheckEnemy = false;
                if (bossBattle)
                {
                    EndBossBattle();
                    yield break;
                }
                else
                {
                    EndNormalBattle();
                    yield break;
                }
            }
            else
            {
                targetEnemy = 0;
                enemyIndex = 0;

                for (int i = 0; i < currentEnemyList.Count; i++)
                {
                    currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyId = enemyIndex;
                    enemyIndex++;
                }
            }
        }
    }

    /*public void ResetSpell()
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
    }*/
    #endregion

    #region Spell Creation
    public void CheckSpell()
    {
        isCreatingSpell = true;
        //cancelBTN.SetActive(true);
        //for (int i = 0; i < runeObjs.Length; i++)
        //{
        //    runeObjs[i].GetComponent<RuneController>().canvasGroup.interactable = false;
        //    runeObjs[i].GetComponent<RuneController>().canvasGroup.alpha = 0.2f;
        //}
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

        //if (toggleSetting.instantSpell)
        //{
        //    return sPrefab.GetComponent<SpellCreation>().spell.spellIndex;
        //}
        //else
        //{
            return index;
        //}
    }

    public void SpellChosen()
    {
        if (rune1 != "" && rune2 != "")
        {
            unlock.SetPrefKey(unlock.spellKeys, ref unlock.unlockSpellList, ChooseSpell(), true);
            unlock.CheckSpellBook();
            ChangeSpellDisabled();
        }

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

    public IEnumerator CreateSpell(int index)
    {
        debuffing = false;

        if (index == 0)
        {
            extraTurn = true;
        }
        else if (index == 1)
        {
            debuffing = true;
            sPrefab = Instantiate(spellPrefab[index], aoeSpellLocation);
            sPrefab.GetComponent<SpellCreation>().spell = spellBTNList[index].GetComponent<SpellController>().spell;
            sPrefab.GetComponent<SpellCreation>().damage = spellBTNList[index].GetComponent<SpellController>().spell.sDamage;

            yield return new WaitForSeconds(sPrefab.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length + sPrefab.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);
            if (enemy.tag != "Lantern")
            {
                isAOE = true;
            }
        }
        else if (index == 2 && enemy.tag != "Lantern")
        {
            enemy.isExposed = true;
            enemy.exposedTurnCount = 3;
            enemy.exposed.GetComponentInChildren<Text>().text = "2";
        }
        else if (index == 3 && enemy.tag != "Lantern" && !enemy.isJuggernautShieldOn)
        {
            if (!enemy.immune.Contains("Fire"))
            {
                enemy.isScald = true;
                enemy.scaldTurnCount = 4;
                enemy.scalded.GetComponentInChildren<Text>().text = enemy.scaldTurnCount.ToString();
            }
        }
        else if (index == 4 && enemy.tag != "Lantern")
        {
            isDrowned = true;
            isAOE = true;
        }
        else if (index == 5 && enemy.tag != "Lantern")
        {
            isMelt = true;
            isAOE = true;
        }
        else if (index == 6)
        {
            debuffing = true;
            sPrefab = Instantiate(spellPrefab[index], aoeSpellLocation);
            sPrefab.GetComponent<SpellCreation>().spell = spellBTNList[index].GetComponent<SpellController>().spell;
            sPrefab.GetComponent<SpellCreation>().damage = spellBTNList[index].GetComponent<SpellController>().spell.sDamage;

            yield return new WaitForSeconds(sPrefab.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length + sPrefab.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);
            if (enemy.tag != "Lantern")
            {
                isAOE = true;
            }
        }
        else if (index == 7 && enemy.tag != "Lantern" && !enemy.isJuggernautShieldOn)
        {
            if (!enemy.immune.Contains("Fire"))
            {
                enemy.isBurn = true;
                enemy.burnTurnCount = 3;
                enemy.burned.GetComponentInChildren<Text>().text = enemy.burnTurnCount.ToString();
            }
        }
        else if (index == 8)
        {
            if (ChanceStatusEffect(reverbAccuracy))
            {
                isReverb = true;
                reverbStacks++;
                reverb.GetComponentInChildren<Text>().text = reverbStacks.ToString();
                reverb.SetActive(true);
            }
            else
            {
                reverbMissed = true;
            }
        }
        else if (index == 9)
        {
            float healAmount = charMaxHealth * 0.2f;

            sPrefab = Instantiate(spellPrefab[index], playerLocation2);
            sPrefab.GetComponent<SpellCreation>().spell = spellBTNList[index].GetComponent<SpellController>().spell;
            sPrefab.GetComponent<SpellCreation>().damage = spellBTNList[index].GetComponent<SpellController>().spell.sDamage;

            yield return new WaitForSeconds(sPrefab.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length + sPrefab.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);

            if (isCharSealed || isCharCursed || isCharPoisoned || isCharExposed)
            {
                healAmount = charMaxHealth * 0.1f;
                statusRemoved = true;
            }

            healBuffCanvas.SetActive(true);
            yield return new WaitForSeconds(0.8f);
            healBuffCanvas.SetActive(false);
            charHealthSlider.value += healAmount;
            PDamagePopup(playerLocation, (int)healAmount, "normalPlayer", true, playerNumPopupObj);
            isCharSealed = false;
            seal.SetActive(false);
            isCharCursed = false;
            curse.SetActive(false);
            isCharPoisoned = false;
            playerPoison.SetActive(false);
            isCharExposed = false;
            charExposed.SetActive(false);
            if (statusRemoved)
            {
                isSteamGuard = true;
                steamGuard.SetActive(true);
            }

        }
        else if (index == 10)
        {
            debrisHit = true;
        }
        else if (index == 11)
        {
            isCrystalize = true;
            crystalize.SetActive(true);
            crystalTurnCount = 2;
            crystalize.GetComponentInChildren<Text>().text = crystalTurnCount.ToString();
        }

        if (!debuffing)
        {
            if (index != 9)
            {
                if (index == 11)
                {
                    sPrefab = Instantiate(spellPrefab[index], playerLocation2);
                }
                else if ((index == 4 || index == 5))
                {
                    sPrefab = Instantiate(spellPrefab[index], aoeSpellLocation);
                }
                else
                {
                    sPrefab = Instantiate(spellPrefab[index], currentEnemyList[targetEnemy].transform);
                }
                sPrefab.GetComponent<SpellCreation>().spell = spellBTNList[index].GetComponent<SpellController>().spell;
                sPrefab.GetComponent<SpellCreation>().damage = spellBTNList[index].GetComponent<SpellController>().spell.sDamage;
            }

            if (index != 2 || index != 9)
            { 
                yield return new WaitForSeconds(sPrefab.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length + sPrefab.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);

                if ((index == 3 || index == 7) && enemy.tag != "Lantern" && !enemy.isJuggernautShieldOn)
                {
                    if (enemy.isScald)
                    {
                        enemy.eDebuffCanvas.SetActive(true);
                        yield return new WaitForSeconds(1f);
                        enemy.eDebuffCanvas.SetActive(false);
                        enemy.scalded.SetActive(true);
                    }
                    if (enemy.isBurn)
                    {
                        enemy.eDebuffCanvas.SetActive(true);
                        yield return new WaitForSeconds(1f);
                        enemy.eDebuffCanvas.SetActive(false);
                        enemy.burned.SetActive(true);
                    }
                }

                if (index == 11)
                {
                    shieldBuffCanvas.SetActive(true);
                    yield return new WaitForSeconds(0.8f);
                    shieldBuffCanvas.SetActive(false);
                }
            }
        }

        if (sPrefab.GetComponent<SpellCreation>().damage != 0)
        {
            yield return PlayerAttack(sPrefab.GetComponent<SpellCreation>().damage);
        }
        else if (enemy.isExposed || enemy.tag == "Lantern")
        {
            if (enemy.tag != "Lantern" && index == 2)
            {
                enemy.eDebuffCanvas.SetActive(true);
                yield return new WaitForSeconds(1f);
                enemy.eDebuffCanvas.SetActive(false);
                enemy.exposed.SetActive(true);
            }
            enemyState = "Damage";
            enemy.SetCharacterState(enemyState);
            yield return new WaitForSeconds(0.5f);
            enemyState = "Idle";
            enemy.SetCharacterState(enemyState);
            if (enemy.tag == "Lantern")
            {
                noEffectText.SetActive(true);
                yield return new WaitForSeconds(1.5f);
                noEffectText.SetActive(false);
            }
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
        StartCoroutine(CreateSpell(ChooseSpell()));
    }
    #endregion

    #region Statuses
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

    public void PlayerPoison()
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
        }
    }

    public void PlayerCurse()
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

    public void PlayerSeal()
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
            if (unlock.unlockSpellList[i])
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
            }
        }
    }

    public IEnumerator FadeOutFishCover(Image image)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        while (image.color.a > 0.0f)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - Time.deltaTime);
            yield return null;
        }

    }

    public IEnumerator FadeInFishCover(Image image)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        while (image.color.a < 1.0f)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + Time.deltaTime);
            yield return null;
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

        if (fishCover.GetComponent<Image>().color.a <= 0)
        {
            fishCover.SetActive(false);
        }
        else if (fishCover.GetComponent<Image>().color.a > 0)
        {
            fishCover.SetActive(true);
        }

        if ((battleState == BattleState.PLAYERTURN) && (!playerAttacked) && !isCreatingSpell && !isSpellCasting)
        {
            if (rune1 != "" && rune2 != "")
            {
                CheckSpell();
            }
        }
    }
}