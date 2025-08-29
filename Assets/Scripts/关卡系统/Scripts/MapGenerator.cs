using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    public static class MapGenerator
    {
        // 存储地图配置信息
        private static MapConfig config;

        // 存储每一层与上一层的距离
        private static List<float> layerDistances;

        // 存储所有层的节点，外层列表表示层，内层列表表示该层的节点
        private static readonly List<List<Node>> nodes = new List<List<Node>>();

        /// <summary>
        /// 根据给定的地图配置生成地图
        /// </summary>
        /// <param name="conf">地图配置信息</param>
        /// <returns>生成的地图对象</returns>
        public static Map GetMap(MapConfig conf)
        {
            if (conf == null)
            {
                Debug.LogWarning("Config was null in MapGenerator.Generate()");
                return null;
            }

            config = conf;
            nodes.Clear();

            //GenerateLayerDistances();

            // 依次放置每一层的节点（原逻辑不变）
            for (int i = 0; i < conf.layers.Count; i++)
                PlaceLayer(i);

            List<List<Vector2Int>> fullConnectPaths = GenerateFullConnectPaths();

            // 根据全连接路径设置节点连接
            SetUpConnections(fullConnectPaths);

            // 筛选有连接的节点
            List<Node> nodesList = nodes.SelectMany(n => n).ToList();

            // 随机选择撤离房节点名称
            string bossNodeName = config.nodeBlueprints
                .Where(b => b.nodeType == NodeType.撤离房)
                .ToList()
                .Random()
                .name;

            return new Map(conf.name, bossNodeName, nodesList, new List<Vector2Int>());
        }

        private static float GetDistanceToLayer(int layerIndex)
        {
            if (layerIndex < 0 || layerIndex > layerDistances.Count) return 0f;
            return layerDistances.Take(layerIndex + 1).Sum();
        }

        private static void PlaceLayer(int layerIndex)
        {
            MapLayer layer = config.layers[layerIndex];
            List<Node> nodesOnThisLayer = new List<Node>();


            NodeType[] nodeTypes = config.GetRandomNode(layer.nodeNum, layerIndex);

            for (int i = 0; i < layer.nodeNum; i++)
            {
                var supportedRandomNodeTypes = config.randomNodes
                    .Where(t => config.nodeBlueprints.Any(b => b.nodeType == t.nodeType))
                    .ToList();


                NodeType nodeType = i >= layer.nodeType.Count
                    ? nodeTypes[i]
                    : layer.nodeType[i];

                NodeBlueprint blueprint = layer.randomizeEdges ? config.nodeBlueprints
                    .Where(b => b.nodeType == nodeType)
                    .ToList()
                    .Random() : layer.nodeBlueprint;

                Node node = new Node(nodeType, blueprint, new Vector2Int(i, layerIndex));


                nodesOnThisLayer.Add(node);
            }

            nodes.Add(nodesOnThisLayer);
        }

        private static void SetUpConnections(List<List<Vector2Int>> paths)
        {
            foreach (List<Vector2Int> path in paths)
            {
                for (int i = 0; i < path.Count - 1; ++i)
                {
                    Node node = GetNode(path[i]);
                    Node nextNode = GetNode(path[i + 1]);

                    if (node == null || nextNode == null) continue;

                    node.AddOutgoing(nextNode.point);
                    nextNode.AddIncoming(node.point);
                }
            }
        }

        private static Node GetNode(Vector2Int p)
        {
            if (p.y >= nodes.Count || p.y < 0) return null;
            if (p.x >= nodes[p.y].Count || p.x < 0) return null;
            return nodes[p.y][p.x];
        }

        /// <summary>
        /// 生成“全连接”路径：当前层每个节点 → 下一层所有节点
        /// </summary>
        /// <returns>全连接路径列表</returns>
        private static List<List<Vector2Int>> GenerateFullConnectPaths()
        {
            List<List<Vector2Int>> fullConnectPaths = new List<List<Vector2Int>>();
            int totalLayers = config.layers.Count;

            // 遍历每一层（从第0层到倒数第2层，因为最后一层无需向下连接）
            for (int currentLayerIndex = 0; currentLayerIndex < totalLayers - 1; currentLayerIndex++)
            {
                int nextLayerIndex = currentLayerIndex + 1; // 下一层的索引
                List<Node> currentLayerNodes = nodes[currentLayerIndex]; // 当前层所有节点
                List<Node> nextLayerNodes = nodes[nextLayerIndex]; // 下一层所有节点

                // 为当前层的每个节点，生成到下一层所有节点的路径
                foreach (Node currentNode in currentLayerNodes)
                {
                    // 遍历下一层的每个节点，生成“当前节点 → 下一层节点”的路径
                    foreach (Node nextNode in nextLayerNodes)
                    {
                        // 路径结构：[当前节点坐标, 下一层节点坐标]
                        // （SetUpConnections会自动处理路径中的连续节点连接）
                        List<Vector2Int> singlePath = new List<Vector2Int>
                        {
                            currentNode.point,  // 起点：当前层节点
                            nextNode.point       // 终点：下一层节点
                        };

                        fullConnectPaths.Add(singlePath);
                    }
                }
            }

            return fullConnectPaths;
        }
    }
}