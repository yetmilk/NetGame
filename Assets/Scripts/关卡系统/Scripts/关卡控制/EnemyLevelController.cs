using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLevelController : LevelController
{

    public List<MonsterSpawner> enemyIntantiates;

    public class EnemyConfig
    {
        public int enemyNum;
        public List<MonsterSpawner> enemySpawns;
    }

    [Header("怪物波次及响应波次怪物的数量")]
    public List<EnemyConfig> enemyConfigs;

    private List<GameObject> spawnedMonsters = new List<GameObject>();  // 已生成的怪物



    public override void Init(BlueprintObj levelInfo)
    {
        base.Init(levelInfo);
    }

    public void IntantiateEnemy()
    {
        if (RoomManager.Instance.curRoom.hostId == PlayerManager.Instance.selfId)
        {

        }
    }
    /// <summary>
    /// 清除所有生成的怪物
    /// </summary>
    public void ClearAllMonsters()
    {
        foreach (var monster in spawnedMonsters)
        {
            if (monster != null)
            {
                monster.GetComponent<CharacterController>().NetDestroy(monster.GetComponent<NetMonobehavior>().NetID, monster);
            }
        }
        spawnedMonsters.Clear();
    }
}
