using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemySO enemy;
    public int enemCurrentHealth;
    public int lowestDmg;
    public int highestDmg;

    private void Awake()
    {
        enemCurrentHealth = enemy.maxHealth;
        lowestDmg = enemy.lowDmg;
        highestDmg = enemy.highDmg;
    }
}
