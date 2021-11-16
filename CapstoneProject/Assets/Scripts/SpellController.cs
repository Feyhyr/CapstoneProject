using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpellController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SpellSO spell;
    public Text nameText;
    public Sprite spellIcon;
    public string spellDescription;
    public Text infoText;
    public BattleManager bm;
    public GameObject message;
    public GameObject selectedState;

    public bool onCD;
    public int maxCD;
    public int currentCD;
    public GameObject counterCDText;
    public GameObject CDCover;

    private void Start()
    {
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        maxCD = spell.maxCooldown;
        currentCD = 0;
        nameText.text = spell.sName;
        spellIcon = spell.icon;
        gameObject.GetComponent<Image>().sprite = spellIcon;
        spellDescription = spell.description;
        infoText.text = spellDescription;
    }

    public void SpellSelect()
    {
        bm.cancelBTN.SetActive(false);
        gameObject.GetComponent<Button>().interactable = false;
        selectedState.SetActive(false);
        message.SetActive(false);
        if (gameObject.tag != "Reverb")
        {
            CDCover.SetActive(true);
            currentCD = maxCD;
            counterCDText.GetComponent<Text>().text = maxCD.ToString();
            counterCDText.SetActive(true);
            onCD = true;
        }
        bm.TriggerAttack();
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameObject.GetComponent<Button>().colors.disabledColor == Color.white)
        {
            message.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        message.SetActive(false);
    }
}
