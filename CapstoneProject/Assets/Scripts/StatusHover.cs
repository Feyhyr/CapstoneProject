using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusHover : MonoBehaviour
{
    public GameObject message;

    private void OnMouseEnter()
    {
        message.SetActive(true);
    }

    private void OnMouseExit()
    {
        message.SetActive(false);
    }
}
