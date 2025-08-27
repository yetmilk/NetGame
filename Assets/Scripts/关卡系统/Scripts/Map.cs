using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Map
{
    public class Map
    {
        public List<Node> nodes;
        private Dictionary<int, List<Node>> layerNodesDic = new Dictionary<int, List<Node>>();

        public int LayerCount => layerNodesDic.Count;
        public List<Vector2Int> path;
        public string lastNodeName;
        public string configName; // similar to the act name in Slay the Spire

        public Map(string configName, string lastNodeName, List<Node> nodes, List<Vector2Int> path)
        {
            this.configName = configName;
            this.lastNodeName = lastNodeName;
            this.nodes = nodes;
            this.path = path;

            foreach (Node node in nodes)
            {
                if (layerNodesDic.ContainsKey(node.point.y))
                {
                    layerNodesDic[node.point.y].Add(node);
                }
                else
                {
                    layerNodesDic.Add(node.point.y, new List<Node>() { node });
                }
            }
        }

        public Node GetBossNode()
        {
            return nodes.FirstOrDefault(n => n.nodeType == NodeType.Boss房);
        }


        public Node[] GetLayerNodes(int layerIndex)
        {
            return layerNodesDic[layerIndex].ToArray();
        }


        public Node GetNode(Vector2Int point)
        {
            return nodes.FirstOrDefault(n => n.point.Equals(point));
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
    }
}