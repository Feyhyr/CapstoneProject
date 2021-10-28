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
    public GameObject selectedState;

    private void Start()
    {
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        nameText.text = spell.sName;
        spellIcon = spell.icon;
        gameObject.GetComponent<Image>().sprite = spellIcon;
        spellDescription = spell.description;
        infoText.text = spellDescription;
    }

    public void SpellSelect()
    {
        gameObject.GetComponent<Button>().interactable = false;
        selectedState.SetActive(false);
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
