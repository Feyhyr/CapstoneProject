using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyNumberPopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float fadeTimer;
    private Color textColor;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(int damage, string state, bool heal)
    {
        textMesh.SetText(damage.ToString());
        fadeTimer = 0.8f;
        if (state == "normalEnemy")
        {
            textMesh.fontSize = 6;
            if (heal)
            {
                textColor = new Color32(166, 243, 130, 255);
            }
            else
            {
                textColor = new Color32(244, 244, 244, 255);
            }
        }
        else if (state == "criticalEnemy")
        {
            textMesh.fontSize = 10;
            textColor = new Color32(253, 87, 87, 255);
        }
        else if (state == "weakEnemy")
        {
            textMesh.fontSize = 4;
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
