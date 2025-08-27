using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "商店房", menuName = "地图配置/商店房")]
public class StoreBlueprint : NodeBlueprint
{
    public StoreBlueprint()
    {
        nodeType = NodeType.商店房;
    }

}
