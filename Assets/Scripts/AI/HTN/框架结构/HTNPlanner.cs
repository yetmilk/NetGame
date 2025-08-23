// HTN规划器
using System.Collections.Generic;
using UnityEngine;

public class Planner
{
    public Plan CreatePlan(WorldState worldState, List<Task> goals)
    {

        var plan = new Plan();
        var taskStack = new Stack<Task>();

        for (int i = 0; i < goals.Count; i++)
        {
            taskStack.Push(goals[i]);
            
            var state = worldState.Clone();//当前进行任务推导使用的克隆世界状态
            while (taskStack.Count > 0)
            {
                var task = taskStack.Pop();

                if (task.IsPrimitive)
                {
                    if (!task.ValidatePreconditions(state))
                    {
                        continue;
                    }
                    var primitiveTask = task as PrimitiveTask;
                    plan.primitiveTasks.Add(primitiveTask);
                    state.ApplyEffect(primitiveTask.effect,true);

                }
                else
                {
                    var compoundTask = task as CompoundTask;
                    List<Task> subTasks = new List<Task>();
                    for (int j = 0; j < compoundTask.GetDecompositionMethods().Length; j++)
                    {
                        var testState = worldState.Clone();
                        subTasks = compoundTask.GetDecompositionMethods()[j](testState);
                        if (subTasks == null || subTasks.Count == 0)
                        {
                            continue;
                        }
                        for (int k = subTasks.Count - 1; k >= 0; k--)
                        {
                            taskStack.Push(subTasks[k]);
                        }
                        break;

                    }
                }
            }
            if (plan.primitiveTasks.Count > 0)
            {
                return plan;
            }

        }
        return plan;
    }
}