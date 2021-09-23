using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public Sprite enemySprite;
    public int dmg;
    public int maxHealth;
    public List<string> weakness;
    public List<string> resistance;
    public List<string> immunity;
}
