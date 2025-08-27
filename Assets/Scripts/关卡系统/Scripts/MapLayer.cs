using OneLine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    [System.Serializable]
    public class MapLayer
    {
        [Header("该层生成节点数量")]
        public int nodeNum;
        [Header("如果不使用随机生成，则生成该类型的房间")]
        public List<NodeType> nodeType;
        [Header("是否使用随机房间类型")]
        public bool randomizeNodes;

        [Header("如果不使用随机生成，则生成该类型的房间")]
        public NodeBlueprint nodeBlueprint;
        [Header("是否使用随机房间配置")]
        public bool randomizeEdges;


        [Tooltip("Distance between the nodes on this layer")]
        public float nodesApartDistance;
        [Tooltip("If this is set to 0, nodes on this layer will appear in a straight line. Closer to 1f = more position randomization")]
        [Range(0f, 1f)] public float randomizePosition;
    }
}