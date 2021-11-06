using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

[CreateAssetMenu(menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public string tagName;
    public int dmg;
    public int maxHealth;
    public List<string> weakness;
    public List<string> resistance;
    public List<string> immunity;
    public AnimationReferenceAsset idle;
    public AnimationReferenceAsset attack;
    public AnimationReferenceAsset damage;
    public SkeletonDataAsset skeletonData;
    public AudioClip attackSFX;
    public string attackType;
}
