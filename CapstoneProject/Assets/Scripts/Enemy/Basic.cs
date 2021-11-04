using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Basic : EnemyController
{
    public override IEnumerator AttackPattern()
    {
        //base.AttackPattern();

        if (tag == "Summon")
        {
            //StartCoroutine(Attack());
            yield return Attack();
        }
        else
        {
            //StartCoroutine(Debuffer());
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
            bm.cameraObject.GetComponent<ScreenShake>().TriggerShake();
            yield return EnemyAttack();
        }

        yield return CheckEnemyStatus();
    }

    private IEnumerator Healer()
    {
        bool ishealing = false;

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

            for (int i = 0; i < bm.currentEnemyList.Count; i++)
            {
                if (bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyHealthSlider.value < bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemy.maxHealth)
                {
                    ishealing = true;
                    bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().enemyHealthSlider.value += 10;
                    bm.EDamagePopup(bm.currentEnemyList[i].transform, 10, "normalEnemy", true, bm.enemyNumPopupObj);
                    currentState = "Attack";
                    SetCharacterState(currentState);
                }
            }

            if (!ishealing)
            {
                bm.cameraObject.GetComponent<ScreenShake>().TriggerShake();
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
                bm.cameraObject.GetComponent<ScreenShake>().TriggerShake();
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
            bm.cameraObject.GetComponent<ScreenShake>().TriggerShake();
            yield return EnemyAttack();

            if (tag == "Human" || tag == "Boss")
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
                        bm.charSealedTurnCount = 2;
                        bm.seal.GetComponentInChildren<Text>().text = bm.charSealedTurnCount.ToString();
                        bm.seal.SetActive(true);
                        bm.isCharSealed = true;
                    }
                }
            }
            else if (tag == "Tree")
            {
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
            StatusTurnChange(3, ref poisonTurnCount, ref isPoisoned, poisoned);
        }

        if (isScald)
        {
            StatusTurnChange(0, ref scaldTurnCount, ref isScald, scalded);
        }

        if (isBurn)
        {
            StatusTurnChange(1, ref burnTurnCount, ref isBurn, burned);
        }

        enemyAttacking = false;

        yield return new WaitForSeconds(0.5f);
        bm.CheckEnemyDeath(enemyId);
    }
}
