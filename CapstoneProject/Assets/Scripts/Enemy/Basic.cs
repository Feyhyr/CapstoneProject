using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Basic : EnemyController
{
    public List<Sprite> TypeList;

    int glassCannonTurnCount = 0;
    public GameObject glassCannon;

    int tauntCD = 0;
    int buffCD = 0;

    public List<Texture> diceImages;
    int diceIndex = 0;
    string jokerTarget;

    protected override void Start()
    {
        base.Start();

        if (enemyType == "Attacker")
        {
            TypeIcon.sprite = TypeList[0];
        }
        else if (enemyType == "Healer")
        {
            TypeIcon.sprite = TypeList[1];
        }
        else if (enemyType == "Summoner")
        {
            TypeIcon.sprite = TypeList[2];
        }
        else if (enemyType == "Debuffer")
        {
            TypeIcon.sprite = TypeList[3];
        }
        else if (enemyType == "Buffer")
        {
            TypeIcon.sprite = TypeList[4];
        }
        else if (enemyType == "GlassCannon")
        {
            TypeIcon.sprite = TypeList[5];
            glassCannon.SetActive(true);
        }
        else if (enemyType == "Joker")
        {
            TypeIcon.sprite = TypeList[6];
        }
        else if (enemyType == "Juggernaut")
        {
            TypeIcon.sprite = TypeList[7];

            isJuggernautShieldOn = true;
            juggernautShield.SetActive(true);
        }
        else if (enemyType == "Kindred")
        {
            TypeIcon.sprite = TypeList[8];
        }
        else if (enemyType == "Tank")
        {
            TypeIcon.sprite = TypeList[9];
        }
        TypeText.text = enemyType;
    }

    public override IEnumerator AttackPattern()
    {
        if (enemyType == "Attacker" || enemyType == "Kindred" || enemyType == "Juggernaut")
        {
            yield return Attack();
        }
        else if (enemyType == "Healer")
        {
            yield return Healer();
        }
        else if (enemyType == "Summoner")
        {
            yield return Summoner();
        }
        else if (enemyType == "Debuffer")
        {
            yield return Debuffer();
        }
        else if (enemyType == "GlassCannon")
        {
            yield return GlassCannon();
        }
        else if (enemyType == "Tank")
        {
            yield return Tank();
        }
        else if (enemyType == "Buffer")
        {
            yield return Buffer();
        }
        else if (enemyType == "Joker")
        {
            yield return Joker();
        }
    }

    protected override void Update()
    {
        base.Update();

        if (tag == "Juggernaut")
        {
            if (isJuggernautShieldOn)
            {
                eHPText.text = enemyHealthSlider.value.ToString() + "/" + enemy.juggernautShieldHealth;
            }
            else if (!isJuggernautShieldOn)
            {
                eHPText.text = enemyHealthSlider.value.ToString() + "/" + enemy.maxHealth;
            }
        }
    }

    private IEnumerator Attack()
    {
        if (isFreeze && freezeTurnCount > 0)
        {
            GameObject fPrefab = Instantiate(freezePrefab, transform.parent);
            yield return new WaitForSeconds(1);
            Destroy(fPrefab);
            freezeTurnCount--;
            frozen.GetComponentInChildren<Text>().text = freezeTurnCount.ToString();
            if (freezeTurnCount == 0)
            {
                isFreeze = false;
                frozen.SetActive(false);
            }
        }

        else
        {
            enemyAttacking = true;
            bm.playerShakeObject.GetComponent<ScreenShake>().TriggerShake();
            yield return EnemyAttack();
        }

        yield return CheckEnemyStatus();
    }

    private IEnumerator Healer()
    {
        bool isHealing = false;
        float healAmount1 = 0;
        float healAmount2 = 0;

        if (isFreeze && freezeTurnCount > 0)
        {
            GameObject fPrefab = Instantiate(freezePrefab, transform.parent);
            yield return new WaitForSeconds(1);
            Destroy(fPrefab);
            freezeTurnCount--;
            frozen.GetComponentInChildren<Text>().text = freezeTurnCount.ToString();
            if (freezeTurnCount == 0)
            {
                isFreeze = false;
                frozen.SetActive(false);
            }
        }

        else
        {
            enemyAttacking = true;
            currentState = "Attack";
            SetCharacterState(currentState);
            int targetsToHeal = enemy.healingTargets;
            int healTarget1 = 0;
            int healTarget2 = 0;
            float lowestHealth = 0;
            float secondLowHealth = 0;

            while (targetsToHeal != 0)
            {
                for (int i = 0; i < bm.currentEnemyList.Count; i++)
                {
                    if (bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyHealthSlider.value < bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemy.maxHealth && !bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().isJuggernautShieldOn)
                    {
                        if (!bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().eCannotHeal)
                        {
                            if (lowestHealth == 0)
                            {
                                lowestHealth = bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyHealthSlider.value;
                                healAmount1 = bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemy.maxHealth * enemy.healingPercent;
                                healTarget1 = i;
                            }
                            else if (secondLowHealth == 0 && enemy.healingTargets > 1 && i != healTarget1)
                            {
                                secondLowHealth = bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyHealthSlider.value;
                                healAmount2 = bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemy.maxHealth * enemy.healingPercent;
                                healTarget2 = i;
                            }
                            else if (lowestHealth > bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyHealthSlider.value)
                            {
                                if (enemy.healingTargets > 1)
                                {
                                    secondLowHealth = lowestHealth;
                                    healAmount2 = bm.currentEnemyList[healTarget1].GetComponentInChildren<EnemyController>().enemy.maxHealth * enemy.healingPercent;
                                    healTarget2 = healTarget1;
                                }
                                lowestHealth = bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyHealthSlider.value;
                                healAmount1 = bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemy.maxHealth * enemy.healingPercent;
                                healTarget1 = i;
                            }
                            else if (secondLowHealth > bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyHealthSlider.value && enemy.healingTargets > 1 && i != healTarget1)
                            {
                                secondLowHealth = bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyHealthSlider.value;
                                healAmount2 = bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemy.maxHealth * enemy.healingPercent;
                                healTarget2 = i;
                            }
                            isHealing = true;
                        }
                    }
                }
                targetsToHeal--;
            }

            if (isHealing)
            {
                bm.currentEnemyList[healTarget1].GetComponentInChildren<EnemyController>().eBuffCanvas.SetActive(true);
                yield return new WaitForSeconds(1f);
                bm.currentEnemyList[healTarget1].GetComponentInChildren<EnemyController>().eBuffCanvas.SetActive(false);
                bm.currentEnemyList[healTarget1].GetComponentInChildren<EnemyController>().enemyHealthSlider.value += healAmount1;
                bm.EDamagePopup(bm.currentEnemyList[healTarget1].transform, (int)healAmount1, "normalEnemy", true, bm.enemyNumPopupObj);

                if (enemy.healingTargets > 1 && secondLowHealth != 0)
                {
                    bm.currentEnemyList[healTarget2].GetComponentInChildren<EnemyController>().eBuffCanvas.SetActive(true);
                    yield return new WaitForSeconds(1f);
                    bm.currentEnemyList[healTarget2].GetComponentInChildren<EnemyController>().eBuffCanvas.SetActive(false);
                    bm.currentEnemyList[healTarget2].GetComponentInChildren<EnemyController>().enemyHealthSlider.value += healAmount2;
                    bm.EDamagePopup(bm.currentEnemyList[healTarget2].transform, (int)healAmount2, "normalEnemy", true, bm.enemyNumPopupObj);
                }
            }
            else if (!isHealing)
            {
                bm.playerShakeObject.GetComponent<ScreenShake>().TriggerShake();
                yield return EnemyAttack();
            }
        }

        yield return CheckEnemyStatus();
    }

    private IEnumerator Summoner()
    {
        bool isSummoning = false;

        if (isFreeze && freezeTurnCount > 0)
        {
            GameObject fPrefab = Instantiate(freezePrefab, transform.parent);
            yield return new WaitForSeconds(1);
            Destroy(fPrefab);
            freezeTurnCount--;
            frozen.GetComponentInChildren<Text>().text = freezeTurnCount.ToString();
            if (freezeTurnCount == 0)
            {
                isFreeze = false;
                frozen.SetActive(false);
            }
        }

        else
        {
            enemyAttacking = true;

            if (bm.currentEnemyList.Count < 3)
            {
                isSummoning = true;
                GameObject ePrefab;
                int enemyIndex = bm.currentEnemyList.Count;
                int index = bm.floor.floorCount - 1;
                ePrefab = Instantiate(bm.enemyPrefab, bm.enemyLocation);

                ePrefab.GetComponentInChildren<EnemyController>().bm = bm;
                ePrefab.GetComponentInChildren<EnemyController>().enemy = bm.enemySummonScriptables[index];
                ePrefab.GetComponentInChildren<EnemyController>().enemyId = enemyIndex;
                ePrefab.tag = ePrefab.GetComponentInChildren<EnemyController>().enemy.tagName;
                foreach (Transform t in ePrefab.transform)
                {
                    t.gameObject.tag = ePrefab.tag;
                }
                ePrefab.GetComponentInChildren<EnemyController>().eText.text = ePrefab.GetComponentInChildren<EnemyController>().enemy.enemyName;
                bm.currentEnemyList.Add(ePrefab);
                yield return new WaitForSeconds(0.5f);
            }

            if (!isSummoning)
            {
                bm.playerShakeObject.GetComponent<ScreenShake>().TriggerShake();
                yield return EnemyAttack();
            }
        }

        yield return CheckEnemyStatus();
    }

    private IEnumerator Debuffer()
    {
        if (isFreeze && freezeTurnCount > 0)
        {
            GameObject fPrefab = Instantiate(freezePrefab, transform.parent);
            yield return new WaitForSeconds(1);
            Destroy(fPrefab);
            freezeTurnCount--;
            frozen.GetComponentInChildren<Text>().text = freezeTurnCount.ToString();
            if (freezeTurnCount == 0)
            {
                isFreeze = false;
                frozen.SetActive(false);
            }
        }

        else
        {
            enemyAttacking = true;
            bm.playerShakeObject.GetComponent<ScreenShake>().TriggerShake();
            yield return EnemyAttack();

            if (!bm.isSteamGuard)
            {
                if (tag == "Hellhound")
                {
                    if (bm.ChanceStatusEffect(0.5f))
                    {
                        yield return new WaitForSeconds(0.5f);
                        bm.debuffCanvas.SetActive(true);
                        yield return new WaitForSeconds(1f);
                        bm.debuffCanvas.SetActive(false);
                        bm.isCharExposed = true;
                        bm.charExposedTurnCount = 3;
                        bm.charExposed.GetComponentInChildren<Text>().text = "2"/*bm.charExposedTurnCount.ToString()*/;
                        bm.charExposed.SetActive(true);
                    }
                }
                else if (tag == "PoisonOak" || tag == "Jellyfish")
                {
                    float poisonChance = 0;
                    if (tag == "PoisonOak")
                    {
                        poisonChance = 0.3f;
                    }
                    else if (tag == "Jellyfish")
                    {
                        poisonChance = 0.5f;
                    }
                    if (bm.ChanceStatusEffect(poisonChance))
                    {
                        yield return new WaitForSeconds(0.5f);
                        bm.debuffCanvas.SetActive(true);
                        yield return new WaitForSeconds(1f);
                        bm.debuffCanvas.SetActive(false);
                        bm.isCharPoisoned = true;
                        bm.charPoisonedTurnCount = 2;
                        bm.playerPoison.GetComponentInChildren<Text>().text = bm.charPoisonedTurnCount.ToString();
                        bm.playerPoison.SetActive(true);
                    }
                }
                else if (tag == "Possessed" || tag == "BlackSpiral")
                {
                    float curseChance = 0;
                    float sealChance = 0;
                    if (tag == "Possessed")
                    {
                        curseChance = 0.4f;
                        sealChance = 0.3f;
                    }
                    else if (tag == "BlackSpiral")
                    {
                        curseChance = 0.5f;
                        sealChance = 0.3f;
                    }
                    if (bm.ChanceStatusEffect(curseChance))
                    {
                        yield return new WaitForSeconds(0.5f);
                        bm.debuffCanvas.SetActive(true);
                        yield return new WaitForSeconds(1f);
                        bm.debuffCanvas.SetActive(false);
                        bm.isCharCursed = true;
                        bm.charCursedTurnCount = 2;
                        bm.curse.GetComponentInChildren<Text>().text = bm.charCursedTurnCount.ToString();
                        bm.curse.SetActive(true);
                    }

                    if (!bm.isCharSealed)
                    {
                        if (bm.ChanceStatusEffect(sealChance))
                        {
                            yield return new WaitForSeconds(0.5f);
                            bm.debuffCanvas.SetActive(true);
                            yield return new WaitForSeconds(1f);
                            bm.debuffCanvas.SetActive(false);
                            bm.sealedRuneIndex = Random.Range(0, 4);
                            bm.runeObjs[bm.sealedRuneIndex].GetComponent<CanvasGroup>().interactable = false;
                            bm.runeObjs[bm.sealedRuneIndex].GetComponent<CanvasGroup>().alpha = 0.2f;
                            bm.charSealedTurnCount = 2;
                            bm.seal.GetComponentInChildren<Text>().text = bm.charSealedTurnCount.ToString();
                            bm.seal.SetActive(true);
                            bm.isCharSealed = true;
                        }
                    }
                }
            }
        }

        yield return CheckEnemyStatus();
    }

    private IEnumerator GlassCannon()
    {
        if (isFreeze && freezeTurnCount > 0)
        {
            GameObject fPrefab = Instantiate(freezePrefab, transform.parent);
            yield return new WaitForSeconds(1);
            Destroy(fPrefab);
            freezeTurnCount--;
            frozen.GetComponentInChildren<Text>().text = freezeTurnCount.ToString();
            if (freezeTurnCount == 0)
            {
                isFreeze = false;
                frozen.SetActive(false);
            }
        }

        else
        {
            enemyAttacking = true;

            if (glassCannonTurnCount <= 4)
            {
                eBuffCanvas.SetActive(true);
                yield return new WaitForSeconds(1);
                eBuffCanvas.SetActive(false);
                glassCannonTurnCount++;
                glassCannon.GetComponentInChildren<Text>().text = glassCannonTurnCount.ToString();

                if (glassCannonTurnCount >= 4)
                {
                    bm.playerShakeObject.GetComponent<ScreenShake>().TriggerShake();
                    yield return EnemyAttack();
                    glassCannonTurnCount = 0;
                    glassCannon.GetComponentInChildren<Text>().text = glassCannonTurnCount.ToString();
                }
            }
        }

        yield return CheckEnemyStatus();
    }

    private IEnumerator Tank()
    {
        if (isFreeze && freezeTurnCount > 0)
        {
            GameObject fPrefab = Instantiate(freezePrefab, transform.parent);
            yield return new WaitForSeconds(1);
            Destroy(fPrefab);
            freezeTurnCount--;
            frozen.GetComponentInChildren<Text>().text = freezeTurnCount.ToString();
            if (freezeTurnCount == 0)
            {
                isFreeze = false;
                frozen.SetActive(false);
            }
        }

        else
        {
            enemyAttacking = true;
            bm.playerShakeObject.GetComponent<ScreenShake>().TriggerShake();
            yield return EnemyAttack();

            tauntCD--;

            if (tauntCD <= 0)
            {
                yield return new WaitForSeconds(0.5f);
                bm.debuffCanvas.SetActive(true);
                yield return new WaitForSeconds(1f);
                bm.debuffCanvas.SetActive(false);
                bm.isCharTaunted = true;
                bm.taunt.SetActive(true);
                bm.targetEnemy = enemyId;
                tauntCD = 2;
            }
        }

        yield return CheckEnemyStatus();
    }

    private IEnumerator Buffer()
    {
        if (isFreeze && freezeTurnCount > 0)
        {
            GameObject fPrefab = Instantiate(freezePrefab, transform.parent);
            yield return new WaitForSeconds(1);
            Destroy(fPrefab);
            freezeTurnCount--;
            frozen.GetComponentInChildren<Text>().text = freezeTurnCount.ToString();
            if (freezeTurnCount == 0)
            {
                isFreeze = false;
                frozen.SetActive(false);
            }
        }

        else
        {
            buffCD--;
            if (buffCD <= 0)
            {
                enemyAttacking = true;
                currentState = "Attack";
                SetCharacterState(currentState);
                int randomEnemy = Random.Range(0, bm.currentEnemyList.Count);
                bm.currentEnemyList[randomEnemy].GetComponentInChildren<EnemyController>().eBuffCanvas.SetActive(true);
                yield return new WaitForSeconds(1f);
                bm.currentEnemyList[randomEnemy].GetComponentInChildren<EnemyController>().eBuffCanvas.SetActive(false);
                bm.currentEnemyList[randomEnemy].GetComponentInChildren<EnemyController>().isBuffed = true;
                if (randomEnemy == enemyId)
                {
                    bm.currentEnemyList[randomEnemy].GetComponentInChildren<EnemyController>().buffedTurnCount = 3;
                }
                else
                {
                    bm.currentEnemyList[randomEnemy].GetComponentInChildren<EnemyController>().buffedTurnCount = 2;
                }
                bm.currentEnemyList[randomEnemy].GetComponentInChildren<EnemyController>().buffed.GetComponentInChildren<Text>().text = "2";
                bm.currentEnemyList[randomEnemy].GetComponentInChildren<EnemyController>().buffed.SetActive(true);
                buffCD = 3;
            }
            else
            {
                enemyAttacking = true;
                bm.playerShakeObject.GetComponent<ScreenShake>().TriggerShake();
                yield return EnemyAttack();
            }
        }

        yield return CheckEnemyStatus();
    }

    private IEnumerator Joker()
    {
        if (isFreeze && freezeTurnCount > 0)
        {
            GameObject fPrefab = Instantiate(freezePrefab, transform.parent);
            yield return new WaitForSeconds(1);
            Destroy(fPrefab);
            freezeTurnCount--;
            frozen.GetComponentInChildren<Text>().text = freezeTurnCount.ToString();
            if (freezeTurnCount == 0)
            {
                isFreeze = false;
                frozen.SetActive(false);
            }
        }

        else
        {
            yield return RollDice();

            if (jokerTarget == "player")
            {
                enemyAttacking = true;
                bm.playerShakeObject.GetComponent<ScreenShake>().TriggerShake();
                yield return EnemyAttack();
            }
            else if (jokerTarget == "ally")
            {
                int damage = atk;
                int randomAlly;

                if (bm.currentEnemyList.Count > 1)
                {
                    randomAlly = Random.Range(0, bm.currentEnemyList.Count);
                    while (randomAlly == enemyId)
                    {
                        randomAlly = Random.Range(0, bm.currentEnemyList.Count);
                    }
                }
                else
                {
                    randomAlly = enemyId;
                }

                if (bm.currentEnemyList[randomAlly].GetComponentInChildren<EnemyController>().isExposed)
                {
                    damage *= 2;
                }

                if (bm.currentEnemyList[randomAlly].GetComponentInChildren<EnemyController>().isSorrow)
                {
                    damage *= 4;
                }

                AudioManager.Instance.Play(enemy.damageSFX);
                bm.currentEnemyList[randomAlly].GetComponentInChildren<EnemyController>().currentState = "Damage";
                bm.currentEnemyList[randomAlly].GetComponentInChildren<EnemyController>().SetCharacterState(bm.currentEnemyList[randomAlly].GetComponentInChildren<EnemyController>().currentState);
                bm.currentEnemyList[randomAlly].GetComponentInChildren<ScreenShake>().TriggerShake();
                yield return new WaitForSeconds(0.5f);
                bm.EDamagePopup(bm.currentEnemyList[randomAlly].transform, (int)damage, "normalEnemy", false, bm.enemyNumPopupObj);
                bm.EnemyDamage((int)damage, randomAlly);
                yield return new WaitForSeconds(1f);
                bm.currentEnemyList[randomAlly].GetComponentInChildren<EnemyController>().currentState = "Idle";
                bm.currentEnemyList[randomAlly].GetComponentInChildren<EnemyController>().SetCharacterState(bm.currentEnemyList[randomAlly].GetComponentInChildren<EnemyController>().currentState);
            }
        }

        yield return CheckEnemyStatus();
    }

    private IEnumerator RollDice()
    {
        float duration = Time.time + 3f;
        bm.jokerDiceDisplay.GetComponentInChildren<Text>().text = "";
        diceIndex = Random.Range(0, 6);
        bm.jokerDiceDisplay.SetActive(true);

        while (Time.time < duration)
        {
            bm.jokerDiceDisplay.GetComponentInChildren<RawImage>().texture = diceImages[diceIndex];
            yield return new WaitForSeconds(0.1f);

            if (diceIndex >= 5)
            {
                diceIndex = 0;
            }
            else
            {
                diceIndex++;
            }
        }

        bm.jokerDiceDisplay.GetComponentInChildren<RawImage>().texture = diceImages[diceIndex];

        if ((diceIndex + 1) % 2 == 0)
        {
            jokerTarget = "player";
        }
        else
        {
            jokerTarget = "ally";
        }
        bm.jokerDiceDisplay.GetComponentInChildren<Text>().text = eText.text + " will attack " + jokerTarget;
        yield return new WaitForSeconds(2);
        bm.jokerDiceDisplay.SetActive(false);
    }

    private IEnumerator CheckEnemyStatus()
    {
        yield return new WaitForSeconds(0.5f);
        currentState = "Idle";
        SetCharacterState(currentState);

        bm.CheckPlayerDeath();

        if (isPoisoned)
        {
            float poisonDmg = enemyHealthSlider.maxValue * 0.08f;
            EStatusTurnChange((int)poisonDmg, ref poisonTurnCount, ref isPoisoned, poisoned, "Poison");
        }

        if (isScald)
        {
            EStatusTurnChange(0, ref scaldTurnCount, ref isScald, scalded, "Scald");
        }

        if (isBurn)
        {
            float burnDmg = enemyHealthSlider.maxValue * 0.06f;
            EStatusTurnChange((int)burnDmg, ref burnTurnCount, ref isBurn, burned, "Burn");
        }

        if (isBuffed)
        {
            EStatusTurnChange(0, ref buffedTurnCount, ref isBuffed, buffed, "Buffed");
        }

        enemyAttacking = false;

        yield return new WaitForSeconds(0.5f);
        yield return bm.CheckEnemyDeath(enemyId);
    }
}
