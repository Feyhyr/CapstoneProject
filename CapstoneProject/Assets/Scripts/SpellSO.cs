using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell")]
public class SpellSO : ScriptableObject
{
    public string spellName;
    public GameObject particleEffect;
}
