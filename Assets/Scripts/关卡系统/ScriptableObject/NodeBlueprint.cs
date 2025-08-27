using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public enum NodeType
    {

        普通怪房,

        精英怪房,

        奇遇房,

        Boss房,

        商店房,

        撤离房,
    }

    public enum RewardType
    {
        无,
        秘籍,
        秘籍强化,
        耐久度道具,
        等价物道具,
        装备,
    }

    public enum ElementType
    {
        无,
        水,
        火,
        木,
    }
}

namespace Map
{
    [CreateAssetMenu(fileName = "地图节点", menuName = "地图配置/基础节点")]
    public class NodeBlueprint : ScriptableObject
    {
        [Header("该节点传送的场景")]
        public SceneName scene;

        public NodeType nodeType;
    }






}

