// 可序列化的方法组
using System.Collections.Generic;

[System.Serializable]
public class MethodGroup
{
    public string groupName = "Method Group";
    public List<ConditionTaskPair> pairs = new List<ConditionTaskPair>();
}