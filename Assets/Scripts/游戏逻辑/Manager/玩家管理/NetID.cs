using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetID : MonoBehaviour
{
    public string id;
    public string flag;

    public void Init(string id, string flag)
    {
        this.id = id;
        this.flag = flag;
    }

    public bool isLocal()
    {
        return flag == "Local";
    }
}
