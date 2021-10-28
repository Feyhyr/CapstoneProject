using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCreation : MonoBehaviour
{
    public SpellSO spell;
    public int damage;

    private void Start()
    {
        Destroy(gameObject, 5.0f);
    }
}
