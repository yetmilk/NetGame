using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public UIHp hpSlider;
    public UISkillCoolDownCtrl skillCtrl;

    public CharacterDataObj data;
    public void Initilize(ref CharacterDataObj dataObj, Transform parent)
    {
        data = dataObj;
        hpSlider.Init(ref dataObj);
        skillCtrl.Init(ref dataObj);
        var go = LoadManager.Instance.GetResourceByName<GameObject>("¾«Á¦Ìõ");
        var obj = Instantiate(go, parent);
        obj.GetComponent<UIEnergyBar>().Init(ref dataObj);
    }
}
