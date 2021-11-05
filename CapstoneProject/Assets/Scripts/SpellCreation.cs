using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCreation : MonoBehaviour
{
    public SpellSO spell;
    public int damage;

    public void DestroySpell()
    {
        Destroy(gameObject);
    }
}
