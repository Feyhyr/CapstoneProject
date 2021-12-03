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
    BattleManager bm;
    public GameObject message;
    public GameObject selectedState;

    public bool onCD;
    public int maxCD;
    public int currentCD;
    public GameObject counterCDText;
    public GameObject CDCover;

    ToggleSettings toggleSetting;

    private void Start()
    {
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        toggleSetting = GameObject.Find("ToggleSetting").GetComponent<ToggleSettings>();
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
        bm.tutorialUX.SetActive(false);
        bm.tutorialSelectUX.SetActive(false);
        //bm.cancelBTN.SetActive(false);
        bm.isSpellCasting = true;
        for (int i = 0; i < bm.runeObjs.Length; i++)
        {
            bm.runeObjs[i].GetComponent<RuneController>().canvasGroup.interactable = false;
            bm.runeObjs[i].GetComponent<RuneController>().canvasGroup.alpha = 0.2f;
        }
        
        if (toggleSetting.instantSpell)
        {
            bm.rune1 = spell.runeA;
            bm.rune2 = spell.runeB;
            for (int i = 0; i < bm.spellBTNList.Count; i++)
            {
                if (!bm.spellBTNList[i].GetComponent<SpellController>().onCD)
                {
                    bm.spellBTNList[i].GetComponent<Button>().interactable = false;
                    bm.spellBTNList[i].GetComponent<SpellController>().selectedState.SetActive(false);
                }
            }
        }
        else
        {
            gameObject.GetComponent<Button>().interactable = false;
            selectedState.SetActive(false);
        }

        message.SetActive(false);
        if (gameObject.tag != "Reverb")
        {
            CDCover.SetActive(true);
            currentCD = maxCD;
            counterCDText.GetComponent<Text>().text = maxCD.ToString();
            counterCDText.SetActive(true);
            onCD = true;
        }
        if (toggleSetting.instantSpell)
        {
            StartCoroutine(bm.CreateSpell(spell.spellIndex));
        }
        else
        {
            bm.TriggerAttack();
        }
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
