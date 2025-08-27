using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "怪物房", menuName = "地图配置/怪物房")]
public class EnemyBlueprint : NodeBlueprint
{
    [Header("奖励类型")]
    public RewardType rewardType;

    [Header("五行类型")]
    public ElementType elementType;

    [Header("难度设置")]
    public DifficultProgress difficultProgress;

    [System.Serializable]
    public class DifficultProgress
    {
        [Header("怪物数量")]
        public float enemyNum;
        [Header("怪物数值提升倍率")]
        public float multiply;
    }

    public EnemyBlueprint()
    {
        nodeType = NodeType.普通怪房;
    }
}