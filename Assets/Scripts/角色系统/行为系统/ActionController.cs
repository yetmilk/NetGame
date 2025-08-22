using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



public class ActionController : MonoBehaviour
{
    public ActionObj curActionObj; // 当前激活的行为

    public List<ActionInfo> allActionInfo = new List<ActionInfo>(); // 此控制器管理的所有行为


    private CharacterBehaviourController owner;

    Dictionary<ActionTag, List<SubmitFuncParam>> submitActionDic = new Dictionary<ActionTag, List<SubmitFuncParam>>(); // 存储条件转换行为的字典
    // 行为切换的两个队列
    private List<ActionObj> actionAppointmentQueue = new List<ActionObj>(); // 预约队列(可执行的行为)

    private Animator animator;

    public Action<ActionTag, ActionObj> OnActionEnter;
    public Action<ActionTag, ActionObj> OnActionUpdate;

    public Action<ActionTag, ActionObj, ActionObj> OnActionExit;
    #region-----------------------------初始化--------------------------

    private void Awake()
    {
        animator = GetComponent<Animator>();

    }
    public void InitializeAction(CharacterClacify character, CharacterBehaviourController owner)
    {

        this.owner = owner;
        //加载资源
        LoadActionData(LoadManager.Instance.GetResourceByName<ActionCollection>(character.ToString()));




        // 订阅事件
        EventCenter.Subscribe(EventCenter.EventId.LogicFrameUpdate, OnLogicFrameUpdate);
        EventCenter.Subscribe(EventCenter.EventId.RenderFrameUpdate, OnRenderFrameUpdate);
    }

    public void InitializeAction(ActionCollection actionCollection, CharacterBehaviourController owner)
    {
        this.owner = owner;
        LoadActionData(actionCollection);

        // 订阅事件
        EventCenter.Subscribe(EventCenter.EventId.LogicFrameUpdate, OnLogicFrameUpdate);
        EventCenter.Subscribe(EventCenter.EventId.RenderFrameUpdate, OnRenderFrameUpdate);
    }

    private void LoadActionData(ActionCollection actionInfos)
    {
        allActionInfo.Clear();


        foreach (var actionInfo in actionInfos.ActionInfos)
        {
            allActionInfo.Add(actionInfo);
            if (!submitActionDic.ContainsKey(actionInfo.tag))
            {
                submitActionDic.Add(actionInfo.tag, new List<SubmitFuncParam>());
            }
        }

        if (allActionInfo.Count > 0)
        {
            AddToAppointmentQueue(allActionInfo[0].id, 0f);
        }
    }
    #endregion
    #region---------------------帧事件----------------------------
    private void OnLogicFrameUpdate(object curFrame)
    {
        ProcessSubmitQueue(); // 处理候补队列
        UpdateCurAction();     // 更新当前行为
    }

    private void OnRenderFrameUpdate(object curFrame)
    {
    }
    #endregion

    private void UpdateAnimationProgress(float normalizedProgress)
    {
        string animationName = curActionObj.curActionInfo.animationName.name;
        int animationHash = Animator.StringToHash(animationName);
        animator.Play(animationHash, 0, normalizedProgress);
    }

    private void UpdateCurAction()
    {
        if (curActionObj == null) return;

        UpdateActionLife();

        CheckEventFromEveryFrame();

        ProcessAppointmentQueue(); // 处理预约队列
    }

    private void UpdateActionLife()
    {


        if (curActionObj.curLifeFrame > curActionObj.curActionInfo.actionLifeRange.y && curActionObj.curActionInfo.actionLifeRange.y != -1)
        {

            AddToAppointmentQueue(curActionObj.curActionInfo.nextActionIds, 0);
        }
        curActionObj.curLifeFrame++;
    }

