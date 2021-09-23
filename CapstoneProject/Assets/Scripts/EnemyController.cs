using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class EnemyController : MonoBehaviour
{
    public EnemySO enemy;
    public int enemCurrentHealth;
    public int atk;
    public List<string> weak;
    public List<string> resist;
    public List<string> immune;
    //public SpriteRenderer spriteRenderer;
    public AnimationReferenceAsset eIdle;
    public AnimationReferenceAsset eAttack;
    public AnimationReferenceAsset eDamage;
    public SkeletonAnimation eSkeletonAnimation;

    public string currentState;

    private void Awake()
    {
        enemCurrentHealth = enemy.maxHealth;
        atk = enemy.dmg;
        weak = enemy.weakness;
        resist = enemy.resistance;
        immune = enemy.immunity;
        //spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        //spriteRenderer.sprite = enemy.enemySprite;
        eIdle = enemy.idle;
        eAttack = enemy.attack;
        eDamage = enemy.damage;

        currentState = "Idle";
        SetCharacterState(currentState);
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
    }
}
