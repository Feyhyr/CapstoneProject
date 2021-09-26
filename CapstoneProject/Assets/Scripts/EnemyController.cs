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
    public GameObject melted;

    public GameObject targetSelected;

    public bool isPoisoned;
    public int poisonTurnCount;
    public bool isExposed;
    public int exposedTurnCount;
    public bool isMelt;
    public int meltTurnCount;
    public bool isFreeze;
    public int freezeTurnCount;
    public bool isBurn;
    public int burnTurnCount;

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

        currentState = "Idle";
        SetCharacterState(currentState);
}

    public void SelectTarget()
    {
        if ((bm.battleState == BattleManager.BattleState.PLAYERTURN) && (!bm.playerAttacked))
        {
            bm.ChangeTarget(bm.targetEnemy);
            bm.targetEnemy = enemyId;
            targetSelected.SetActive(true);
            Debug.Log(eText.text + " selected");
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
}
