using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatusHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject message;

    public void OnPointerEnter(PointerEventData eventData)
    {
        message.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        message.SetActive(false);
    }
}
