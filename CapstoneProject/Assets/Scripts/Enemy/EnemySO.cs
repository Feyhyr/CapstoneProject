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
    public List<Sprite> weakImages;
    public List<string> resistance;
    public List<Sprite> resistImages;
    public List<string> immunity;
    public List<Sprite> immuneImages;
    public AnimationReferenceAsset idle;
    public AnimationReferenceAsset attack;
    public AnimationReferenceAsset damage;
    public SkeletonDataAsset skeletonData;
    public AudioClip attackSFX;
    public AudioClip damageSFX;
    public string attackType;
    public float healingPercent;
    public int healingTargets;
}
