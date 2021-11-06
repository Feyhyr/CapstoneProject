using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : EnemyController
{
    public EnemySO hellSpireSummon;

    public override IEnumerator AttackPattern()
    {
        if (enemyType == "Attack")
        {
            yield return Attack();
        }
        else if (enemyType == "Wendigo")
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

            if (bm.currentEnemyList.Count < 3)
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
