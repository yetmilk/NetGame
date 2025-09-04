using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiColldown : MonoBehaviour
{
    public TMP_Text colldownText;
    public GameObject colldownObj;

    public Image skillImg;


    public void UpdateSkill(float coolDown)
    {
        colldownText.text = coolDown.ToString("F1");
        colldownObj.SetActive(!(coolDown <= 0));
    }
}
