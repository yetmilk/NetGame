using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

[System.Serializable]
public class BlueprintObj
{
    public NodeBlueprint info;

    public BlueprintObj(NodeBlueprint info)
    {
        if (info is EnemyBlueprint enemyBlueprint)
        {
            this.info = ObjectCloner.Clone(enemyBlueprint);
        }
        else if (info is MysteriousBlueprint mysterious)
        {
            this.info = ObjectCloner.Clone(mysterious);
        }
        else if (info is StoreBlueprint storeBlueprint)
        {
            this.info = ObjectCloner.Clone(storeBlueprint);
        }
        else
        {
            this.info = info;
        }

    }
}
