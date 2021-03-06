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
    [TextArea(15, 20)]
    public string description;
    public int maxCooldown;
    public int spellIndex;
    public string runeA;
    public string runeB;
}
