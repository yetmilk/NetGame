using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public static BuffManager instance;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }
    }
    //buff����б�
    public List<BuffController> controllerList = new List<BuffController>();

    /// <summary>
    /// ע��һ��buffController
    /// </summary>
    /// <param name="controller"></param>
    public void LogOnBuffController(BuffController controller)
    {
        if (!controllerList.Contains(controller))
            controllerList.Add(controller);
    }

    /// <summary>
    /// ע��һ��buffController
    /// </summary>
    /// <param name="controller"></param>
    public void LogOffBuffController(BuffController controller)
    {
        if (controllerList.Contains(controller))
            controllerList.Remove(controller);
    }
}
