using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RuneController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler
{
    public RuneSO rune;
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
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor / 0.5111522f;
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
            bm.isAudioPlaying = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canvasGroup.interactable)
        {
            AudioManager.Instance.Play(rune.audioSFX);
            rectTransform.SetAsLastSibling();
        }
    }
}