    private void CheckEventFromEveryFrame()
    {
        OnActionUpdate?.Invoke(curActionObj.curActionInfo.tag, curActionObj);
        foreach (var eventData in curActionObj.frameIndexEvents)
        {
            if (curActionObj.curLifeFrame >= eventData.eventLifeRange.x &&
                (curActionObj.curLifeFrame <= eventData.eventLifeRange.y || eventData.eventLifeRange.y == -1))
            {
                // Debug.Log(111);
                //owner.eventCenter.Publish(eventData.actionName, curActionObj, eventData.GetEventParam());
                ActionLogicCollection.Execute(eventData.actionName, owner, eventData.GetEventParam());
            }
        }
        if (curActionObj.curActionInfo.audioInfos == null) return;
        foreach (var audioInfo in curActionObj.curActionInfo.audioInfos)
        {
            if (audioInfo.audioLifeRange.x == curActionObj.curLifeFrame)
            {
                owner.audioSource.clip = audioInfo.audioClip;
                owner.audioSource.Play();
            }

        }
    }

    #region----------------------------行为打断处理------------------------------------

    // 处理单个指令
    private bool ProcessCommand(ActionCommand command)
    {

        CanBeCancelTag cancelInfo;
        // 检查是否可以打断当前行为
        if (CanCancelCurrentAction(command.commandType, out cancelInfo))
        {
            //查找合适的行为
            ActionInfo targetAction = null;
            if (submitActionDic.ContainsKey(command.commandType) && submitActionDic[command.commandType].Count != 0)//如果候补队列有匹配的行为，优先处理候补队列
            {
                //Debug.Log(submitActionDic[cancelInfo.canCancelTag][0].submitTag);
                if (!submitActionDic[command.commandType][0].cantTranslate)
                {
                    targetAction = allActionInfo.Find(info => (info.id == submitActionDic[command.commandType][0].actionId));

                    cancelInfo.nextActionStartPercent = submitActionDic[command.commandType][0].delay;

                    submitActionDic[command.commandType].RemoveAt(0);
                }

            }
            else
            {
                targetAction = allActionInfo.Find(info =>
                {
                    if (info.id == command.actionId)
                    {
                        //Debug.Log(info.id + "   " + command.actionId);
                        return true;
                    }

                    if (String.IsNullOrEmpty(command.actionId))
                        if (info.tag == command.commandType) return true;

                    return false;
                }
                );
                //Debug.Log(command.diraction);

            }


            if (targetAction == null)
            {
                //Debug.LogWarning($"找不到行为: {command.actionId}+{command.commandType} + {command.diraction}");
                return false;
            }

            //Debug.Log("id:  " + targetAction.id + "Dir: " + command.diraction);
            //初始化行为
            ActionObj newAction = new ActionObj(targetAction
                , command.direction, cancelInfo.nextActionStartPercent);

            //Debug.Log("原的优先级为 ：  " + newAction.curPriority);
            newAction.curPriority = curActionObj.curActionInfo.canCancelTags.ToList().Find(a => a.canCancelTag == newAction.curActionInfo.tag).proprity;
            //Debug.Log("新的优先级为 ：  " + newAction.curPriority);

            AddToAppointmentQueue(newAction);

            return true;
        }
        return false;
    }

    // 检查是否可以打断当前行为
    private bool CanCancelCurrentAction(ActionTag nextActionTag, out CanBeCancelTag cancelInfo)
    {
        foreach (var cancelTag in curActionObj.curActionInfo.canCancelTags)
        {
            if (cancelTag.canCancelTag == nextActionTag)
            {
                foreach (var range in cancelTag.canBeCancelRange)
                {
                    if (curActionObj.curLifeFrame >= range.x && (curActionObj.curLifeFrame <= range.y || range.y == -1))
                    {
                        cancelInfo = cancelTag;
                        return true; // 在可打断范围内
                    }
                }
            }
        }
        cancelInfo = null;
        return false;
    }
    #endregion

    #region-------------------------------预约队列处理------------------------------------
    // 添加到预约队列并排序
    private void AddToAppointmentQueue(ActionObj action)
    {
        actionAppointmentQueue.Add(action);

        // 按优先级排序(降序)
        actionAppointmentQueue = actionAppointmentQueue
            .OrderByDescending(a => a.curPriority)
            .ToList();
    }
    private void AddToAppointmentQueue(string actionNmae, float startPercent)
    {
        ActionInfo target = allActionInfo.Find(info => info.id == actionNmae);
        if (target == null) return;
        ActionObj action = new ActionObj(target, curActionObj.direction, startPercent);
        actionAppointmentQueue.Add(action);

        // 按优先级排序(降序)
        actionAppointmentQueue = actionAppointmentQueue
            .OrderByDescending(a => a.curActionInfo.priority)
            .ToList();
    }

