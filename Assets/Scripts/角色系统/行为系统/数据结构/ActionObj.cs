using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����Ϊ����ʵ���������Ϊ���
/// </summary>
[System.Serializable]
public class ActionObj
{
    public ActionInfo curActionInfo;//��ǰʵ������Ϊ����Ϣ

    public float startPercent;//�����Ϊ��ʼ֡

    public int curLifeFrame;//��ǰ��Ϊ�������ڽ���

    public Vector3 direction;

    //��̬���ȼ�����ǰ��Ϊ�����ȼ�
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
