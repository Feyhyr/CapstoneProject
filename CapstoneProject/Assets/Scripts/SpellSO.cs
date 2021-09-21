using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell")]
public class SpellSO : ScriptableObject
{
    public RuneSO rune1;
    public RuneSO rune2;
    public string spellName;
}
