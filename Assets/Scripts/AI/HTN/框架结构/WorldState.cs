
using System.Collections.Generic;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;

// 可观察的世界状态
[CreateAssetMenu(fileName = "WorldState", menuName = "HTN/WorldState")]
public class WorldState : ScriptableObject, ISerializationCallbackReceiver
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        TypeNameHandling = TypeNameHandling.Auto,
        PreserveReferencesHandling = PreserveReferencesHandling.None,
        Error = OnError,
        Formatting = Formatting.Indented
    };
    [System.Serializable]
    public class Value
    {
        [Header("值")]
        public object value;
        [Header("更改触发重新规划")]
        public bool changeTriggerReplan;

        public Value()
        {

        }
        public Value(object value, bool changeTriggerReplan = false)
        {
            this.value = value;
            this.changeTriggerReplan = changeTriggerReplan;
        }
    }
    public Dictionary<string, Value> runTimeValues = new Dictionary<string, Value>();

    // 状态变更事件
    public event EventHandler<WorldStateChangedEventArgs> StateChanged;
    // 需重新规划事件
    public event EventHandler<string> ReplanNeeded;

    [field: SerializeField]
    public string Json { get; set; }

    public bool HasState(string key) => runTimeValues.ContainsKey(key);

    public T GetState<T>(string key)
    {
        if (!runTimeValues.ContainsKey(key))
            throw new KeyNotFoundException($"State {key} not found");
        return (T)runTimeValues[key].value;
    }



    public void SetState(string key, object value, bool isPlan = false)
    {


        Value oldValue = null;
        bool existed = runTimeValues.TryGetValue(key, out oldValue);

        if (existed)
            runTimeValues[key].value = value;
        else
            runTimeValues.Add(key, new Value(value));

        // 触发状态变更事件
        if ((existed && !Equals(oldValue, value) || !existed) && !isPlan)
        {
            StateChanged?.Invoke(this, new WorldStateChangedEventArgs(key, value, oldValue));

            // 检查是否需要触发重新规划
            if (runTimeValues[key].changeTriggerReplan)
            {
                Debug.Log("SetState调用  " + isPlan);
                ReplanNeeded?.Invoke(this, key);
            }
        }
    }

    public Value GetValue(string key)
    {
        if (runTimeValues.TryGetValue(key, out var value))
        {
            return value;
        }
        return null;
    }

    public WorldState Clone()
    {
        WorldState clone = ScriptableObject.CreateInstance<WorldState>();
        // 深拷贝 runTimeValues：不仅复制字典，还要复制每个 Value 对象
        clone.runTimeValues = new Dictionary<string, Value>();
        foreach (var kvp in runTimeValues)
        {
            // 为每个 Value 创建新实例，复制其属性
            var originalValue = kvp.Value;
            var clonedValue = new Value(
                originalValue.value,
                originalValue.changeTriggerReplan
            );
            clone.runTimeValues.Add(kvp.Key, clonedValue);
        }
        return clone;
    }

    public void ApplyEffect(List<Effect> effects, bool isPlan = false)
    {
        if (effects == null && effects.Count == 0) return;
        foreach (var effect in effects)
        {
            Value value = GetValue(effect.worldStateName);
            if (value != null)
            {
                var ChangeValue = StringConversionHelper.ConvertStringToType(effect.changeValue, value.value);
                SetState(effect.worldStateName, ChangeValue, isPlan);
            }
        }
    }

    public override string ToString()
    {
        string result = "WorldState:\n";
        foreach (var kvp in runTimeValues)
        {
            result += $"  {kvp.Key}: {kvp.Value}\n";
        }
        return result;
    }

    public void OnBeforeSerialize()
    {
        // SerializeToJson();
    }

    public void OnAfterDeserialize()
    {
        DeserializeFromJson();
    }
    public void SerializeToJson()
    {
        Json = JsonConvert.SerializeObject(runTimeValues, JsonSerializerSettings);
        Debug.Log("json保存成功，当前json数据为：" + Json.ToString());
    }

    public void DeserializeFromJson()
    {
        if (string.IsNullOrEmpty(Json))
        {
            return;
        }

        runTimeValues = JsonConvert.DeserializeObject<Dictionary<string, Value>>(Json, JsonSerializerSettings);
        Debug.Log("json读取成功，当前json数据为：" + Json.ToString());
    }

    private static void OnError(object sender, ErrorEventArgs e)
    {

        Debug.LogError($"There was an error deserializing: {e.CurrentObject}: {e.ErrorContext?.Error?.Message}");
        e.ErrorContext.Handled = true;
    }

    public List<(string variableName, string variableType)> GetVariableNameAndTypePairs()
    {
        var result = new List<(string variableName, string variableType)>();

        foreach (var kvp in runTimeValues)
        {
            string typeName = kvp.Value.value?.GetType().Name ?? "Null";
            result.Add((kvp.Key, typeName));
        }

        return result;
    }
}
// 世界状态变更事件参数
public class WorldStateChangedEventArgs : EventArgs
{
    public string Key { get; }
    public object Value { get; }
    public object OldValue { get; }

    public WorldStateChangedEventArgs(string key, object value, object oldValue)
    {
        Key = key;
        Value = value;
        OldValue = oldValue;
    }
}


// 变量类型枚举
public enum VariableType
{
    Float,
    Int,
    Bool,
    String,
    Vector3,
    GameObject,
}
#region--可序列化的变量类---
// 
[System.Serializable]
public class GameObjectVariable
{
    public string name;
    public GameObject value;
}
[System.Serializable]
public class Vector3Variable
{
    public string name;
    public Vector3 value;
}
[System.Serializable]
public class FloatVariable
{
    public string name;
    public float value;
}

[System.Serializable]
public class IntVariable
{
    public string name;
    public int value;
}

[System.Serializable]
public class BoolVariable
{
    public string name;
    public bool value;
}

[System.Serializable]
public class StringVariable
{
    public string name;
    public string value;
}
#endregion
