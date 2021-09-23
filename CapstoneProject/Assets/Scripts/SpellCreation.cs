using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCreation : MonoBehaviour
{
    public SpellSO spell;
    public string spellName;
    public int damage;

    private void Start()
    {
        damage = spell.sDamage;
        Destroy(gameObject, 5.0f);
    }
}
