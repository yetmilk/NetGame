using System.Collections.Generic;

using System;
using UnityEngine.Events;

/// <summary>
/// �¼����ģ����ڹ�����Ϸ�е������¼�
/// </summary>
public static class EventCenter
{
    #region-------------------�¼�--------------------
    static Action<object> LogicFrameUpdate;//�߼�֡�����¼�
    static Action<object> RenderFrameUpdate;//��Ⱦ֡�����¼�
    static Action<object> MoveEvent;//�ƶ��¼�
    static Action<object> RollEvent;//�����¼�
    static Action<object> OnAttackEvent;//�����ж��¼�
    #endregion

    // ʹ�÷����ֵ�洢��ͬ���͵��¼�
    private static readonly Dictionary<string, Action<object>> eventDictionary = new Dictionary<string, Action<object>>()
    {
        {EventId.LogicFrameUpdate,LogicFrameUpdate },
        {EventId.RenderFrameUpdate, RenderFrameUpdate},
        {EventId.Move, MoveEvent},
        { EventId.Roll,RollEvent},
        {EventId.OnAttack, OnAttackEvent},
    };

    // �¼�ID������
    public static class EventId
    {
        public const string LogicFrameUpdate = "LogicFrameUpdate";
        public const string RenderFrameUpdate = "RenderFrameUpdate";
        public const string Move = "Move";
        public const string Roll = "Roll";
        public const string OnAttack = "OnAttack";
        // ������������¼�ID...
    }

    /// <summary>
    /// �����¼�
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
    /// ȡ�������¼�
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
    /// �����¼�
    /// </summary>
    public static void Publish(string eventName, object parameter = null)
    {
        if (eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent?.Invoke(parameter);
        }
    }
}