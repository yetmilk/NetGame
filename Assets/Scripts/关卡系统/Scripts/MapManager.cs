
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace Map
{
    public class MapManager : Singleton<MapManager>
    {
        public List<MapConfig> configs;

        public EnemyRewardPropocityConfig enemyRewardPropocityConfig;

        Dictionary<int, Map> chapterToMap = new Dictionary<int, Map>();

        private readonly Progress progress = new Progress();

        public Progress curProgress = new Progress();

        public BlueprintObj curRoom;
        public Map CurrentMap { get; private set; }

        [System.Serializable]
        public class Progress
        {
            [Header("章节")]
            public int chapter;
            [Header("关卡")]
            public int level;

        }

        private void Start()
        {

            Init();

        }

        protected void Init()
        {

            //初始化章节
            for (int i = 0; i < configs.Count; i++)
            {
                Map map = GenerateNewChapter(i);

                chapterToMap.Add(i, map);
            }
            curProgress.chapter = 0;
            progress.chapter = chapterToMap.Count;
            progress.level = chapterToMap[curProgress.chapter].LayerCount;
            curProgress.level = -1;
            CurrentMap = chapterToMap[curProgress.chapter];

        }

        public Map GenerateNewChapter(int index)
        {
            Map map = MapGenerator.GetMap(Utility.ObjectCloner.Clone<MapConfig>(configs[index]));

            for (int i = 0; i < map.LayerCount; i++)
            {
                Node[] nodes = map.GetLayerNodes(i);
                var objs = InitRoom(nodes);
                for (int j = 0; j < objs.Length; j++)
                {
                    nodes[j].blueprintObj = objs[j];
                }
            }

            return map;
        }

        public void SaveMap()
        {
            if (CurrentMap == null) return;

            string json = JsonConvert.SerializeObject(CurrentMap, Formatting.Indented,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            PlayerPrefs.SetString("Map", json);
            PlayerPrefs.Save();
        }

        private void OnApplicationQuit()
        {
            SaveMap();
        }

        public void GoToNextLevel()
        {
           
            curProgress.level++;
            if (curProgress.level >= progress.level)
            {
                curProgress.level = 0;
                curProgress.chapter++;
                if (curProgress.chapter >= progress.chapter)
                {
                    ChapterEnd();
                    return;
                }

                CurrentMap = chapterToMap[curProgress.chapter];


            }
            TipManager.Instance.ShowTip(TipType.切换房间提示, "");




        }

        public void SetRoom(int index)
        {
            BlueprintObj obj = chapterToMap[curProgress.chapter].GetLayerNodes(curProgress.level)[index].blueprintObj;

            GameSceneManager.Instance.LoadSceneToServer(obj.info.scene, () =>
            {
                GameObject.FindFirstObjectByType<LevelController>().Init(obj);
                curRoom = obj;
            });
        }

        public void ChapterEnd()
        {
            Debug.Log("全部结束");
        }
        public BlueprintObj[] InitRoom(Node[] nodes)
        {
            if (nodes == null || nodes.Length == 0)
                return null;

            // 获取怪物奖励概率配置
            EnemyRewardPropocityConfig origin = LoadManager.Instance.GetResourceByName<EnemyRewardPropocityConfig>("怪物房奖励概率配置");
            EnemyRewardPropocityConfig rewardConfig = Utility.ObjectCloner.Clone<EnemyRewardPropocityConfig>(origin);
            if (rewardConfig == null || rewardConfig.proporityList == null || !rewardConfig.proporityList.Any())
            {
                Debug.LogError("未找到有效的敌人奖励概率配置");
                return null;
            }



            BlueprintObj[] blueprintObjs = new BlueprintObj[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                // 计算总概率权重
                int totalWeight = rewardConfig.proporityList.Sum(p => p.propority);
                BlueprintObj obj;
                switch (nodes[i].nodeType)
                {
                    case NodeType.普通怪房:

                    case NodeType.精英怪房:
                        // 尝试将节点蓝图转换为敌人蓝图
                        if (nodes[i].blueprint is EnemyBlueprint enemyBlueprint)
                        {
                            // 基于概率随机选择奖励类型
                            int randomValue = UnityEngine.Random.Range(0, totalWeight);
                            int cumulativeWeight = 0;
                            RewardType selectedRewardType = RewardType.无;

                            foreach (var prop in rewardConfig.proporityList)
                            {
                                cumulativeWeight += prop.propority;
                                selectedRewardType = prop.type;
                                if (randomValue < cumulativeWeight)
                                {
                                    //如果抽到的是秘籍，要确保当前房间与其他房间的元素类型不同
                                    if (prop.type == RewardType.秘籍)
                                    {
                                        var types = ((ElementType[])Enum.GetValues(typeof(ElementType))).ToList();
                                        types.Remove(ElementType.无);
                                        if (blueprintObjs != null)
                                        {
                                            foreach (var item in blueprintObjs)
                                            {
                                                if (item == null) continue;
                                                if (item.info.nodeType == NodeType.普通怪房 || item.info.nodeType == NodeType.精英怪房)
                                                {
                                                    EnemyLevelObj enemyLevelObj = (EnemyLevelObj)item;
                                                    if (enemyLevelObj.rewardType == RewardType.秘籍)
                                                    {
                                                        types.Remove(enemyLevelObj.elementType);
                                                    }
                                                }
                                            }
                                        }
                                        obj = new EnemyLevelObj(enemyBlueprint, selectedRewardType, types.Random());
                                        blueprintObjs[i] = obj;
                                    }
                                    else//其他奖励，概率都置为0
                                    {
                                        rewardConfig.proporityList.Find(p => p.type == prop.type).propority = 0;
                                        obj = new EnemyLevelObj(enemyBlueprint, selectedRewardType);
                                        blueprintObjs[i] = obj;
                                    }


                                    break;
                                }
                            }


                        }
                        break;
                    case NodeType.Boss房:
                    case NodeType.奇遇房:
                    case NodeType.商店房:
                    case NodeType.撤离房:
                        obj = new BlueprintObj(nodes[i].blueprint);
                        blueprintObjs[i] = obj;
                        break;
                    default:
                        break;
                }
            }


            return blueprintObjs;
        }
    }
}