    // 处理预约队列
    private void ProcessAppointmentQueue()
    {
        if (actionAppointmentQueue.Count == 0) return;

        // 获取优先级最高的行为
        ActionObj nextAction = actionAppointmentQueue[0];

        if (nextAction.curActionInfo.id == curActionObj.curActionInfo.id)
        {
            KeepAction(nextAction);
        }
        else
        {
            // 执行行为切换
            ChangeAction(nextAction);
        }

        // 清空预约队列
        actionAppointmentQueue.Clear();
    }
    #endregion

    #region--------------------处理条件候选队列-------------------------------
    private void ProcessSubmitQueue()
    {
        // 处理候选字典中的行为
        foreach (var kvp in submitActionDic)
        {
            ActionTag tag = kvp.Key;
            List<SubmitFuncParam> submitEvents = kvp.Value;

            if (submitEvents.Count == 0) continue;

            List<SubmitFuncParam> removeEvent = new List<SubmitFuncParam>();

            foreach (var submitEvent in submitEvents)
            {

                if (submitEvent.waitFrame <= 0)
                {
                    removeEvent.Add(submitEvent);
                }
                else
                {
                    submitEvent.waitFrame--;
                }
            }

            foreach (var submitEvent in removeEvent)
            {
                submitEvents.Remove(submitEvent);
            }
        }
    }


    #endregion

    private void ChangeAction(ActionObj action)
    {
        OnActionExit?.Invoke(curActionObj.curActionInfo.tag, curActionObj, action);
        // 保存新的动作对象
        curActionObj = action;

        //Debug.Log(curActionObj.curActionInfo.animationName.name);

        // 使用当前动画进度启动新动画的CrossFade
        animator.CrossFade(
            curActionObj.curActionInfo.animationName.name,
            0, // 过渡持续时间
            0, // 动画层
           action.startPercent // 使用上一个动画的进度作为新动画的起始点
        );

        OnActionEnter?.Invoke(action.curActionInfo.tag, action);
        // Debug.Log(action.startPercent);
    }

    private void KeepAction(ActionObj action)
    {
        //curCompleteAction?.Invoke();
        // 保存新的动作对象
        //int curFrame = curActionObj.curLifeFrame;
        //Debug.Log(curActionObj.actionDiraction + "新" + action.actionDiraction);
        curActionObj = action;
        //curActionObj.curLifeFrame = curFrame;
    }

    #region-----------------------------------外部调用------------------------------
    // 外部接口：添加指令到预处理队列
    public bool AddCommand(string actionId, ActionTag tag, Vector3 direction)
    {
        ActionCommand command = new ActionCommand(actionId, tag, direction);
        //commandQueue.Enqueue(command);
        return ProcessCommand(command);
    }

    public void AddSubmitAction(SubmitFuncParam submitParam)
    {
        submitActionDic[submitParam.submitTag].Add(new SubmitFuncParam(submitParam));
    }
    public void DisableCurAction()
    {
        AddToAppointmentQueue(curActionObj.curActionInfo.nextActionIds, 0);
    }

    public void AddActionEnterListener(Action<ActionTag, ActionObj> action)
    {
        OnActionEnter += action;
    }
    public void RemoveActionEnterListener(Action<ActionTag, ActionObj> action)
    {
        OnActionEnter -= action;
    }
    public void AddActionUpdateListener(Action<ActionTag, ActionObj> action)
    {
        OnActionUpdate += action;
    }
    public void RemoveActionUpdateListener(Action<ActionTag, ActionObj> action)
    {
        OnActionUpdate -= action;
    }
    public void AddActionExitListener(Action<ActionTag, ActionObj, ActionObj> action)
    {
        OnActionExit += action;
    }
    public void RemoveActionExitListener(Action<ActionTag, ActionObj, ActionObj> action)
    {
        OnActionExit -= action;
    }
    #endregion
    private void OnDestroy()
    {
        EventCenter.Unsubscribe(EventCenter.EventId.LogicFrameUpdate, OnLogicFrameUpdate);
    }
}