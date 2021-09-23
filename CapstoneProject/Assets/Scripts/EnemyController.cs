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
    //public int lowestDmg;
    //public int highestDmg;

    private void Awake()
    {
        enemCurrentHealth = enemy.maxHealth;
        atk = enemy.dmg;
        weak = enemy.weakness;
        resist = enemy.resistance;
        //lowestDmg = enemy.lowDmg;
        //highestDmg = enemy.highDmg;
    }
}
