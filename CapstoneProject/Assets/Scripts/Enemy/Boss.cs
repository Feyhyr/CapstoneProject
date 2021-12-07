using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : EnemyController
{
    public EnemySO hellSpireSummon;
    private int summonCount = 0;

    public Sprite TypeImage;

    public GameObject lanternPrefab;
    public int hiddenFishTurnCount = 0;

    protected override void Start()
    {
        base.Start();

        TypeIcon.sprite = TypeImage;
        TypeText.text = "Boss";
    }

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

    private IEnumerator LavaGolem()
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

        hiddenFishTurnCount++;
        yield return CheckEnemyStatus();
    }

    private IEnumerator Spider()
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
                if (bm.ChanceStatusEffect(0.3f))
                {
                    yield return new WaitForSeconds(0.5f);
                    bm.debuffCanvas.SetActive(true);
                    yield return new WaitForSeconds(1f);
                    bm.debuffCanvas.SetActive(false);
                    bm.isCharBound = true;
                    //bm.charStuckTurnCount = 1;
                    //bm.charStuck.GetComponentInChildren<Text>().text = bm.charStuckTurnCount.ToString();
                    bm.bound.SetActive(true);
                    //bm.charStuck.SetActive(true);
                }

                if (bm.ChanceStatusEffect(0.3f))
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

    private IEnumerator HellSpire()
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

            if (((enemyHealthSlider.value / enemy.maxHealth) <= 0.3f) && bm.currentEnemyList.Count > 1 && !isPoisoned)
            {
                int eaten = 0;
                
                AudioManager.Instance.Play(attackSFX);
                currentState = "Attack";
                SetCharacterState(currentState);
                yield return new WaitForSeconds(1);
                for (int i = bm.currentEnemyList.Count - 1; i > 0; i--)
                {
                    GameObject tempGameObject = bm.currentEnemyList[i];
                    AudioManager.Instance.Play(bm.currentEnemyList[i].GetComponentInChildren<EnemyController>().damageSFX);
                    bm.currentEnemyList[i].GetComponentInChildren<ScreenShake>().TriggerShake();
                    yield return new WaitForSeconds(1);
                    bm.currentEnemyList[i].SetActive(false);
                    bm.currentEnemyList.RemoveAt(i);
                    Destroy(tempGameObject);
                    eaten++;

                    int newEnemyIndex = 0;
                    for (int j = 0; j < bm.currentEnemyList.Count; j++)
                    {
                        bm.currentEnemyList[j].GetComponentInChildren<EnemyController>().enemyId = newEnemyIndex;
                        newEnemyIndex++;
                    }
                }
                bm.targetEnemy = 0;
                yield return new WaitForSeconds(0.5f);
                eBuffCanvas.SetActive(true);
                yield return new WaitForSeconds(1f);
                eBuffCanvas.SetActive(false);
                float healAmount = (enemy.maxHealth * 0.2f) * eaten;
                enemyHealthSlider.value += healAmount;
                bm.EDamagePopup(transform.parent, (int)healAmount, "normalEnemy", true, bm.enemyNumPopupObj);
            }

            else
            {
                if (bm.currentEnemyList.Count == 1 && summonCount < 8)
                {
                    isSummoning = true;
                    for (int i = 0; i < 2; i++)
                    {
                        GameObject ePrefab;
                        int enemyIndex = i + 1;
                        ePrefab = Instantiate(bm.enemyPrefab, bm.enemyLocation);
                        if (i == 0)
                        {
                            gameObject.transform.parent.SetAsLastSibling();
                        }
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
                    }
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

    public IEnumerator SpawnLantern()
    {
        yield return bm.FadeInFishCover(bm.fishCover.GetComponent<Image>());
        GameObject lantern = Instantiate(lanternPrefab, bm.lanternLocation);
        lantern.GetComponentInChildren<EnemyController>().bm = bm;
        lantern.GetComponentInChildren<EnemyController>().enemyId = 1;
        lantern.tag = lantern.GetComponentInChildren<EnemyController>().enemy.tagName;
        lantern.GetComponentInChildren<EnemyController>().eText.text = lantern.GetComponentInChildren<EnemyController>().enemy.enemyName;
        bm.currentEnemyList.Add(lantern);
        bm.targetEnemy = 1;
    }
}
