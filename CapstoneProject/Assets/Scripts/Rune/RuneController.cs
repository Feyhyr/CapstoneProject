using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RuneController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IDropHandler
{
    public RuneSO rune;
    public Sprite runeIcon;
    BattleManager bm;
    ToggleSettings toggleSetting;

    private RectTransform rectTransform;
    public Vector3 defaultPos;
    public bool droppedOnSlot;
    public Canvas canvas;
    public CanvasGroup canvasGroup;
    public bool onFirstSlot;
    public bool onSecondSlot;
    public string slotted;
    public GameObject textBox;

    public RuneSlot firstSlot;
    public RuneSlot secondSlot;

    private void Awake()
    {
        defaultPos = transform.position;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        toggleSetting = GameObject.Find("ToggleSetting").GetComponent<ToggleSettings>();
        runeIcon = rune.icon;
        gameObject.GetComponent<Image>().sprite = runeIcon;
    }

    private void Start()
    {
        defaultPos = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        bm.spellOnCDObj.SetActive(false);
        if (canvasGroup.interactable)
        {
            textBox.SetActive(true);
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
            eventData.pointerDrag.GetComponent<RuneController>().droppedOnSlot = false;
            if (!toggleSetting.instantSpell)
            {
                RefreshSpellButton();
            }
            if (eventData.pointerDrag.GetComponent<RuneController>().onFirstSlot)
            {
                eventData.pointerDrag.GetComponent<RuneController>().onFirstSlot = false;
                firstSlot.currentlyDroppedObj = null;
                bm.rune1 = "";
            }
            if (eventData.pointerDrag.GetComponent<RuneController>().onSecondSlot)
            {
                eventData.pointerDrag.GetComponent<RuneController>().onSecondSlot = false;
                secondSlot.currentlyDroppedObj = null;
                bm.rune2 = "";
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvasGroup.interactable)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor / 0.5111522f;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        textBox.SetActive(false);
        bm.isCreatingSpell = false;
        if (canvasGroup.interactable)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            if (!droppedOnSlot)
            {
                transform.position = defaultPos;
                slotted = "";
            }
        }
    }

    public void SelectFirstRune()
    {
        if ((bm.battleState == BattleManager.BattleState.PLAYERTURN) && (!bm.playerAttacked))
        {
            if (!toggleSetting.instantSpell)
            {
                RefreshSpellButton();
            }
            slotted = "first";
            bm.rune1 = rune.runeName;
            onSecondSlot = false;
            onFirstSlot = true;
            bm.isAudioPlaying = true;
        }
    }

    public void SelectSecondRune()
    {
        if ((bm.battleState == BattleManager.BattleState.PLAYERTURN) && (!bm.playerAttacked))
        {
            if (!toggleSetting.instantSpell)
            {
                RefreshSpellButton();
            }
            slotted = "second";
            bm.rune2 = rune.runeName;
            onSecondSlot = true;
            onFirstSlot = false;
            bm.isAudioPlaying = true;
        }
    }

    void RefreshSpellButton()
    {
        bm.spellBTNList[bm.ChooseSpell()].GetComponent<Button>().interactable = false;
        bm.spellBTNList[bm.ChooseSpell()].GetComponent<SpellController>().selectedState.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canvasGroup.interactable)
        {
            AudioManager.Instance.Play(rune.audioSFX);
            rectTransform.SetAsLastSibling();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        bm.isCreatingSpell = true;
        if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<CanvasGroup>().interactable)
        {
            if (onFirstSlot)
            {
                if (eventData.pointerDrag.GetComponent<RuneController>().slotted == "second")
                {
                    firstSlot.currentlyDroppedObj.GetComponent<RectTransform>().position = secondSlot.GetComponent<RectTransform>().position;
                    firstSlot.currentlyDroppedObj.GetComponent<RuneController>().SelectSecondRune();
                    secondSlot.currentlyDroppedObj = firstSlot.currentlyDroppedObj;
                }
                else
                {
                    firstSlot.currentlyDroppedObj.GetComponent<RuneController>().droppedOnSlot = false;
                    firstSlot.currentlyDroppedObj.GetComponent<RuneController>().slotted = "";
                    firstSlot.currentlyDroppedObj.GetComponent<RuneController>().onFirstSlot = false;
                    firstSlot.currentlyDroppedObj.GetComponent<RectTransform>().position = firstSlot.currentlyDroppedObj.GetComponent<RuneController>().defaultPos;
                }

                AudioManager.Instance.Play(firstSlot.audioSFX);
                eventData.pointerDrag.GetComponent<RuneController>().droppedOnSlot = true;
                eventData.pointerDrag.GetComponent<RectTransform>().position = firstSlot.GetComponent<RectTransform>().position;
                eventData.pointerDrag.GetComponent<RuneController>().SelectFirstRune();

                firstSlot.currentlyDroppedObj = eventData.pointerDrag.GetComponent<RuneController>().gameObject;
            }
            else if (onSecondSlot)
            {
                if (eventData.pointerDrag.GetComponent<RuneController>().slotted == "first")
                {
                    secondSlot.currentlyDroppedObj.GetComponent<RectTransform>().position = firstSlot.GetComponent<RectTransform>().position;
                    secondSlot.currentlyDroppedObj.GetComponent<RuneController>().SelectFirstRune();
                    firstSlot.currentlyDroppedObj = secondSlot.currentlyDroppedObj;
                }
                else
                {
                    secondSlot.currentlyDroppedObj.GetComponent<RuneController>().droppedOnSlot = false;
                    secondSlot.currentlyDroppedObj.GetComponent<RuneController>().slotted = "";
                    secondSlot.currentlyDroppedObj.GetComponent<RuneController>().onSecondSlot = false;
                    secondSlot.currentlyDroppedObj.GetComponent<RectTransform>().position = secondSlot.currentlyDroppedObj.GetComponent<RuneController>().defaultPos;
                }

                AudioManager.Instance.Play(secondSlot.audioSFX);
                eventData.pointerDrag.GetComponent<RuneController>().droppedOnSlot = true;
                eventData.pointerDrag.GetComponent<RectTransform>().position = secondSlot.GetComponent<RectTransform>().position;
                eventData.pointerDrag.GetComponent<RuneController>().SelectSecondRune();

                secondSlot.currentlyDroppedObj = eventData.pointerDrag.GetComponent<RuneController>().gameObject;
            }
        }
    }
}
