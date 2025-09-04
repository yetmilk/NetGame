// 规划结果
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Plan
{
    public List<PrimitiveTask> primitiveTasks = new List<PrimitiveTask>();
    public bool IsValid => primitiveTasks.Count > 0;
    public IEnumerator Execute(Agent agent)
    {
        Queue<PrimitiveTask> tasks = new Queue<PrimitiveTask>(primitiveTasks);

        bool isExecuting = false;
        while (tasks.Count > 0)
        {
            PrimitiveTask task = tasks.Dequeue();
            if (!isExecuting)
            {
                if (task != null)
                {
                    isExecuting = true;
                }
                agent.currentTask = task;
                Debug.Log("当前执行的任务： " + agent.currentTask);
                task.GetExecuteAction()?.Invoke(agent, () =>
                {
                    isExecuting = false;
                    agent.state.ApplyEffect(task.effects);
                });
            }
            while (isExecuting)
            {
                yield return null;
            }

        }
        agent.currentTask = null;
    }

    public bool IsEqual(Plan comparePlan)
    {

        if (comparePlan.primitiveTasks.Count != primitiveTasks.Count) return false;
        for (int i = 0; i < comparePlan.primitiveTasks.Count; i++)
        {
            if (primitiveTasks[i].taskName != comparePlan.primitiveTasks[i].taskName)
            {
                return false;
            }
        }
        return true;
    }

}