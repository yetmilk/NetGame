using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Map
{
    public class Node
    {
        // �ڵ��Ӧ�Ķ�ά���������
        public readonly Vector2Int point;

        // �洢ָ��ýڵ�������ڵ�Ķ�ά����������б�
        public readonly List<Vector2Int> incoming = new List<Vector2Int>();

        // �洢�Ӹýڵ����ָ�������ڵ�Ķ�ά����������б�
        public readonly List<Vector2Int> outgoing = new List<Vector2Int>();


        [JsonConverter(typeof(StringEnumConverter))]
        // �ڵ������
        public readonly NodeType nodeType;

        // �ڵ��Ӧ����ͼ
        public readonly NodeBlueprint blueprint;
        public BlueprintObj blueprintObj;
        /// <summary>
        /// ���캯�������ڳ�ʼ���ڵ������
        /// </summary>
        /// <param name="nodeType">�ڵ������</param>
        /// <param name="blueprint">�ڵ��Ӧ����ͼ</param>
        /// <param name="point">�ڵ��Ӧ�Ķ�ά���������</param>
        public Node(NodeType nodeType, NodeBlueprint blueprint, Vector2Int point)
        {
            this.nodeType = nodeType;
            this.blueprint = blueprint;
            this.point = point;
        }

        // �� incoming �б������һ��ָ��ýڵ�������
        // p: Ҫ��ӵĶ�ά���������
        public void AddIncoming(Vector2Int p)
        {
            // ��� incoming �б����Ƿ��Ѿ����ڸ�����㣬������������
            if (incoming.Any(element => element.Equals(p)))
                return;

            // ��������ڣ��򽫸��������ӵ� incoming �б���
            incoming.Add(p);
        }

        // �� outgoing �б������һ���Ӹýڵ�����������
        // p: Ҫ��ӵĶ�ά���������
        public void AddOutgoing(Vector2Int p)
        {
            // ��� outgoing �б����Ƿ��Ѿ����ڸ�����㣬������������
            if (outgoing.Any(element => element.Equals(p)))
                return;

            // ��������ڣ��򽫸��������ӵ� outgoing �б���
            outgoing.Add(p);
        }

        // �� incoming �б����Ƴ�ָ���������
        // p: Ҫ�Ƴ��Ķ�ά���������
        public void RemoveIncoming(Vector2Int p)
        {
            // �Ƴ� incoming �б���������ָ���������ȵ�Ԫ��
            incoming.RemoveAll(element => element.Equals(p));
        }

        // �� outgoing �б����Ƴ�ָ���������
        // p: Ҫ�Ƴ��Ķ�ά���������
        public void RemoveOutgoing(Vector2Int p)
        {
            // �Ƴ� outgoing �б���������ָ���������ȵ�Ԫ��
            outgoing.RemoveAll(element => element.Equals(p));
        }

        // ���ýڵ��Ƿ�û���κ����ӣ��� incoming �� outgoing �б�Ϊ�գ�
        // ����ֵ: ���û�������򷵻� true�����򷵻� false
        public bool HasNoConnections()
        {
            return incoming.Count == 0 && outgoing.Count == 0;
        }
    }
}