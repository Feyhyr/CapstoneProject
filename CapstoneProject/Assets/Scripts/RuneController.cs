using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RuneController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public RuneSO rune;
    //public Text nameText;
    public Sprite runeIcon;
    public BattleManager bm;

    private RectTransform rectTransform;
    public Vector3 defaultPos;
    public bool droppedOnSlot;
    public Canvas canvas;
    public CanvasGroup canvasGroup;
    public bool onFirstSlot;
    public bool onSecondSlot;

    private void Awake()
    {
        defaultPos = transform.position;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        //nameText.text = rune.name;
        runeIcon = rune.icon;
        gameObject.GetComponent<Image>().sprite = runeIcon;
    }

    private void Start()
    {
        defaultPos = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup.interactable)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
            eventData.pointerDrag.GetComponent<RuneController>().droppedOnSlot = false;
            if (eventData.pointerDrag.GetComponent<RuneController>().onFirstSlot)
            {
                eventData.pointerDrag.GetComponent<RuneController>().onFirstSlot = false;
                bm.rune1 = "";
            }
            if (eventData.pointerDrag.GetComponent<RuneController>().onSecondSlot)
            {
                eventData.pointerDrag.GetComponent<RuneController>().onSecondSlot = false;
                bm.rune2 = "";
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvasGroup.interactable)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup.interactable)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            if (droppedOnSlot == false)
            {
                transform.position = defaultPos;
            }
        }
    }

    public void SelectFirstRune()
    {
        if ((bm.battleState == BattleManager.BattleState.PLAYERTURN) && (!bm.playerAttacked))
        {
            bm.rune1 = rune.runeName;
            onFirstSlot = true;
        }
    }

    public void SelectSecondRune()
    {
        if ((bm.battleState == BattleManager.BattleState.PLAYERTURN) && (!bm.playerAttacked))
        {
            bm.rune2 = rune.runeName;
            onSecondSlot = true;
        }
    }

    /*public void runeSelect()
    {
        if ((bm.battleState == BattleManager.BattleState.PLAYERTURN) && (!bm.playerAttacked))
        {
            if (bm.rune1 == "")
            {
                bm.rune1 = rune.runeName;
                gameObject.GetComponent<Button>().interactable = false;
                bm.firstRune.color = ChangeColour(bm.rune1);
            }
            else if ((bm.rune1 != "") && (bm.rune2 == ""))
            {
                bm.rune2 = rune.runeName;
                bm.runeIndex = rune.runeId;
                gameObject.GetComponent<Button>().interactable = false;
                bm.secondRune.color = ChangeColour(bm.rune2);
                bm.ChooseSpell();
                StartCoroutine(bm.SpellChosen());
            }
        }
    }

    private Color ChangeColour(string name)
    {
        Color element = Color.white;

        if (name == "Fire")
        {
            element = new Color32(134, 36, 35, 255);
        }
        else if (name == "Earth")
        {
            element = new Color32(115, 73, 46, 255);
        }
        else if (name == "Water")
        {
            element = new Color32(35, 73, 123, 255);
        }
        else if (name == "Wind")
        {
            element = new Color32(101, 185, 126, 255);
        }

        return element;
    }*/
}
