using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbabilityConfig : ScriptableObject
{
    [System.Serializable]
    public class Propority
    {
        [Header("��������")]
        public RewardType type;
        [Header("����")]
        public int propority;
    }
}
