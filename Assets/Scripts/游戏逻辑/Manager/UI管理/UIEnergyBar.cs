using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnergyBar : UIElement
{
    public Slider energySlider;

    private void Update()
    {
        energySlider.value = dataObj.curEnergyValue/dataObj.energyValue;
    }
}
