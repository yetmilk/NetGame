
using System.Collections.Generic;
using System;
using UnityEngine;

// �������� - �ɷֽ�Ϊ���������
[CreateAssetMenu(fileName = "CompoundTask", menuName = "HTN/Task/CompoundTask")]
public class CompoundTask : Task
{
    public WorldState _state;
    public override bool IsPrimitive => false;

    // �����ڱ༭�������õķֽⷽ���б�
    [SerializeField]
    private List<MethodGroup> decompositionMethodReferences = new List<MethodGroup>();

    // ����ʱʹ�õ�ʵ�ʷֽⷽ��
    [System.NonSerialized]
    private Func<WorldState, List<Task>>[] runtimeDecompositionMethods;

    public void OnEnable()
    {
        // ��ʼ������ʱ��ί��
        UpdateRuntimeMethods();
    }

    // ��ʼ������ʱ����
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

    // ��ȡ�ֽⷽ��
    public Func<WorldState, List<Task>>[] GetDecompositionMethods()
    {
        if (runtimeDecompositionMethods == null || runtimeDecompositionMethods.Length == 0)
        {
            UpdateRuntimeMethods();
        }

        return runtimeDecompositionMethods;
    }
}

// ���� ConditionTaskPair �࣬���ڴ洢 condition �� task �����
[System.Serializable]
public class ConditionTaskPair
{
    public Condition condition;
    public List<Task> tasks;
}