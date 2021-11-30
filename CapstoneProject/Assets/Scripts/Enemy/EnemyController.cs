using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class EnemyController : MonoBehaviour
{
    public BattleManager bm;
    public EnemySO enemy;
    public Slider enemyHealthSlider;
    public int atk;
    public int enemyId;
    public Text eText;
    public Text eHPText;
    public bool isDead;
    public string enemyType;
    public List<string> weak;
    public List<string> resist;
    public List<string> immune;
    private AnimationReferenceAsset eIdle;
    private AnimationReferenceAsset eAttack;
    private AnimationReferenceAsset eDamage;
    public SkeletonAnimation eSkeletonAnimation;
    public GameObject enemyShake;

    public string currentState;

    public GameObject burned;
    public GameObject frozen;
    public GameObject poisoned;
    public GameObject exposed;
    public GameObject scalded;

    public GameObject targetSelected;

    public bool eCannotHeal;
    public bool isPoisoned;
    public int poisonTurnCount;
    public bool isExposed;
    public int exposedTurnCount;
    public bool isScald;
    public int scaldTurnCount;
    public bool isFreeze;
    public int freezeTurnCount;
    public bool isBurn;
    public int burnTurnCount;

    public AudioClip uiClick;
    public AudioClip attackSFX;
    public AudioClip damageSFX;
    public GameObject enemyCover;
    public bool enemyAttacking;

    public GameObject freezePrefab;
    public GameObject eDebuffCanvas;
    public GameObject eBuffCanvas;

    public Image TypeIcon;
    public Text TypeText;

    public GameObject enemyResistance;
    public Transform enemyWeakLocation;
    public Transform enemyResistLocation;
    public Transform enemyImmuneLocation;

    protected virtual void Start()
    {
        GetComponent<SkeletonAnimation>().skeletonDataAsset = enemy.skeletonData;
        GetComponent<SkeletonAnimation>().Initialize(true);
        eSkeletonAnimation = GetComponent<SkeletonAnimation>();
        enemyHealthSlider.maxValue = enemy.maxHealth;
        enemyHealthSlider.value = enemy.maxHealth;
        atk = enemy.dmg;
        weak = enemy.weakness;
        resist = enemy.resistance;
        immune = enemy.immunity;
        eIdle = enemy.idle;
        eAttack = enemy.attack;
        eDamage = enemy.damage;
        attackSFX = enemy.attackSFX;
        damageSFX = enemy.damageSFX;
        enemyType = enemy.attackType;

        currentState = "Idle";
        SetCharacterState(currentState);

        if (tag != "Lantern")
        {
            for (int i = 0; i < enemy.weakImages.Count; i++)
            {
                GameObject r = Instantiate(enemyResistance, enemyWeakLocation);
                r.GetComponent<Image>().sprite = enemy.weakImages[i];
            }

            for (int i = 0; i < enemy.resistImages.Count; i++)
            {
                GameObject r = Instantiate(enemyResistance, enemyResistLocation);
                r.GetComponent<Image>().sprite = enemy.resistImages[i];
            }

            for (int i = 0; i < enemy.immuneImages.Count; i++)
            {
                GameObject r = Instantiate(enemyResistance, enemyImmuneLocation);
                r.GetComponent<Image>().sprite = enemy.immuneImages[i];
            }
        }

        StartCoroutine(FadeIn());
    }

    private void Update()
    {
        eHPText.text = enemyHealthSlider.value.ToString() + "/" + enemy.maxHealth;
        if (tag != "Lantern")
        {
            if (bm.battleState == BattleManager.BattleState.ENEMYTURN)
            {
                if (enemyAttacking)
                {
                    enemyCover.SetActive(true);
                }
                else
                {
                    enemyCover.SetActive(false);
                }
            }
            else
            {
                enemyCover.SetActive(false);
            }
        }
    }

    public virtual IEnumerator AttackPattern()
    {
        yield return new WaitForSeconds(0.1f);
    }

    public void EStatusTurnChange(int dmg, ref int count, ref bool status, GameObject obj, string statusName)
    {
        if (dmg > 0)
        {
            GetComponent<ScreenShake>().TriggerShake();
            EnemyDamage(dmg);
            bm.EDamagePopup(transform.parent, dmg, "normalEnemy", false, bm.enemyNumPopupObj);
        }
        count--;
        obj.GetComponentInChildren<Text>().text = count.ToString();
        if (count <= 0)
        {
            status = false;
            obj.SetActive(false);
            if (statusName == "Poison")
            {
                eCannotHeal = false;
            }
        }
    }

    public void EnemyDamage(int damage)
    {
        enemyHealthSlider.value -= damage;
    }

    public IEnumerator EnemyAttack()
    {
        AudioManager.Instance.Play(attackSFX);
        int enemyDmg = atk;
        if (bm.isCharExposed)
        {
            enemyDmg *= 2;
        }
        if (bm.isCrystalize)
        {
            enemyDmg /= 2;
        }

        bm.charHealthSlider.value -= enemyDmg;
        currentState = "Attack";
        SetCharacterState(currentState);
        yield return new WaitForSeconds(1);
        bm.PDamagePopup(bm.playerLocation, enemyDmg, "normalPlayer", false, bm.playerNumPopupObj);

        if ((tag == "Vampire" || enemyType == "Wendigo") && !isPoisoned)
        {
            int healAmount = enemyDmg / 2;
            eBuffCanvas.SetActive(true);
            yield return new WaitForSeconds(1f);
            eBuffCanvas.SetActive(false);
            enemyHealthSlider.value += healAmount;
            bm.EDamagePopup(transform.parent, healAmount, "normalEnemy", true, bm.enemyNumPopupObj);
        }
    }

    public void SelectTarget()
    {
        if ((bm.battleState == BattleManager.BattleState.PLAYERTURN) && (!bm.playerAttacked) && (!bm.isSpellCasting))
        {
            AudioManager.Instance.Play(uiClick);
            bm.ChangeTarget(bm.targetEnemy);
            bm.targetEnemy = enemyId;
            targetSelected.SetActive(true);
        }
    }

    public void SetAnimation(AnimationReferenceAsset animation, bool loop, float timeScale)
    {
        eSkeletonAnimation.state.SetAnimation(0, animation, loop).TimeScale = timeScale;
    }

    public void SetCharacterState(string state)
    {
        if (state.Equals("Idle"))
        {
            SetAnimation(eIdle, true, 1f);
        }
        else if (state.Equals("Attack"))
        {
            SetAnimation(eAttack, false, 1f);
        }
        else if (state.Equals("Damage"))
        {
            SetAnimation(eDamage, false, 1f);
        }
    }

    IEnumerator FadeIn(int steps = 10)
    {
        float totalTransparencyPerStep = 1 / (float)steps;

        for (int i = 0; i < steps; i++)
        {
            ChangeOpacity(totalTransparencyPerStep);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void ChangeOpacity(float opacityStep)
    {
        float alpha = eSkeletonAnimation.skeleton.A;
        alpha += opacityStep;
        eSkeletonAnimation.skeleton.A = alpha;
    }
}
