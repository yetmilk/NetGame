using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionInfo
{
    [Header("行为id")]
    public string id;//行为名称

    [Header("自然过度的下一个行为id")]
    public string nextActionIds;//自然过度的下一个行为

    [Header("对应动画")]
    public AnimationClip animationName;//行为对应动画名称

    [Header("对应音效")]
    public ActionAudioInfo[] audioInfos;//行为对应音效组信息

    [Header("该行为帧范围")]
    public Vector2 actionLifeRange;//行为有效的帧范围

    [Header("行为触发的事件组")]
    public FrameIndexEvent[] frameIndexEvent;//行为触发的事件组

    [Header("行为的标签")]
    public ActionTag tag;//行为的标签

    [Header("可以取消该行为的标签组")]
    public CanBeCancelTag[] canCancelTags;//可以取消该行为的标签组

    [Header("该行为切换优先级")]
    public int priority;//该行为切换优先级

    public ActionInfo(ActionInfo actionInfo)
    {
        id = actionInfo.id;
        nextActionIds = actionInfo.nextActionIds;
        animationName = actionInfo.animationName;
        tag = actionInfo.tag;
        actionLifeRange = actionInfo.actionLifeRange;
        frameIndexEvent = actionInfo.frameIndexEvent;
        canCancelTags = actionInfo.canCancelTags;
        priority = actionInfo.priority;
        audioInfos = actionInfo.audioInfos;
    }


}
