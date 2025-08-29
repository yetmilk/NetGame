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
            if (conf == null)
            {
                Debug.LogWarning("Config was null in MapGenerator.Generate()");
                return null;
            }

            config = conf;
            nodes.Clear();

            //GenerateLayerDistances();

            // ���η���ÿһ��Ľڵ㣨ԭ�߼����䣩
            for (int i = 0; i < conf.layers.Count; i++)
                PlaceLayer(i);

            List<List<Vector2Int>> fullConnectPaths = GenerateFullConnectPaths();

            // ����ȫ����·�����ýڵ�����
            SetUpConnections(fullConnectPaths);

            // ɸѡ�����ӵĽڵ�
            List<Node> nodesList = nodes.SelectMany(n => n).ToList();

            // ���ѡ���뷿�ڵ�����
            string bossNodeName = config.nodeBlueprints
                .Where(b => b.nodeType == NodeType.���뷿)
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
        /// ���ɡ�ȫ���ӡ�·������ǰ��ÿ���ڵ� �� ��һ�����нڵ�
        /// </summary>
        /// <returns>ȫ����·���б�</returns>
        private static List<List<Vector2Int>> GenerateFullConnectPaths()
        {
            List<List<Vector2Int>> fullConnectPaths = new List<List<Vector2Int>>();
            int totalLayers = config.layers.Count;

            // ����ÿһ�㣨�ӵ�0�㵽������2�㣬��Ϊ���һ�������������ӣ�
            for (int currentLayerIndex = 0; currentLayerIndex < totalLayers - 1; currentLayerIndex++)
            {
                int nextLayerIndex = currentLayerIndex + 1; // ��һ�������
                List<Node> currentLayerNodes = nodes[currentLayerIndex]; // ��ǰ�����нڵ�
                List<Node> nextLayerNodes = nodes[nextLayerIndex]; // ��һ�����нڵ�

                // Ϊ��ǰ���ÿ���ڵ㣬���ɵ���һ�����нڵ��·��
                foreach (Node currentNode in currentLayerNodes)
                {
                    // ������һ���ÿ���ڵ㣬���ɡ���ǰ�ڵ� �� ��һ��ڵ㡱��·��
                    foreach (Node nextNode in nextLayerNodes)
                    {
                        // ·���ṹ��[��ǰ�ڵ�����, ��һ��ڵ�����]
                        // ��SetUpConnections���Զ�����·���е������ڵ����ӣ�
                        List<Vector2Int> singlePath = new List<Vector2Int>
                        {
                            currentNode.point,  // ��㣺��ǰ��ڵ�
                            nextNode.point       // �յ㣺��һ��ڵ�
                        };

                        fullConnectPaths.Add(singlePath);
                    }
                }
            }

            return fullConnectPaths;
        }
    }
}