using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Condition
{
    public LogicOperator logicOperator;
    public List<ConditionItem> conditions = new List<ConditionItem>();

    public enum LogicOperator
    {
        并且,
        或者
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
            if (logicOperator == LogicOperator.并且)
            {
                result = result && current;
                if (!result) break; // 短路优化
            }
            else
            {
                result = result || current;
                if (result) break; // 短路优化
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
        相等,
        不相等,
        大于,
        小于,
        大于等于,
        小于等于
    }

    public bool Evaluate(WorldState worldState)
    {
        if (worldState == null || string.IsNullOrEmpty(variableName))
            return false;

        WorldState.Value value = worldState.GetValue(variableName);
        if (value == null || value.value == null)
            return false;

        // 将 compareValue 转换为与 variableName 对应值相同的类型
        object convertedCompareValue = ConvertToSameType(value.value, compareValue);

        // 类型转换和比较逻辑
        switch (comparisonOperator)
        {
            case ComparisonOperator.相等:
                return CompareValues(value.value, convertedCompareValue);
            case ComparisonOperator.不相等:
                return !CompareValues(value.value, convertedCompareValue);
            case ComparisonOperator.大于:
                return CompareNumeric(value.value, convertedCompareValue) > 0;
            case ComparisonOperator.小于:
                return CompareNumeric(value.value, convertedCompareValue) < 0;
            case ComparisonOperator.大于等于:
                return CompareNumeric(value.value, convertedCompareValue) >= 0;
            case ComparisonOperator.小于等于:
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
        return input; // 如果无法转换，返回原始的 string 值
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