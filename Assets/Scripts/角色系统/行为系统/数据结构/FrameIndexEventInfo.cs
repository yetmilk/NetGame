using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ActionLogicCollection;
[System.Serializable]
public class FrameIndexEvent
{
    public Vector2 eventLifeRange;

    [Header("逻辑对应函数名")]
    public string actionName;

    public MoveFunctionParam MoveFunctionParam;
    public SubmitFuncParam SubmitFuncParam;
    public RotateFuncParam RotateFuncParam;
    public OnInstantiateObjFunctionParam OnInstantiateObjFunctionParam;
    public RigidbodyParam RigidbodyParam;

    public FrameIndexEvent(Vector2 eventLifeRange, string eventName, FunctionParam param)
    {
        this.actionName = eventName;
        this.eventLifeRange = eventLifeRange;
        // 根据事件名类型，将通用参数转换为具体类型并赋值
        switch (eventName)
        {
            case FunctionId.Move:
                MoveFunctionParam = param as MoveFunctionParam;
                break;
            case FunctionId.SubmitActionChangeQuest:
                SubmitFuncParam = param as SubmitFuncParam;
                break;
            case FunctionId.Rotate:
                RotateFuncParam = param as RotateFuncParam;
                break;
            case FunctionId.InstantiateObj:
                OnInstantiateObjFunctionParam = param as OnInstantiateObjFunctionParam;
                break;
            case FunctionId.SetRigidBody:
                RigidbodyParam = param as RigidbodyParam;
                break;
            default:
                Debug.LogWarning($"未识别的事件名: {eventName}");
                break;
        }
    }

    // 返回当前事件对应的参数对象
    public FunctionParam GetEventParam()
    {
        switch (actionName)
        {
            case FunctionId.Move:
                return MoveFunctionParam;
            case FunctionId.SubmitActionChangeQuest:
                return SubmitFuncParam;
            case FunctionId.Rotate:
                return RotateFuncParam;
            case FunctionId.InstantiateObj:
                return OnInstantiateObjFunctionParam;
            case FunctionId.SetRigidBody:
                return RigidbodyParam;
            default:
                return null;

        }
    }
}
// 标记所有事件参数类，方便反射查找
public class FunctionParamAttribute : System.Attribute
{
    public string FunctionName { get; private set; }

    public FunctionParamAttribute(string eventName)
    {
        FunctionName = eventName;
    }
}

[System.Serializable]
public class FunctionParam
{

}
// 移动事件参数
[System.Serializable]
[FunctionParam(FunctionId.Move)]
public class MoveFunctionParam : FunctionParam
{
    [Header("是否使用rigidBody移动（true为使用，false为使用transform移动）")]
    public bool useRig;
    [Header("填写的方向向量使用什么坐标空间（勾选为目标自身局部坐标空间，未勾选表示使用世界坐标空间）")]
    public bool useLocalSpace = false;
    [Space(20)]
    [Header("输入配置")]
    [Tooltip("使用角色面朝方向")]
    public bool useActionDir = true;
    [Tooltip("如果前面未勾选，应用此方向")]
    public Vector3 moveDir = Vector3.zero;

    [Space(10)]
    [Header("移动数据配置")]
    [Tooltip("是否使用行为绑定角色的速度数据")]
    public bool useDataSpeed = false;
    public CharaDataEnum dataName;
    [Tooltip("移动速度")]
    public float speed;
    [Header("地面层")]
    public LayerMask layerMask;

    [Header("未勾选useRig时，为其赋值")]
    [HideInInspector] public Transform moveTransform;
    [Header("勾选useRig时，为其赋值")]
    [HideInInspector] public Rigidbody moveRig;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="useRig">是否使用rigidBody移动</param>
    /// <param name="usefaceDir">填写的方向向量使用什么坐标空间</param>
    /// <param name="useActionDir">移动方向向量</param>
    /// <param name="speed">移动速度</param>
    /// <param name="moveTransform">移动目标的Transform组件</param>
    /// <param name="moveRig">移动目标的rigidbody组件</param>
    public MoveFunctionParam(bool useRig, bool usefaceDir, bool useActionDir, float speed, Transform moveTransform = null, Rigidbody moveRig = null)
    {
        this.useRig = useRig;
        this.useLocalSpace = usefaceDir;
        this.useActionDir = useActionDir;
        this.speed = speed;
        this.moveTransform = moveTransform;
        this.moveRig = moveRig;
    }
}

// 生成生成物的事件
[System.Serializable]
[FunctionParam(FunctionId.InstantiateObj)]
public class OnInstantiateObjFunctionParam : FunctionParam
{
    public string vfxName;
    public Vector3 startDirection;
    public float distance;
    [Header("特效生成的父物体是否为自身（false则为无父物体）")]
    public bool selfAsparent;

    [Header("使用鼠标位置生成")]
    public bool useMousePosition;

    [Header("生成位置(世界坐标)")]
    public Vector3 instantiatePos = Vector3.zero;

    [Header("特效生命周期")]
    public float VfxLifeTime = -1f;

    [Header("是否跟随行为结束而销毁")]
    public bool destroyByActionEnd = true;
}
[System.Serializable]
[FunctionParam(FunctionId.SubmitActionChangeQuest)]
public class SubmitFuncParam : FunctionParam
{
    [Header("提交转换行为的类型")]
    public ActionTag submitTag;

    [Header("转换行为的id")]
    public string actionId;

    [Header("禁止转换还是允许转换")]
    public bool cantTranslate;

    [Header("该行为转换等待帧（60帧/秒）")]
    public float waitFrame;

    [Header("转换到下一个行为的进度")]
    [Range(0, 1)]
    public float delay;


    public SubmitFuncParam()
    {

    }
    public SubmitFuncParam(SubmitFuncParam submitEventParam)
    {
        this.submitTag = submitEventParam.submitTag;
        this.actionId = submitEventParam.actionId;
        this.waitFrame = submitEventParam.waitFrame;
        this.cantTranslate = submitEventParam.cantTranslate;
        this.delay = submitEventParam.delay;
    }
}
[System.Serializable]
[FunctionParam(FunctionId.Rotate)]
public class RotateFuncParam : FunctionParam
{


    [Header("使用行为方向进行旋转")]
    public bool useActionDir = true;
    [Header("如果上述选项都未勾选，应用此项")]
    public Vector3 rotateDir;

    [Header("开启插值旋转（无法一瞬间旋转到目标方向）")]
    public bool useSmooth = true;
    [Header("开启插值旋转的旋转速度")]
    public float rotationSpeed = 1500f;


}


