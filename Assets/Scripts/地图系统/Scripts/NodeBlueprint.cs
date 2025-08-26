using UnityEngine;

namespace Map
{
    public enum NodeType
    {

        普通怪房,

        精英怪房,

        奇遇房,

        Boss房,

        商店房,

        撤离房,
    }
}

namespace Map
{
    [CreateAssetMenu(fileName = "地图节点", menuName = "地图配置/地图节点")]
    public class NodeBlueprint : ScriptableObject
    {
        [Header("该节点传送的场景")]
        public SceneName scene;




        public NodeType nodeType;
    }
}