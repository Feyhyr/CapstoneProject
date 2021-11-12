using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : EnemyController
{
    public EnemySO hellSpireSummon;
    private int summonCount = 0;

    public override IEnumerator AttackPattern()
    {
        if (enemyType == "Wendigo")
        {
            yield return Wendigo();
        }
        else if (enemyType == "LavaGolem")
        {
            yield return LavaGolem();
        }
        else if (enemyType == "AnglerFish")
        {
            yield return AnglerFish();
        }
        else if (enemyType == "Spider")
        {
            yield return Spider();
        }
        else if (enemyType == "HellSpire")
        {
            yield return HellSpire();
        }
    }

    private IEnumerator Wendigo()
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

    private IEnumerator LavaGolem()
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
            yield return new WaitForSeconds(0.7f);
            eBuffCanvas.SetActive(true);
            yield return new WaitForSeconds(1f);
            eBuffCanvas.SetActive(false);
            float healAmount = enemy.maxHealth * 0.05f;
            enemyHealthSlider.value += healAmount;
            bm.EDamagePopup(transform.parent, (int)healAmount, "normalEnemy", true, bm.enemyNumPopupObj);
        }

        yield return CheckEnemyStatus();
    }

    private IEnumerator AnglerFish()
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

    private IEnumerator Spider()
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

            if (bm.ChanceStatusEffect(0.7f))
            {
                yield return new WaitForSeconds(0.5f);
                bm.debuffCanvas.SetActive(true);
                yield return new WaitForSeconds(1f);
                bm.debuffCanvas.SetActive(false);
                bm.isCharStuck = true;
                bm.charStuckTurnCount = 1;
                bm.charStuck.GetComponentInChildren<Text>().text = bm.charStuckTurnCount.ToString();
                bm.charStuck.SetActive(true);
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

        yield return CheckEnemyStatus();
    }

    private IEnumerator HellSpire()
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

            if (((enemyHealthSlider.value / enemy.maxHealth) <= 0.5f) && bm.currentEnemyList.Count > 1)
            {
                AudioManager.Instance.Play(attackSFX);
                currentState = "Attack";
                SetCharacterState(currentState);
                yield return new WaitForSeconds(1);
                AudioManager.Instance.Play(bm.currentEnemyList[1].GetComponentInChildren<EnemyController>().damageSFX);
                bm.currentEnemyList[1].GetComponentInChildren<ScreenShake>().TriggerShake();
                yield return new WaitForSeconds(1);
                bm.currentEnemyList[1].SetActive(false);
                bm.currentEnemyList.RemoveAt(1);
                yield return new WaitForSeconds(0.5f);
                eBuffCanvas.SetActive(true);
                yield return new WaitForSeconds(1f);
                eBuffCanvas.SetActive(false);
                float healAmount = enemy.maxHealth * 0.2f;
                enemyHealthSlider.value += healAmount;
                bm.EDamagePopup(transform.parent, (int)healAmount, "normalEnemy", true, bm.enemyNumPopupObj);
            }

            else
            {
                if (bm.currentEnemyList.Count < 3 && summonCount < 8)
                {
                    isSummoning = true;
                    GameObject ePrefab;
                    int enemyIndex = bm.currentEnemyList.Count;
                    ePrefab = Instantiate(bm.enemyPrefab, bm.enemyLocation);

                    ePrefab.GetComponentInChildren<EnemyController>().bm = bm;
                    ePrefab.GetComponentInChildren<EnemyController>().enemy = hellSpireSummon;
                    ePrefab.GetComponentInChildren<EnemyController>().enemyId = enemyIndex;
                    ePrefab.tag = ePrefab.GetComponentInChildren<EnemyController>().enemy.tagName;
                    foreach (Transform t in ePrefab.transform)
                    {
                        t.gameObject.tag = ePrefab.tag;
                    }
                    ePrefab.GetComponentInChildren<EnemyController>().eText.text = ePrefab.GetComponentInChildren<EnemyController>().enemy.enemyName;
                    bm.currentEnemyList.Add(ePrefab);
                    summonCount++;
                    yield return new WaitForSeconds(0.5f);
                }

                if (!isSummoning)
                {
                    bm.playerShakeObject.GetComponent<ScreenShake>().TriggerShake();
                    yield return EnemyAttack();
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
            StatusTurnChange((int)poisonDmg, ref poisonTurnCount, ref isPoisoned, poisoned, "Poison");
        }

        if (isScald)
        {
            StatusTurnChange(0, ref scaldTurnCount, ref isScald, scalded, "Scald");
        }

        if (isBurn)
        {
            float burnDmg = enemyHealthSlider.maxValue * 0.06f;
            StatusTurnChange((int)burnDmg, ref burnTurnCount, ref isBurn, burned, "Burn");
        }

        enemyAttacking = false;

        yield return new WaitForSeconds(0.5f);
        bm.CheckEnemyDeath(enemyId);
    }
}
