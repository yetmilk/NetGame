using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHp : UIElement
{
    public Slider barSlider;

    public override void Init(ref CharacterDataObj dataObj)
    {
        base.Init(ref dataObj);
    }

    private void Update()
    {
        barSlider.value = dataObj.curHealth / dataObj.maxhealth;
    }
}
