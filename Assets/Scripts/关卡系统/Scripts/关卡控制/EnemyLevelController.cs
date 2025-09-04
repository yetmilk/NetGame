using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLevelController : LevelController
{

    public List<MonsterSpawner> enemyIntantiates;

    [System.Serializable]
    public class EnemyConfig
    {
        public int enemyNum;
        public List<MonsterSpawner> enemySpawns;
    }

    [Header("怪物波次及响应波次怪物的数量")]
    public List<EnemyConfig> enemyConfigs;

    private List<GameObject> spawnedMonsters = new List<GameObject>();  // 已生成的怪物

    int curTime;//当前波次

    public override void Init(BlueprintObj levelInfo)
    {
        base.Init(levelInfo);
        IntantiateEnemy();
    }

    public void IntantiateEnemy()
    {
        if (RoomManager.Instance.curRoom.hostId == PlayerManager.Instance.selfId)
        {
            // 检查当前波次是否在有效范围内
            if (curTime < 0 || curTime >= enemyConfigs.Count)
            {
                Debug.LogWarning("当前波次配置不存在");
                return;
            }

            EnemyConfig currentConfig = enemyConfigs[curTime];
            List<MonsterSpawner> spawnPoints = currentConfig.enemySpawns;

            // 检查生成点列表是否有效
            if (spawnPoints == null || spawnPoints.Count == 0)
            {
                Debug.LogWarning("当前波次没有配置生成点");
                return;
            }

            int totalEnemies = currentConfig.enemyNum;
            int spawnPointCount = spawnPoints.Count;

            // 计算每个生成点基础生成数量
            int baseCountPerSpawn = totalEnemies / spawnPointCount;
            // 计算剩余需要分配的怪物数量
            int remainingEnemies = totalEnemies % spawnPointCount;

            for (int i = 0; i < spawnPointCount; i++)
            {
                MonsterSpawner spawner = spawnPoints[i];
                if (spawner == null)
                {
                    Debug.LogWarning($"第{i}个生成点为空，跳过生成");
                    continue;
                }

                // 每个生成点至少生成baseCountPerSpawn个，前remainingEnemies个生成点多生成1个
                int enemiesToSpawn = baseCountPerSpawn + (i < remainingEnemies ? 1 : 0);

                // 生成对应数量的怪物

                var monsters = spawner.SpawnMonsters(enemiesToSpawn);
                if (monsters != null)
                {
                    spawnedMonsters.AddRange(monsters);
                }
                else
                {
                    Debug.LogWarning($"生成点 {spawner.name} 生成怪物失败");
                }

            }
        }
    }

    private void Update()
    {
        if (spawnedMonsters.Count <= 0 || spawnedMonsters == null) return;
        bool enemyAllDead = true;
        foreach (var item in spawnedMonsters)
        {
            if (item.GetComponent<CharacterController>().selfActionCtrl.curActionObj.curActionInfo.tag != ActionTag.Dead)
            {
                enemyAllDead = false; break;
            }
        }
        if (enemyAllDead)
        {
            ClearAllMonsters();
            curTime++;
            if (curTime >= enemyConfigs.Count)
            {
                //TODO:生成奖励
                LevelReward();
            }
            else
            {
                IntantiateEnemy();
            }
        }
    }
    public void LevelReward()
    {
        EnemyLevelObj enemyLevelObj = levelInfo as EnemyLevelObj;
        if (enemyLevelObj.rewardType == Map.RewardType.秘籍)
        {
            var rareBooks = LoadManager.Instance.GetResourcesByLabel<RareBook>("Rarebook");
            if (rareBooks != null)
            {
                RareBook rareBook = rareBooks.Random();

                BattleManager.Instance.FettersManager.AddRareBook(rareBook);
            }
        }

        BattleManager.Instance.GotoNextLevel();
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
