using Map;
using RectEx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    public FettersManager FettersManager;
    public MapManager MapManager;
    public UIManager uiManager;

    public class levelInfo
    {
        public NodeType nodeType;
        public RewardType rewardType;
        public ElementType elementType;
    }
    private void Start()
    {
        NetManager.AddMsgListener("MsgLevelOverInfo", NoticeLevel);
    }

    public List<levelInfo> levelInfos = new List<levelInfo>();
    public void StarGame()
    {
        MapManager.SetRoom(0);

        foreach (var item in PlayerManager.Instance.curPlayerInfos)
        {
            var data = LoadManager.Instance.GetResourceByName<CharacterDataSO>(item.character.ToString());
            if (data != null)
            {
                item.characterData = new CharacterDataObj(data);
            }
        }
    }
    public void NoticeLevel(MsgBase msgBase)
    {
        MsgLevelOverInfo msg = msgBase as MsgLevelOverInfo;
        SetLevelInfo(msg.nodetype.Length, msg.nodetype, msg.rewardType, msg.elementType);
        var rareBooks = LoadManager.Instance.GetResourcesByLabel<RareBook>("Rarebook");
        if (rareBooks != null)
        {
            RareBook rareBook = rareBooks.Random();

            FettersManager.AddRareBook(rareBook);
        }
        GotoNextLevel();
    }
    public void GotoNextLevel()
    {
        MapManager.GoToNextLevel();
       
    }

    public List<levelInfo> GetNextLevelInfo()
    {
        return levelInfos;
    }

    public void SetLevelInfo(int count, int[] nodeType, int[] rewardType, int[] elementType)
    {
        levelInfos.Clear();
        for (int i = 0; i < count; i++)
        {
            levelInfo info = new levelInfo();
            info.nodeType = (NodeType)nodeType[i];
            info.elementType = (ElementType)elementType[i];
            info.rewardType = (RewardType)rewardType[i];
            levelInfos.Add(info);
        }
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        NetManager.RemoveMsgListener("MsgLevelOverInfo", NoticeLevel);
    }
}
