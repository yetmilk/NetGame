using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 将行为数据实例化后的行为物件
/// </summary>
[System.Serializable]
public class ActionObj
{
    public ActionInfo curActionInfo;//当前实例化行为的信息

    public float startPercent;//这个行为起始帧

    public int curLifeFrame;//当前行为生命周期进度

    public Vector3 direction;

    //动态优先级，当前行为的优先级
    public int curPriority;

    [HideInInspector] public List<FrameIndexEvent> frameIndexEvents;


    public ActionObj(ActionInfo actionInfo, Vector3 direction = default, float startPercent = 0f, List<FrameIndexEvent> frameIndexEvents = null)
    {
        this.direction = direction;

        this.curActionInfo = new ActionInfo(actionInfo);

        this.startPercent = startPercent;

        this.curLifeFrame = Mathf.RoundToInt(curActionInfo.actionLifeRange.magnitude * startPercent);

        this.frameIndexEvents = new List<FrameIndexEvent>();
        foreach (FrameIndexEvent e in curActionInfo.frameIndexEvent)
        {
            this.frameIndexEvents.Add(e);
        }
        if (frameIndexEvents != null)
            foreach (FrameIndexEvent e in frameIndexEvents)
            {
                this.frameIndexEvents.Add(e);
            }
        this.curPriority = curActionInfo.priority;
    }

    public FrameIndexEvent GetLogicByFuncId(string eventId)
    {
        FrameIndexEvent target = frameIndexEvents.Find((item) =>
       {
           return item.actionName == eventId;
       });

        return target;
    }

    public void AddFunc(FrameIndexEvent indexEvent)
    {
        if (!frameIndexEvents.Contains(indexEvent))
            frameIndexEvents.Add(indexEvent);
    }
}
