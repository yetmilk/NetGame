using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHp : UIElement
{
    public Image fillImg;
    public Slider Slider;

    public override void Init(ref CharacterDataObj dataObj)
    {
        base.Init(ref dataObj);
    }

    private void Update()
    {
        if (dataObj == null) return;
        if (fillImg != null)
            fillImg.fillAmount = dataObj.curHealth / dataObj.maxhealth;
        if (Slider != null)
            Slider.value = dataObj.curHealth / dataObj.maxhealth;
    }
}
