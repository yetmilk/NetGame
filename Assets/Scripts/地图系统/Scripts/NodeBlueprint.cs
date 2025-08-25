using UnityEngine;

namespace Map
{
    public enum NodeType
    {
        [InspectorName("普通怪")]
        MinorEnemy,
        [InspectorName("精英怪")]
        EliteEnemy,
        [InspectorName("休息处")]
        RestSite,
        [InspectorName("宝藏")]
        Treasure,
        [InspectorName("商店")]
        Store,
        [InspectorName("Boss")]
        Boss,
        [InspectorName("未知")]
        Mystery,
        [InspectorName("死神")]
        DiedPerson,
    }
}

namespace Map
{
    [CreateAssetMenu(fileName = "地图节点", menuName = "地图配置/地图节点")]
    public class NodeBlueprint : ScriptableObject
    {
        public Sprite sprite;
        public NodeType nodeType;
    }
}