using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerNumberPopup : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private float fadeTimer;
    private Color textColor;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshProUGUI>();
    }

    public void Setup(int damage, string state, bool heal)
    {
        textMesh.SetText(damage.ToString());
        fadeTimer = 0.8f;
        if (state == "normalPlayer")
        {
            textMesh.fontSize = 45;
            if (heal)
            {
                textColor = new Color32(166, 243, 130, 255);
            }
            else
            {
                textColor = new Color32(244, 244, 244, 255);
            }
        }
        else if (state == "criticalPlayer")
        {
            textMesh.fontSize = 55;
            textColor = new Color32(253, 87, 87, 255);
        }
        else if (state == "weakPlayer")
        {
            textMesh.fontSize = 35;
            textColor = new Color32(185, 188, 188, 255);
        }
        textMesh.color = textColor;
    }

    private void Update()
    {
        fadeTimer -= Time.deltaTime;
        if (fadeTimer < 0)
        {
            float fadeSpeed = 30f;
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
