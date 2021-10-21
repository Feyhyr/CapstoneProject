using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Spell")]
public class SpellSO : ScriptableObject
{
    public string sName;
    public GameObject particleEffect;
    public int sDamage;
    public AudioClip sfx;
    public Sprite icon;
    public string description;
}
