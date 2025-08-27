using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Map
{
    public class Node
    {
        // 节点对应的二维整数坐标点
        public readonly Vector2Int point;

        // 存储指向该节点的其他节点的二维整数坐标点列表，
        public readonly List<Vector2Int> incoming = new List<Vector2Int>();

        // 存储从该节点出发指向其他节点的二维整数坐标点列表
        public readonly List<Vector2Int> outgoing = new List<Vector2Int>();


        [JsonConverter(typeof(StringEnumConverter))]
        // 节点的类型
        public readonly NodeType nodeType;

        // 节点对应的蓝图
        public readonly NodeBlueprint blueprint;
        public BlueprintObj blueprintObj;
        /// <summary>
        /// 构造函数，用于初始化节点的属性
        /// </summary>
        /// <param name="nodeType">节点的类型</param>
        /// <param name="blueprint">节点对应的蓝图</param>
        /// <param name="point">节点对应的二维整数坐标点</param>
        public Node(NodeType nodeType, NodeBlueprint blueprint, Vector2Int point)
        {
            this.nodeType = nodeType;
            this.blueprint = blueprint;
            this.point = point;
        }

        // 向 incoming 列表中添加一个指向该节点的坐标点
        // p: 要添加的二维整数坐标点
        public void AddIncoming(Vector2Int p)
        {
            // 检查 incoming 列表中是否已经存在该坐标点，如果存在则不添加
            if (incoming.Any(element => element.Equals(p)))
                return;

            // 如果不存在，则将该坐标点添加到 incoming 列表中
            incoming.Add(p);
        }

        // 向 outgoing 列表中添加一个从该节点出发的坐标点
        // p: 要添加的二维整数坐标点
        public void AddOutgoing(Vector2Int p)
        {
            // 检查 outgoing 列表中是否已经存在该坐标点，如果存在则不添加
            if (outgoing.Any(element => element.Equals(p)))
                return;

            // 如果不存在，则将该坐标点添加到 outgoing 列表中
            outgoing.Add(p);
        }

        // 从 incoming 列表中移除指定的坐标点
        // p: 要移除的二维整数坐标点
        public void RemoveIncoming(Vector2Int p)
        {
            // 移除 incoming 列表中所有与指定坐标点相等的元素
            incoming.RemoveAll(element => element.Equals(p));
        }

        // 从 outgoing 列表中移除指定的坐标点
        // p: 要移除的二维整数坐标点
        public void RemoveOutgoing(Vector2Int p)
        {
            // 移除 outgoing 列表中所有与指定坐标点相等的元素
            outgoing.RemoveAll(element => element.Equals(p));
        }

        // 检查该节点是否没有任何连接（即 incoming 和 outgoing 列表都为空）
        // 返回值: 如果没有连接则返回 true，否则返回 false
        public bool HasNoConnections()
        {
            return incoming.Count == 0 && outgoing.Count == 0;
        }
    }
}