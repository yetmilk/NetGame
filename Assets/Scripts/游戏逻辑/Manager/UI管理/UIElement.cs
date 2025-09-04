using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElement : MonoBehaviour
{
    public CharacterDataObj dataObj;

    public virtual void Init(ref CharacterDataObj dataObj)
    {
        this.dataObj = dataObj;
    }
}
