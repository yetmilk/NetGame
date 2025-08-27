using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbabilityConfig : ScriptableObject
{
    [System.Serializable]
    public class Propority
    {
        [Header("½±ÀøÀàĞÍ")]
        public RewardType type;
        [Header("¸ÅÂÊ")]
        public int propority;
    }
}
