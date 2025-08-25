using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    public static class MapGenerator
    {
        // �洢��ͼ������Ϣ
        private static MapConfig config;

        // �洢ÿһ������һ��ľ���
        private static List<float> layerDistances;

        // �洢���в�Ľڵ㣬����б��ʾ�㣬�ڲ��б��ʾ�ò�Ľڵ�
        private static readonly List<List<Node>> nodes = new List<List<Node>>();

        /// <summary>
        /// ���ݸ����ĵ�ͼ�������ɵ�ͼ
        /// </summary>
        /// <param name="conf">��ͼ������Ϣ</param>
        /// <returns>���ɵĵ�ͼ����</returns>
        public static Map GetMap(MapConfig conf)
        {
            // ��������Ƿ�Ϊ��
            if (conf == null)
            {
                Debug.LogWarning("Config was null in MapGenerator.Generate()");
                return null;
            }

            // ����������Ϣ
            config = conf;
            // ���֮ǰ�洢�Ľڵ���Ϣ
            nodes.Clear();

            // ����ÿһ������һ��ľ���
            GenerateLayerDistances();

            // ���η���ÿһ��Ľڵ�
            for (int i = 0; i < conf.layers.Count; i++)
                PlaceLayer(i);

            // ���ɽڵ�֮���·��
            List<List<Vector2Int>> paths = GeneratePaths();

            // ������ڵ��λ��
            RandomizeNodePositions();

            // �������ɵ�·�����ýڵ�֮�������
            SetUpConnections(paths);

            // �Ƴ����������
            RemoveCrossConnections();

            // ɸѡ�������ӵĽڵ�
            List<Node> nodesList = nodes.SelectMany(n => n).Where(n => n.incoming.Count > 0 || n.outgoing.Count > 0).ToList();

            // ���ѡ��һ��boss�ؿ�������
            string bossNodeName = config.nodeBlueprints.Where(b => b.nodeType == NodeType.Boss).ToList().Random().name;
            // �������ɵĵ�ͼ����
            return new Map(conf.name, bossNodeName, nodesList, new List<Vector2Int>());
        }

        /// <summary>
        /// ����ÿһ������һ��ľ���
        /// </summary>
        private static void GenerateLayerDistances()
        {
            layerDistances = new List<float>();
            // ����ÿһ�㣬��ȡ�ò�����һ��ľ���
            foreach (MapLayer layer in config.layers)
                layerDistances.Add(layer.distanceFromPreviousLayer.GetValue());
        }

        /// <summary>
        /// ��ȡ��ָ������ܾ���
        /// </summary>
        /// <param name="layerIndex">�������</param>
        /// <returns>��ָ������ܾ���</returns>
        private static float GetDistanceToLayer(int layerIndex)
        {
            // ��������Ƿ�Ϸ�
            if (layerIndex < 0 || layerIndex > layerDistances.Count) return 0f;

            // ���㵽ָ������ܾ���
            return layerDistances.Take(layerIndex + 1).Sum();
        }

        /// <summary>
        /// ��ָ������ýڵ�
        /// </summary>
        /// <param name="layerIndex">�������</param>
        private static void PlaceLayer(int layerIndex)
        {
            // ��ȡ��ǰ���������Ϣ
            MapLayer layer = config.layers[layerIndex];
            // �洢��ǰ��Ľڵ�
            List<Node> nodesOnThisLayer = new List<Node>();

            // ���㵱ǰ��ڵ��ƫ������ʹ�ڵ����
            float offset = layer.nodesApartDistance * config.GridWidth / 2f;

            // ������ǰ���ÿһ��
            for (int i = 0; i < config.GridWidth; i++)
            {
                // ��ȡ֧��������ɵĽڵ�����
                var supportedRandomNodeTypes =
                    config.randomNodes.Where(t => config.nodeBlueprints.Any(b => b.nodeType == t)).ToList();
                // �����������ѡ��ڵ�����
                NodeType nodeType = Random.Range(0f, 1f) < layer.randomizeNodes && supportedRandomNodeTypes.Count > 0
                    ? supportedRandomNodeTypes.Random()
                    : layer.nodeType;
                // ���ѡ��һ���ڵ���ͼ������
                string blueprintName = config.nodeBlueprints.Where(b => b.nodeType == nodeType).ToList().Random().name;
                // ����һ���ڵ����
                Node node = new Node(nodeType, blueprintName, new Vector2Int(i, layerIndex))
                {
                    // ���ýڵ��λ��
                    position = new Vector2(-offset + i * layer.nodesApartDistance, GetDistanceToLayer(layerIndex))
                };
                // ���ڵ���ӵ���ǰ��Ľڵ��б���
                nodesOnThisLayer.Add(node);
            }

            // ����ǰ��Ľڵ��б���ӵ��ܵĽڵ��б���
            nodes.Add(nodesOnThisLayer);
        }

        /// <summary>
        /// ������ڵ��λ��
        /// </summary>
        private static void RandomizeNodePositions()
        {
            // ����ÿһ��
            for (int index = 0; index < nodes.Count; index++)
            {
                // ��ȡ��ǰ��Ľڵ��б�
                List<Node> list = nodes[index];
                // ��ȡ��ǰ���������Ϣ
                MapLayer layer = config.layers[index];
                // ��ȡ����һ��ľ���
                float distToNextLayer = index + 1 >= layerDistances.Count
                    ? 0f
                    : layerDistances[index + 1];
                // ��ȡ����һ��ľ���
                float distToPreviousLayer = layerDistances[index];

                // ������ǰ���ÿһ���ڵ�
                foreach (Node node in list)
                {
                    // ���������x��yƫ����
                    float xRnd = Random.Range(-0.5f, 0.5f);
                    float yRnd = Random.Range(-0.5f, 0.5f);

                    // ����ʵ�ʵ�x��yƫ����
                    float x = xRnd * layer.nodesApartDistance;
                    float y = yRnd < 0 ? distToPreviousLayer * yRnd : distToNextLayer * yRnd;

                    // ���½ڵ��λ��
                    node.position += new Vector2(x, y) * layer.randomizePosition;
                }
            }
        }

        /// <summary>
        /// �������ɵ�·�����ýڵ�֮�������
        /// </summary>
        /// <param name="paths">�ڵ�֮���·���б�</param>
        private static void SetUpConnections(List<List<Vector2Int>> paths)
        {
            // ����ÿһ��·��
            foreach (List<Vector2Int> path in paths)
            {
                // ����·���ϵ�ÿһ���ڵ�
                for (int i = 0; i < path.Count - 1; ++i)
                {
                    // ��ȡ��ǰ�ڵ�
                    Node node = GetNode(path[i]);
                    // ��ȡ��һ���ڵ�
                    Node nextNode = GetNode(path[i + 1]);
                    // ���õ�ǰ�ڵ�ĳ���
                    node.AddOutgoing(nextNode.point);
                    // ������һ���ڵ�����
                    nextNode.AddIncoming(node.point);
                }
            }
        }

        /// <summary>
        /// �Ƴ����������
        /// </summary>
        private static void RemoveCrossConnections()
        {
            // ����ÿһ�к�ÿһ��
            for (int i = 0; i < config.GridWidth - 1; ++i)
                for (int j = 0; j < config.layers.Count - 1; ++j)
                {
                    // ��ȡ��ǰ�ڵ�
                    Node node = GetNode(new Vector2Int(i, j));
                    // ��鵱ǰ�ڵ��Ƿ������������
                    if (node == null || node.HasNoConnections()) continue;
                    // ��ȡ��ǰ�ڵ��Ҳ�Ľڵ�
                    Node right = GetNode(new Vector2Int(i + 1, j));
                    // ����Ҳ�ڵ��Ƿ������������
                    if (right == null || right.HasNoConnections()) continue;
                    // ��ȡ��ǰ�ڵ��Ϸ��Ľڵ�
                    Node top = GetNode(new Vector2Int(i, j + 1));
                    // ����Ϸ��ڵ��Ƿ������������
                    if (top == null || top.HasNoConnections()) continue;
                    // ��ȡ��ǰ�ڵ����Ϸ��Ľڵ�
                    Node topRight = GetNode(new Vector2Int(i + 1, j + 1));
                    // ������Ϸ��ڵ��Ƿ������������
                    if (topRight == null || topRight.HasNoConnections()) continue;

                    // ����Ƿ���ڽ�������
                    if (!node.outgoing.Any(element => element.Equals(topRight.point))) continue;
                    if (!right.outgoing.Any(element => element.Equals(top.point))) continue;

                    // ���ֱ������
                    node.AddOutgoing(top.point);
                    top.AddIncoming(node.point);

                    right.AddOutgoing(topRight.point);
                    topRight.AddIncoming(right.point);

                    // ����һ�������
                    float rnd = Random.Range(0f, 1f);
                    if (rnd < 0.2f)
                    {
                        // �Ƴ�������������
                        node.RemoveOutgoing(topRight.point);
                        topRight.RemoveIncoming(node.point);

                        right.RemoveOutgoing(top.point);
                        top.RemoveIncoming(right.point);
                    }
                    else if (rnd < 0.6f)
                    {
                        // �Ƴ�һ����������
                        node.RemoveOutgoing(topRight.point);
                        topRight.RemoveIncoming(node.point);
                    }
                    else
                    {
                        // �Ƴ���һ����������
                        right.RemoveOutgoing(top.point);
                        top.RemoveIncoming(right.point);
                    }
                }
        }

        /// <summary>
        /// ���������ȡ�ڵ�
        /// </summary>
        /// <param name="p">�ڵ������</param>
        /// <returns>��Ӧ�Ľڵ��������������򷵻�null</returns>
        private static Node GetNode(Vector2Int p)
        {
            // ���y�����Ƿ�Խ��
            if (p.y >= nodes.Count) return null;
            // ���x�����Ƿ�Խ��
            if (p.x >= nodes[p.y].Count) return null;

            // ���ض�Ӧ�Ľڵ����
            return nodes[p.y][p.x];
        }

        /// <summary>
        /// ��ȡ���սڵ������
        /// </summary>
        /// <returns>���սڵ������</returns>
        private static Vector2Int GetFinalNode()
        {
            // ��ȡ���һ�������
            int y = config.layers.Count - 1;
            // ���������Ϊ�����������м��еĽڵ�����
            if (config.GridWidth % 2 == 1)
                return new Vector2Int(config.GridWidth / 2, y);

            // ���������Ϊż������������м����еĽڵ�����
            return Random.Range(0, 2) == 0
                ? new Vector2Int(config.GridWidth / 2, y)
                : new Vector2Int(config.GridWidth / 2 - 1, y);
        }

        /// <summary>
        /// ���ɽڵ�֮���·��
        /// </summary>
        /// <returns>·���б�</returns>
        private static List<List<Vector2Int>> GeneratePaths()
        {
            // ��ȡ���սڵ������
            Vector2Int finalNode = GetFinalNode();
            // �洢���ɵ�·��
            var paths = new List<List<Vector2Int>>();
            // ��ȡ��ʼ�ڵ������
            int numOfStartingNodes = config.numOfStartingNodes.GetValue();
            // ��ȡbossǰ�ڵ������
            int numOfPreBossNodes = config.numOfPreBossNodes.GetValue();

            // �洢��ѡ��x����
            List<int> candidateXs = new List<int>();
            for (int i = 0; i < config.GridWidth; i++)
                candidateXs.Add(i);

            // ���Һ�ѡx�����˳��
            candidateXs.Shuffle();
            // ѡȡ��ʼ�ڵ��x����
            IEnumerable<int> startingXs = candidateXs.Take(numOfStartingNodes);
            // ������ʼ�ڵ�������б�
            List<Vector2Int> startingPoints = (from x in startingXs select new Vector2Int(x, 0)).ToList();

            // �ٴδ��Һ�ѡx�����˳��
            candidateXs.Shuffle();
            // ѡȡbossǰ�ڵ��x����
            IEnumerable<int> preBossXs = candidateXs.Take(numOfPreBossNodes);
            // ����bossǰ�ڵ�������б�
            List<Vector2Int> preBossPoints = (from x in preBossXs select new Vector2Int(x, finalNode.y - 1)).ToList();

            // ����·��������
            int numOfPaths = Mathf.Max(numOfStartingNodes, numOfPreBossNodes) + Mathf.Max(0, config.extraPaths);
            // ����·��
            for (int i = 0; i < numOfPaths; ++i)
            {
                // ��ȡ��ʼ�ڵ������
                Vector2Int startNode = startingPoints[i % numOfStartingNodes];
                // ��ȡ�����ڵ������
                Vector2Int endNode = preBossPoints[i % numOfPreBossNodes];
                // ����һ��·��
                List<Vector2Int> path = Path(startNode, endNode);
                // �����սڵ���ӵ�·����
                path.Add(finalNode);
                // ��·����ӵ�·���б���
                paths.Add(path);
            }

            return paths;
        }

        /// <summary>
        /// ���ɴ���ʼ�㵽����������·��
        /// </summary>
        /// <param name="fromPoint">��ʼ�������</param>
        /// <param name="toPoint">�����������</param>
        /// <returns>���ɵ�·��</returns>
        private static List<Vector2Int> Path(Vector2Int fromPoint, Vector2Int toPoint)
        {
            // ��ȡ��������к���
            int toRow = toPoint.y;
            int toCol = toPoint.x;

            // ��ȡ��ʼ�����
            int lastNodeCol = fromPoint.x;

            // �洢���ɵ�·��
            List<Vector2Int> path = new List<Vector2Int> { fromPoint };
            // �洢��ѡ����
            List<int> candidateCols = new List<int>();
            // �ӵ�1�п�ʼ���������������һ��
            for (int row = 1; row < toRow; ++row)
            {
                // ��պ�ѡ���б�
                candidateCols.Clear();

                // ���㴹ֱ����
                int verticalDistance = toRow - row;
                int horizontalDistance;

                // ��ǰ�ƶ�һ��
                int forwardCol = lastNodeCol;
                horizontalDistance = Mathf.Abs(toCol - forwardCol);
                if (horizontalDistance <= verticalDistance)
                    candidateCols.Add(lastNodeCol);

                // �����ƶ�һ��
                int leftCol = lastNodeCol - 1;
                horizontalDistance = Mathf.Abs(toCol - leftCol);
                if (leftCol >= 0 && horizontalDistance <= verticalDistance)
                    candidateCols.Add(leftCol);

                // �����ƶ�һ��
                int rightCol = lastNodeCol + 1;
                horizontalDistance = Mathf.Abs(toCol - rightCol);
                if (rightCol < config.GridWidth && horizontalDistance <= verticalDistance)
                    candidateCols.Add(rightCol);

                // ���ѡ��һ����ѡ��
                int randomCandidateIndex = Random.Range(0, candidateCols.Count);
                int candidateCol = candidateCols[randomCandidateIndex];
                // ������һ���������
                Vector2Int nextPoint = new Vector2Int(candidateCol, row);

                // ����һ������ӵ�·����
                path.Add(nextPoint);

                // �������һ���ڵ����
                lastNodeCol = candidateCol;
            }

            // ����������ӵ�·����
            path.Add(toPoint);

            return path;
        }
    }
}