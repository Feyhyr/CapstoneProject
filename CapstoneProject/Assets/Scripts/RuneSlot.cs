using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RuneSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RuneController>().droppedOnSlot = true;
            eventData.pointerDrag.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
            if (gameObject.tag == "FirstSlot")
            {
                eventData.pointerDrag.GetComponent<RuneController>().SelectFirstRune();
            }
            else
            {
                eventData.pointerDrag.GetComponent<RuneController>().SelectSecondRune();
            }
        }
    }
}
