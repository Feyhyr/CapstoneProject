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
    public bool isDead;
    public List<string> weak;
    public List<string> resist;
    public List<string> immune;
    public AnimationReferenceAsset eIdle;
    public AnimationReferenceAsset eAttack;
    public AnimationReferenceAsset eDamage;
    public SkeletonAnimation eSkeletonAnimation;

    public string currentState;

    public GameObject burned;
    public GameObject frozen;
    public GameObject poisoned;
    public GameObject exposed;
    public GameObject scalded;

    public GameObject targetSelected;

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
    public GameObject enemyCover;
    public bool enemyAttacking;

    private void Awake()
    {
        enemyHealthSlider.value = enemy.maxHealth;
        eText.text = enemy.enemyName;
        atk = enemy.dmg;
        weak = enemy.weakness;
        resist = enemy.resistance;
        immune = enemy.immunity;
        eIdle = enemy.idle;
        eAttack = enemy.attack;
        eDamage = enemy.damage;
        attackSFX = enemy.attackSFX;

        currentState = "Idle";
        SetCharacterState(currentState);

        StartCoroutine(FadeIn());
    }

    private void Update()
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

    public void SelectTarget()
    {
        if ((bm.battleState == BattleManager.BattleState.PLAYERTURN) && (!bm.playerAttacked))
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
