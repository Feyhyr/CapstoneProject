using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Basic : EnemyController
{
    public List<Sprite> TypeList;

    protected override void Start()
    {
        base.Start();

        if (enemyType == "Attack")
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
        TypeText.text = enemyType;
    }

    public override IEnumerator AttackPattern()
    {
        if (enemyType == "Attack")
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
    }

    private IEnumerator Attack()
    {
        if (isFreeze && freezeTurnCount > 0)
        {
            GameObject fPrefab = Instantiate(freezePrefab, transform);
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
        float healAmount = 0;

        if (isFreeze && freezeTurnCount > 0)
        {
            GameObject fPrefab = Instantiate(freezePrefab, transform);
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
            int healTarget = 0;
            float lowestHealth = 0;

            for (int i = 0; i < bm.currentEnemyList.Count; i++)
            {
                if (bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyHealthSlider.value < bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemy.maxHealth)
                {
                    if (!bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().eCannotHeal)
                    {
                        if (lowestHealth == 0)
                        {
                            lowestHealth = bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyHealthSlider.value;
                            healAmount = bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemy.maxHealth * 0.1f;
                            healTarget = i;
                        }
                        else if (lowestHealth > bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyHealthSlider.value)
                        {
                            lowestHealth = bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyHealthSlider.value;
                            healAmount = bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemy.maxHealth * 0.1f;
                            healTarget = i;
                        }
                        isHealing = true;
                    }
                }
            }

            if (isHealing)
            {
                bm.currentEnemyList[healTarget].GetComponentInChildren<EnemyController>().eBuffCanvas.SetActive(true);
                yield return new WaitForSeconds(1f);
                bm.currentEnemyList[healTarget].GetComponentInChildren<EnemyController>().eBuffCanvas.SetActive(false);
                bm.currentEnemyList[healTarget].GetComponentInChildren<EnemyController>().enemyHealthSlider.value += healAmount;
                bm.EDamagePopup(bm.currentEnemyList[healTarget].transform, (int)healAmount, "normalEnemy", true, bm.enemyNumPopupObj);
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
            GameObject fPrefab = Instantiate(freezePrefab, transform);
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
            GameObject fPrefab = Instantiate(freezePrefab, transform);
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

            if (tag == "Hellhound")
            {
                if (bm.ChanceStatusEffect(0.5f))
                {
                    yield return new WaitForSeconds(0.5f);
                    bm.debuffCanvas.SetActive(true);
                    yield return new WaitForSeconds(1f);
                    bm.debuffCanvas.SetActive(false);
                    bm.isCharExposed = true;
                    bm.charExposedTurnCount = 2;
                    bm.charExposed.GetComponentInChildren<Text>().text = bm.charExposedTurnCount.ToString();
                    bm.charExposed.SetActive(true);
                }
            }
            else
            {
                if (bm.ChanceStatusEffect(0.7f))
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
                    if (bm.ChanceStatusEffect(0.7f))
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

                if (bm.ChanceStatusEffect(0.7f))
                {
                    yield return new WaitForSeconds(0.5f);
                    bm.debuffCanvas.SetActive(true);
                    yield return new WaitForSeconds(1f);
                    bm.debuffCanvas.SetActive(false);
                    bm.isCharPoisoned = true;
                    bm.charPoisonedTurnCount = 2;
                    bm.playerPoison.GetComponentInChildren<Text>().text = bm.charPoisonedTurnCount.ToString();
                    bm.playerPoison.SetActive(true);
                    bm.pCannotHeal = true;
                }
            }
        }

        yield return CheckEnemyStatus();
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

        enemyAttacking = false;

        yield return new WaitForSeconds(0.5f);
        yield return bm.CheckEnemyDeath(enemyId);
    }
}
