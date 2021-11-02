using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Basic : EnemyController
{
    public override void AttackPattern()
    {
        base.AttackPattern();

        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        if (isFreeze && freezeTurnCount > 0)
        {
            GameObject fPrefab = Instantiate(freezePrefab, transform);
            yield return new WaitForSeconds(2);
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
            Debug.Log("Starting");
            enemyAttacking = true;
            bm.cameraObject.GetComponent<ScreenShake>().TriggerShake();
            EnemyAttack();
            if (tag == "Human" || tag == "Boss")
            {
                if (bm.ChanceStatusEffect(0.7f))
                {
                    bm.isCharCursed = true;
                    bm.charCursedTurnCount = 2;
                    bm.curse.GetComponentInChildren<Text>().text = bm.charCursedTurnCount.ToString();
                    bm.curse.SetActive(true);
                }

                if (!bm.isCharSealed)
                {
                    if (bm.ChanceStatusEffect(0.7f))
                    {
                        bm.sealedRuneIndex = Random.Range(0, 4);
                        bm.runeObjs[bm.sealedRuneIndex].GetComponent<Button>().interactable = false;
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
                    bm.isCharPoisoned = true;
                    bm.charPoisonedTurnCount = 2;
                    bm.playerPoison.GetComponentInChildren<Text>().text = bm.charPoisonedTurnCount.ToString();
                    bm.playerPoison.SetActive(true);
                }
            }
        }

        yield return new WaitForSeconds(1);
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

        yield return new WaitForSeconds(1);
        bm.CheckEnemyDeath(enemyId);
    }
}
