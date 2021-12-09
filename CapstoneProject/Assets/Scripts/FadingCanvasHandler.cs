using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingCanvasHandler : MonoBehaviour
{
    public GameObject fadeCanvas;

    private void Start()
    {
        CallFadeIn();
    }

    public void CallFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        fadeCanvas.SetActive(true);
        yield return new WaitForSeconds(1.8f);
        fadeCanvas.SetActive(false);
    }
}
