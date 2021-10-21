using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCreation : MonoBehaviour
{
    public SpellSO spell;
    public string spellName;
    public int damage;
    public AudioClip sfx;

    private void Start()
    {
        damage = spell.sDamage;
        sfx = spell.sfx;
        AudioManager.Instance.Play(sfx);
        Destroy(gameObject, 5.0f);
    }
}
