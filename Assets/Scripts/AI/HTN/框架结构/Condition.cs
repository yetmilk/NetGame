using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Condition
{
    public LogicOperator logicOperator;
    public List<ConditionItem> conditions = new List<ConditionItem>();

    public enum LogicOperator
    {
        ����,
        ����
    }

    public bool Evaluate(WorldState worldState)
    {
        if (worldState == null)
            return false;
        if (conditions.Count == 0)
            return true;

        bool result = conditions[0].Evaluate(worldState);

        for (int i = 1; i < conditions.Count; i++)
        {
            bool current = conditions[i].Evaluate(worldState);
            if (logicOperator == LogicOperator.����)
            {
                result = result && current;
                if (!result) break; // ��·�Ż�
            }
            else
            {
                result = result || current;
                if (result) break; // ��·�Ż�
            }
        }

        return result;
    }
}

[System.Serializable]
public class ConditionItem
{
    public string variableName;
    public string compareValue;
    public ComparisonOperator comparisonOperator;

    public enum ComparisonOperator
    {
        ���,
        �����,
        ����,
        С��,
        ���ڵ���,
        С�ڵ���
    }

    public bool Evaluate(WorldState worldState)
    {
        if (worldState == null || string.IsNullOrEmpty(variableName))
            return false;

        WorldState.Value value = worldState.GetValue(variableName);
        if (value == null || value.value == null)
            return false;

        // �� compareValue ת��Ϊ�� variableName ��Ӧֵ��ͬ������
        object convertedCompareValue = ConvertToSameType(value.value, compareValue);

        // ����ת���ͱȽ��߼�
        switch (comparisonOperator)
        {
            case ComparisonOperator.���:
                return CompareValues(value.value, convertedCompareValue);
            case ComparisonOperator.�����:
                return !CompareValues(value.value, convertedCompareValue);
            case ComparisonOperator.����:
                return CompareNumeric(value.value, convertedCompareValue) > 0;
            case ComparisonOperator.С��:
                return CompareNumeric(value.value, convertedCompareValue) < 0;
            case ComparisonOperator.���ڵ���:
                return CompareNumeric(value.value, convertedCompareValue) >= 0;
            case ComparisonOperator.С�ڵ���:
                return CompareNumeric(value.value, convertedCompareValue) <= 0;
            default:
                return false;
        }
    }

    private object ConvertToSameType(object target, string input)
    {
        if (target is float)
        {
            if (float.TryParse(input, out float result))
            {
                return result;
            }
        }
        else if (target is int)
        {
            if (int.TryParse(input, out int result))
            {
                return result;
            }
        }
        else if (target is bool)
        {
            if (bool.TryParse(input, out bool result))
            {
                return result;
            }
        }
        return input; // ����޷�ת��������ԭʼ�� string ֵ
    }

    private bool CompareValues(object a, object b)
    {
        if (a == null || b == null)
            return a == b;

        if (a.GetType() != b.GetType())
            return false;

        return a.Equals(b);
    }

    private int CompareNumeric(object a, object b)
    {
        if (a is float floatA && b is float floatB)
            return floatA.CompareTo(floatB);
        if (a is int intA && b is int intB)
            return intA.CompareTo(intB);
        if (a is bool boolA && b is bool boolB)
            return boolA.CompareTo(boolB);
        return 0;
    }
}