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

        [Header("如果不使用随机生成，则生成该类型的房间")]
        public NodeBlueprint nodeBlueprint;
        [Header("是否使用随机房间配置")]
        public bool randomizeEdges;


    }
}