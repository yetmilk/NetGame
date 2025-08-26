using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Map
{
    // 定义 Node 类，代表图中的一个节点
    public class Node
    {
        // 节点对应的二维整数坐标点，一旦初始化就不能再修改
        public readonly Vector2Int point;
        // 存储指向该节点的其他节点的二维整数坐标点列表，初始化时创建一个空列表
        public readonly List<Vector2Int> incoming = new List<Vector2Int>();
        // 存储从该节点出发指向其他节点的二维整数坐标点列表，初始化时创建一个空列表
        public readonly List<Vector2Int> outgoing = new List<Vector2Int>();
        // 使用 JsonConverter 特性，指定在进行 JSON 序列化和反序列化时使用 StringEnumConverter
        // 这意味着枚举类型在 JSON 中会以字符串形式表示
        [JsonConverter(typeof(StringEnumConverter))]
        // 节点的类型，一旦初始化就不能再修改
        public readonly NodeType nodeType;
        // 节点对应的蓝图名称，一旦初始化就不能再修改
        public readonly string blueprintName;
        // 节点的二维位置
        public Vector2 position;

        // 构造函数，用于初始化节点的属性
        // nodeType: 节点的类型
        // blueprintName: 节点对应的蓝图名称
        // point: 节点对应的二维整数坐标点
        public Node(NodeType nodeType, string blueprintName, Vector2Int point)
        {
            this.nodeType = nodeType;
            this.blueprintName = blueprintName;
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