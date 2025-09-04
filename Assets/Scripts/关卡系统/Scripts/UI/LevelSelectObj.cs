using Map;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectObj : MonoBehaviour
{
    public TMP_Text text1;
    public TMP_Text text2;
    public TMP_Text text3;
    public Button selectBtn;

    public GameObject selectObj;

    public int index;

    public void Init(LevelSelectController controller,int index, string text1, string text2 = "", string text3 = "")
    {
        this.index = index;
        this.text1.text = text1;
        this.text2.text = text2;
        this.text3.text = text3;

        selectBtn.onClick.AddListener(() =>
        {
            controller.SetIndex(index);
        });
    }


}
