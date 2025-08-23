// 任务基类
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Task : ScriptableObject
{
    public string taskName;
    public virtual bool IsPrimitive { get; }
    public Condition precondition;

    public bool ValidatePreconditions(WorldState state)
        => precondition == null || precondition.Evaluate(state);
}
