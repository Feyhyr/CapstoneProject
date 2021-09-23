using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemySO enemy;
    public int enemCurrentHealth;
    public int atk;
    public List<string> weak;
    public List<string> resist;
    public List<string> immune;
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        enemCurrentHealth = enemy.maxHealth;
        atk = enemy.dmg;
        weak = enemy.weakness;
        resist = enemy.resistance;
        immune = enemy.immunity;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = enemy.enemySprite;
    }
}
