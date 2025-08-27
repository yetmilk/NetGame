using Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class EnemyLevelObj : BlueprintObj
{

    public RewardType rewardType;

    public ElementType elementType;


    public EnemyLevelObj(EnemyBlueprint enemyBlueprint, RewardType rewardType, ElementType elementType = ElementType.нч) : base(enemyBlueprint)
    {
        this.info = enemyBlueprint;
        this.rewardType = rewardType;
        if (elementType != ElementType.нч)
        {
            this.elementType = elementType;
        }
        else
        {
            var types = ((ElementType[])Enum.GetValues(typeof(ElementType))).ToList();
            types.Remove(ElementType.нч);
            elementType = types.Random();
        }

    }
}
