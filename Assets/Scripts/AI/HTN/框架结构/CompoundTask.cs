
using System.Collections.Generic;
using System;
using UnityEngine;

// 复合任务 - 可分解为多个子任务
[CreateAssetMenu(fileName = "CompoundTask", menuName = "HTN/Task/CompoundTask")]
public class CompoundTask : Task
{
    public WorldState _state;
    public override bool IsPrimitive => false;

    // 用于在编辑器中配置的分解方法列表
    [SerializeField]
    private List<MethodGroup> decompositionMethodReferences = new List<MethodGroup>();

    // 运行时使用的实际分解方法
    [System.NonSerialized]
    private Func<WorldState, List<Task>>[] runtimeDecompositionMethods;

    public void OnEnable()
    {
        // 初始化运行时的委托
        UpdateRuntimeMethods();
    }

    // 初始化运行时方法
    public void UpdateRuntimeMethods()
    {
        List<Func<WorldState, List<Task>>> methods = new List<Func<WorldState, List<Task>>>();

        foreach (var methodReferences in decompositionMethodReferences)
        {
            methods.Add((worldState) =>
            {
                List<Task> requireTask = new List<Task>();
                foreach (var pair in methodReferences.pairs)
                {
                    if (pair.condition.Evaluate(worldState))
                    {
                        for (int i = 0; i < pair.tasks.Count; i++)
                        {
                            requireTask.Add(pair.tasks[i]);
                            if (pair.tasks[i] is PrimitiveTask primitiveTask)
                            {
                                worldState.ApplyEffect(primitiveTask.effect);
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                return requireTask;
            });
        }

        runtimeDecompositionMethods = methods.ToArray();
    }

    // 获取分解方法
    public Func<WorldState, List<Task>>[] GetDecompositionMethods()
    {
        if (runtimeDecompositionMethods == null || runtimeDecompositionMethods.Length == 0)
        {
            UpdateRuntimeMethods();
        }

        return runtimeDecompositionMethods;
    }
}

// 定义 ConditionTaskPair 类，用于存储 condition 和 task 的组合
[System.Serializable]
public class ConditionTaskPair
{
    public Condition condition;
    public List<Task> tasks;
}