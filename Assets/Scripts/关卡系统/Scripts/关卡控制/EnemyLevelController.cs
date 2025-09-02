using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLevelController : LevelController
{

    public List<MonsterSpawner> enemyIntantiates;
    public override void Init(BlueprintObj levelInfo)
    {
        base.Init(levelInfo);
    }
}
