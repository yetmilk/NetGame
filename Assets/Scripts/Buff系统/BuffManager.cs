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
    //buff组件列表
    public List<BuffController> controllerList = new List<BuffController>();

    /// <summary>
    /// 注册一个buffController
    /// </summary>
    /// <param name="controller"></param>
    public void LogOnBuffController(BuffController controller)
    {
        if (!controllerList.Contains(controller))
            controllerList.Add(controller);
    }

    /// <summary>
    /// 注销一个buffController
    /// </summary>
    /// <param name="controller"></param>
    public void LogOffBuffController(BuffController controller)
    {
        if (controllerList.Contains(controller))
            controllerList.Remove(controller);
    }
}
