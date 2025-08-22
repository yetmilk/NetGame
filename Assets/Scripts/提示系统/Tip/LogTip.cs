using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogTip : MonoBehaviour, ITip
{
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        
    }
}
