using System.Collections.Generic;

using System;
using UnityEngine.Events;

/// <summary>
/// 事件中心，用于管理游戏中的所有事件
/// </summary>
public static class EventCenter
{
    #region-------------------事件--------------------
    static Action<object> LogicFrameUpdate;//逻辑帧更新事件
    static Action<object> RenderFrameUpdate;//渲染帧更新事件
    static Action<object> MoveEvent;//移动事件
    static Action<object> RollEvent;//翻滚事件
    static Action<object> OnAttackEvent;//攻击判定事件
    #endregion

    // 使用泛型字典存储不同类型的事件
    private static readonly Dictionary<string, Action<object>> eventDictionary = new Dictionary<string, Action<object>>()
    {
        {EventId.LogicFrameUpdate,LogicFrameUpdate },
        {EventId.RenderFrameUpdate, RenderFrameUpdate},
        {EventId.Move, MoveEvent},
        { EventId.Roll,RollEvent},
        {EventId.OnAttack, OnAttackEvent},
    };

    // 事件ID常量类
    public static class EventId
    {
        public const string LogicFrameUpdate = "LogicFrameUpdate";
        public const string RenderFrameUpdate = "RenderFrameUpdate";
        public const string Move = "Move";
        public const string Roll = "Roll";
        public const string OnAttack = "OnAttack";
        // 可以添加其他事件ID...
    }

    /// <summary>
    /// 订阅事件
    /// </summary>
    public static void Subscribe(string eventName, Action<object> listener)
    {
        if (!eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent = obj => { };
            eventDictionary.Add(eventName, thisEvent);
        }

        thisEvent += listener;
        eventDictionary[eventName] = thisEvent;
    }

    /// <summary>
    /// 取消订阅事件
    /// </summary>
    public static void Unsubscribe(string eventName, Action<object> listener)
    {
        if (eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent -= listener;
            eventDictionary[eventName] = thisEvent;
        }
    }

    /// <summary>
    /// 发布事件
    /// </summary>
    public static void Publish(string eventName, object parameter = null)
    {
        if (eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent?.Invoke(parameter);
        }
    }
}