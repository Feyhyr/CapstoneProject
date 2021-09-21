using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rune")]
public class RuneSO : ScriptableObject
{
    public string runeName;
    public Sprite icon;
    public AudioClip audioSFX;
}
