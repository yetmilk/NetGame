using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SyncTransform))]
public class InputBase : MonoBehaviour
{
    public IDealActionCommand commandCtrl;

    public virtual void Init()
    {
        commandCtrl = GetComponent<IDealActionCommand>();

    }

    public virtual void Enable()
    {

    }

    public virtual void Disable()
    {

    }



}
