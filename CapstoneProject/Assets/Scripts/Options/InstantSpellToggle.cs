using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstantSpellToggle : MonoBehaviour
{
    ToggleSettings toggleSetting;
    Toggle toggle;
    

    private void Start()
    {
        toggleSetting = GameObject.Find("ToggleSetting").GetComponent<ToggleSettings>();
        toggle = this.GetComponent<Toggle>();
        toggle.isOn = toggleSetting.instantSpell;
    }

    public void SpellCastToggle(bool value)
    {
        toggleSetting.SetInstantSpellKey(value);
    }
}
