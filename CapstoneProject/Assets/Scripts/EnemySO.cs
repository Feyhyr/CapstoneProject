using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

[CreateAssetMenu(menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    //public Sprite enemySprite;
    public int dmg;
    public int maxHealth;
    public List<string> weakness;
    public List<string> resistance;
    public List<string> immunity;
    public AnimationReferenceAsset idle;
    public AnimationReferenceAsset attack;
    public AnimationReferenceAsset damage;
    
    public AudioClip attackSFX;
}
