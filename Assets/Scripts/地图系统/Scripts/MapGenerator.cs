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
            // 检查配置是否为空
            if (conf == null)
            {
                Debug.LogWarning("Config was null in MapGenerator.Generate()");
                return null;
            }

            // 保存配置信息
            config = conf;
            // 清空之前存储的节点信息
            nodes.Clear();

            // 生成每一层与上一层的距离
            GenerateLayerDistances();

            // 依次放置每一层的节点
            for (int i = 0; i < conf.layers.Count; i++)
                PlaceLayer(i);

            // 生成节点之间的路径
            List<List<Vector2Int>> paths = GeneratePaths();

            // 随机化节点的位置
            RandomizeNodePositions();

            // 根据生成的路径设置节点之间的连接
            SetUpConnections(paths);

            // 移除交叉的连接
            RemoveCrossConnections();

            // 筛选出有连接的节点
            List<Node> nodesList = nodes.SelectMany(n => n).Where(n => n.incoming.Count > 0 || n.outgoing.Count > 0).ToList();

            // 随机选择一个boss关卡的名称
            string bossNodeName = config.nodeBlueprints.Where(b => b.nodeType == NodeType.Boss).ToList().Random().name;
            // 返回生成的地图对象
            return new Map(conf.name, bossNodeName, nodesList, new List<Vector2Int>());
        }

        /// <summary>
        /// 生成每一层与上一层的距离
        /// </summary>
        private static void GenerateLayerDistances()
        {
            layerDistances = new List<float>();
            // 遍历每一层，获取该层与上一层的距离
            foreach (MapLayer layer in config.layers)
                layerDistances.Add(layer.distanceFromPreviousLayer.GetValue());
        }

        /// <summary>
        /// 获取到指定层的总距离
        /// </summary>
        /// <param name="layerIndex">层的索引</param>
        /// <returns>到指定层的总距离</returns>
        private static float GetDistanceToLayer(int layerIndex)
        {
            // 检查索引是否合法
            if (layerIndex < 0 || layerIndex > layerDistances.Count) return 0f;

            // 计算到指定层的总距离
            return layerDistances.Take(layerIndex + 1).Sum();
        }

        /// <summary>
        /// 在指定层放置节点
        /// </summary>
        /// <param name="layerIndex">层的索引</param>
        private static void PlaceLayer(int layerIndex)
        {
            // 获取当前层的配置信息
            MapLayer layer = config.layers[layerIndex];
            // 存储当前层的节点
            List<Node> nodesOnThisLayer = new List<Node>();

            // 计算当前层节点的偏移量，使节点居中
            float offset = layer.nodesApartDistance * config.GridWidth / 2f;

            // 遍历当前层的每一列
            for (int i = 0; i < config.GridWidth; i++)
            {
                // 获取支持随机生成的节点类型
                var supportedRandomNodeTypes =
                    config.randomNodes.Where(t => config.nodeBlueprints.Any(b => b.nodeType == t)).ToList();
                // 根据随机概率选择节点类型
                NodeType nodeType = Random.Range(0f, 1f) < layer.randomizeNodes && supportedRandomNodeTypes.Count > 0
                    ? supportedRandomNodeTypes.Random()
                    : layer.nodeType;
                // 随机选择一个节点蓝图的名称
                string blueprintName = config.nodeBlueprints.Where(b => b.nodeType == nodeType).ToList().Random().name;
                // 创建一个节点对象
                Node node = new Node(nodeType, blueprintName, new Vector2Int(i, layerIndex))
                {
                    // 设置节点的位置
                    position = new Vector2(-offset + i * layer.nodesApartDistance, GetDistanceToLayer(layerIndex))
                };
                // 将节点添加到当前层的节点列表中
                nodesOnThisLayer.Add(node);
            }

            // 将当前层的节点列表添加到总的节点列表中
            nodes.Add(nodesOnThisLayer);
        }

        /// <summary>
        /// 随机化节点的位置
        /// </summary>
        private static void RandomizeNodePositions()
        {
            // 遍历每一层
            for (int index = 0; index < nodes.Count; index++)
            {
                // 获取当前层的节点列表
                List<Node> list = nodes[index];
                // 获取当前层的配置信息
                MapLayer layer = config.layers[index];
                // 获取到下一层的距离
                float distToNextLayer = index + 1 >= layerDistances.Count
                    ? 0f
                    : layerDistances[index + 1];
                // 获取到上一层的距离
                float distToPreviousLayer = layerDistances[index];

                // 遍历当前层的每一个节点
                foreach (Node node in list)
                {
                    // 生成随机的x和y偏移量
                    float xRnd = Random.Range(-0.5f, 0.5f);
                    float yRnd = Random.Range(-0.5f, 0.5f);

                    // 计算实际的x和y偏移量
                    float x = xRnd * layer.nodesApartDistance;
                    float y = yRnd < 0 ? distToPreviousLayer * yRnd : distToNextLayer * yRnd;

                    // 更新节点的位置
                    node.position += new Vector2(x, y) * layer.randomizePosition;
                }
            }
        }

        /// <summary>
        /// 根据生成的路径设置节点之间的连接
        /// </summary>
        /// <param name="paths">节点之间的路径列表</param>
        private static void SetUpConnections(List<List<Vector2Int>> paths)
        {
            // 遍历每一条路径
            foreach (List<Vector2Int> path in paths)
            {
                // 遍历路径上的每一个节点
                for (int i = 0; i < path.Count - 1; ++i)
                {
                    // 获取当前节点
                    Node node = GetNode(path[i]);
                    // 获取下一个节点
                    Node nextNode = GetNode(path[i + 1]);
                    // 设置当前节点的出边
                    node.AddOutgoing(nextNode.point);
                    // 设置下一个节点的入边
                    nextNode.AddIncoming(node.point);
                }
            }
        }

        /// <summary>
        /// 移除交叉的连接
        /// </summary>
        private static void RemoveCrossConnections()
        {
            // 遍历每一列和每一层
            for (int i = 0; i < config.GridWidth - 1; ++i)
                for (int j = 0; j < config.layers.Count - 1; ++j)
                {
                    // 获取当前节点
                    Node node = GetNode(new Vector2Int(i, j));
                    // 检查当前节点是否存在且有连接
                    if (node == null || node.HasNoConnections()) continue;
                    // 获取当前节点右侧的节点
                    Node right = GetNode(new Vector2Int(i + 1, j));
                    // 检查右侧节点是否存在且有连接
                    if (right == null || right.HasNoConnections()) continue;
                    // 获取当前节点上方的节点
                    Node top = GetNode(new Vector2Int(i, j + 1));
                    // 检查上方节点是否存在且有连接
                    if (top == null || top.HasNoConnections()) continue;
                    // 获取当前节点右上方的节点
                    Node topRight = GetNode(new Vector2Int(i + 1, j + 1));
                    // 检查右上方节点是否存在且有连接
                    if (topRight == null || topRight.HasNoConnections()) continue;

                    // 检查是否存在交叉连接
                    if (!node.outgoing.Any(element => element.Equals(topRight.point))) continue;
                    if (!right.outgoing.Any(element => element.Equals(top.point))) continue;

                    // 添加直接连接
                    node.AddOutgoing(top.point);
                    top.AddIncoming(node.point);

                    right.AddOutgoing(topRight.point);
                    topRight.AddIncoming(right.point);

                    // 生成一个随机数
                    float rnd = Random.Range(0f, 1f);
                    if (rnd < 0.2f)
                    {
                        // 移除两条交叉连接
                        node.RemoveOutgoing(topRight.point);
                        topRight.RemoveIncoming(node.point);

                        right.RemoveOutgoing(top.point);
                        top.RemoveIncoming(right.point);
                    }
                    else if (rnd < 0.6f)
                    {
                        // 移除一条交叉连接
                        node.RemoveOutgoing(topRight.point);
                        topRight.RemoveIncoming(node.point);
                    }
                    else
                    {
                        // 移除另一条交叉连接
                        right.RemoveOutgoing(top.point);
                        top.RemoveIncoming(right.point);
                    }
                }
        }

        /// <summary>
        /// 根据坐标获取节点
        /// </summary>
        /// <param name="p">节点的坐标</param>
        /// <returns>对应的节点对象，如果不存在则返回null</returns>
        private static Node GetNode(Vector2Int p)
        {
            // 检查y坐标是否越界
            if (p.y >= nodes.Count) return null;
            // 检查x坐标是否越界
            if (p.x >= nodes[p.y].Count) return null;

            // 返回对应的节点对象
            return nodes[p.y][p.x];
        }

        /// <summary>
        /// 获取最终节点的坐标
        /// </summary>
        /// <returns>最终节点的坐标</returns>
        private static Vector2Int GetFinalNode()
        {
            // 获取最后一层的索引
            int y = config.layers.Count - 1;
            // 如果网格宽度为奇数，返回中间列的节点坐标
            if (config.GridWidth % 2 == 1)
                return new Vector2Int(config.GridWidth / 2, y);

            // 如果网格宽度为偶数，随机返回中间两列的节点坐标
            return Random.Range(0, 2) == 0
                ? new Vector2Int(config.GridWidth / 2, y)
                : new Vector2Int(config.GridWidth / 2 - 1, y);
        }

        /// <summary>
        /// 生成节点之间的路径
        /// </summary>
        /// <returns>路径列表</returns>
        private static List<List<Vector2Int>> GeneratePaths()
        {
            // 获取最终节点的坐标
            Vector2Int finalNode = GetFinalNode();
            // 存储生成的路径
            var paths = new List<List<Vector2Int>>();
            // 获取起始节点的数量
            int numOfStartingNodes = config.numOfStartingNodes.GetValue();
            // 获取boss前节点的数量
            int numOfPreBossNodes = config.numOfPreBossNodes.GetValue();

            // 存储候选的x坐标
            List<int> candidateXs = new List<int>();
            for (int i = 0; i < config.GridWidth; i++)
                candidateXs.Add(i);

            // 打乱候选x坐标的顺序
            candidateXs.Shuffle();
            // 选取起始节点的x坐标
            IEnumerable<int> startingXs = candidateXs.Take(numOfStartingNodes);
            // 生成起始节点的坐标列表
            List<Vector2Int> startingPoints = (from x in startingXs select new Vector2Int(x, 0)).ToList();

            // 再次打乱候选x坐标的顺序
            candidateXs.Shuffle();
            // 选取boss前节点的x坐标
            IEnumerable<int> preBossXs = candidateXs.Take(numOfPreBossNodes);
            // 生成boss前节点的坐标列表
            List<Vector2Int> preBossPoints = (from x in preBossXs select new Vector2Int(x, finalNode.y - 1)).ToList();

            // 计算路径的数量
            int numOfPaths = Mathf.Max(numOfStartingNodes, numOfPreBossNodes) + Mathf.Max(0, config.extraPaths);
            // 生成路径
            for (int i = 0; i < numOfPaths; ++i)
            {
                // 获取起始节点的坐标
                Vector2Int startNode = startingPoints[i % numOfStartingNodes];
                // 获取结束节点的坐标
                Vector2Int endNode = preBossPoints[i % numOfPreBossNodes];
                // 生成一条路径
                List<Vector2Int> path = Path(startNode, endNode);
                // 将最终节点添加到路径中
                path.Add(finalNode);
                // 将路径添加到路径列表中
                paths.Add(path);
            }

            return paths;
        }

        /// <summary>
        /// 生成从起始点到结束点的随机路径
        /// </summary>
        /// <param name="fromPoint">起始点的坐标</param>
        /// <param name="toPoint">结束点的坐标</param>
        /// <returns>生成的路径</returns>
        private static List<Vector2Int> Path(Vector2Int fromPoint, Vector2Int toPoint)
        {
            // 获取结束点的行和列
            int toRow = toPoint.y;
            int toCol = toPoint.x;

            // 获取起始点的列
            int lastNodeCol = fromPoint.x;

            // 存储生成的路径
            List<Vector2Int> path = new List<Vector2Int> { fromPoint };
            // 存储候选的列
            List<int> candidateCols = new List<int>();
            // 从第1行开始遍历到结束点的上一行
            for (int row = 1; row < toRow; ++row)
            {
                // 清空候选列列表
                candidateCols.Clear();

                // 计算垂直距离
                int verticalDistance = toRow - row;
                int horizontalDistance;

                // 向前移动一列
                int forwardCol = lastNodeCol;
                horizontalDistance = Mathf.Abs(toCol - forwardCol);
                if (horizontalDistance <= verticalDistance)
                    candidateCols.Add(lastNodeCol);

                // 向左移动一列
                int leftCol = lastNodeCol - 1;
                horizontalDistance = Mathf.Abs(toCol - leftCol);
                if (leftCol >= 0 && horizontalDistance <= verticalDistance)
                    candidateCols.Add(leftCol);

                // 向右移动一列
                int rightCol = lastNodeCol + 1;
                horizontalDistance = Mathf.Abs(toCol - rightCol);
                if (rightCol < config.GridWidth && horizontalDistance <= verticalDistance)
                    candidateCols.Add(rightCol);

                // 随机选择一个候选列
                int randomCandidateIndex = Random.Range(0, candidateCols.Count);
                int candidateCol = candidateCols[randomCandidateIndex];
                // 生成下一个点的坐标
                Vector2Int nextPoint = new Vector2Int(candidateCol, row);

                // 将下一个点添加到路径中
                path.Add(nextPoint);

                // 更新最后一个节点的列
                lastNodeCol = candidateCol;
            }

            // 将结束点添加到路径中
            path.Add(toPoint);

            return path;
        }
    }
}