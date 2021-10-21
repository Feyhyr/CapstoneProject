using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellController : MonoBehaviour
{
    public SpellSO spell;
    public Text nameText;
    public Sprite spellIcon;
    public string spellDescription;
    public Text infoText;

    public BattleManager bm;

    public GameObject message;

    private void Start()
    {
        infoText.text = spellDescription;
    }

    public void SpellSelect()
    {
        bm.TriggerAttack();
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void OnMouseEnter()
    {
        message.SetActive(true);
    }

    private void OnMouseExit()
    {
        message.SetActive(false);
    }
}
