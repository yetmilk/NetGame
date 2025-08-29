using System;
using System.Collections.Generic;
using OneLine;
using UnityEngine;

namespace Map
{
    [CreateAssetMenu(fileName = "MapConfig", menuName = "地图配置/MapConfig")]
    public class MapConfig : ScriptableObject
    {
        [Header("节点预制体")]
        public List<NodeBlueprint> nodeBlueprints;
        [Header("随机节点列表")]
        public List<NodeInfo> randomNodes = new List<NodeInfo>();

        [System.Serializable]
        public class NodeInfo
        {
            public NodeType nodeType;

            [Header("初始概率")]
            public int startChance;

            [Header("代表生成此房间对房间生成概率的影响")]
            public List<NodeEffect> effect;

            [System.Serializable]
            public class NodeEffect
            {
                [Header("效果作用的类型")]
                public NodeType node;
                [Header("影响数值（增加填正数，减少填负数）")]
                public int effect;
            }
        }

        private int width = 0;
        public int GridWidth
        {
            get
            {
                if (width == 0)
                {
                    foreach (var layer in layers)
                    {
                        width = Mathf.Max(layer.nodeNum, width);
                    }
                    return width;
                }

                return width;
            }
        }

        public List<MapLayer> layers;

        /// <summary>
        /// 生成随机节点类型数组
        /// </summary>
        /// <param name="num">需要生成的节点数量</param>
        /// <returns>随机节点类型数组</returns>
        public NodeType[] GetRandomNode(int num, int layerIndex)
        {
            List<NodeType> resultNodes = new List<NodeType>();

            Dictionary<NodeType, int> dynamicProbDict = new Dictionary<NodeType, int>();
            foreach (var nodeInfo in randomNodes)
            {
                if (!dynamicProbDict.ContainsKey(nodeInfo.nodeType))
                {
                    if (!layers[layerIndex].nodeType.Contains(nodeInfo.nodeType) ||
                        (nodeInfo.nodeType == NodeType.普通怪房 || nodeInfo.nodeType == NodeType.精英怪房))
                        dynamicProbDict.Add(nodeInfo.nodeType, nodeInfo.startChance);
                }
            }

            for (int i = 0; i < num; i++)
            {
                //  构建“可选中的节点权重列表”
                List<(NodeType Type, int Weight)> selectableNodes = new List<(NodeType, int)>();
                int totalWeight = 0;

                foreach (var nodeInfo in randomNodes)
                {
                    NodeType nodeType = nodeInfo.nodeType;
                    // 过滤条件：奇遇房/商店房只能存在1个，其他节点无限制
                    bool isSpecialNode = nodeType == NodeType.奇遇房 || nodeType == NodeType.商店房;
                    if (isSpecialNode && resultNodes.Contains(nodeType))
                    {
                        continue; // 已存在特殊节点，跳过
                    }

                    // 从字典中获取当前概率
                    if (dynamicProbDict.TryGetValue(nodeType, out int currentWeight) && currentWeight > 0)
                    {
                        selectableNodes.Add((nodeType, currentWeight));
                        totalWeight += currentWeight;
                    }
                }

                // 若无可选中节点（理论上不会发生），用默认类型填充
                if (selectableNodes.Count == 0)
                {
                    resultNodes.Add(NodeType.普通怪房);
                    continue;
                }

                // 生成随机权重值
                int randomWeight = UnityEngine.Random.Range(0, totalWeight + 1);

                // 遍历权重列表，找到对应节点
                NodeType selectedType = NodeType.普通怪房;
                int currentSum = 0;
                foreach (var (type, weight) in selectableNodes)
                {
                    currentSum += weight;
                    if (currentSum >= randomWeight)
                    {
                        selectedType = type;
                        break;
                    }
                }

                // 应用节点的 effect 调整概率
                var selectedNodeInfo = randomNodes.Find(n => n.nodeType == selectedType);
                if (selectedNodeInfo != null)
                {
                    foreach (var effect in selectedNodeInfo.effect)
                    {
                        if (dynamicProbDict.ContainsKey(effect.node))
                        {
                            // 调整概率,避免概率为负
                            dynamicProbDict[effect.node] = Mathf.Max(0, dynamicProbDict[effect.node] + effect.effect);
                        }
                    }
                }

                //将选中节点加入结果
                resultNodes.Add(selectedType);
            }

            return resultNodes.ToArray();
        }
    }
}