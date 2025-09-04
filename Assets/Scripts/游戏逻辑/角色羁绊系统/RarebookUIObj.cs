using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RarebookUIObj : MonoBehaviour
{
    public Image sprite;

    public TMP_Text rarebookName;

    public TMP_Text rarebookDescription;


    public void Init(string name, string description)
    {
        rarebookName.text = name;
        rarebookDescription.text = description;
    }

}
