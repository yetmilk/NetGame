using Map;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectController : MonoBehaviour, ITip
{
    public Transform insParent;

    public int selectIndex = -1;

    public class SelectInfo
    {
        public string fromIp;
        public int selectIndex;
        public bool isRequire;
    }

    public bool isLock = false;

    public List<LevelSelectObj> selectList = new List<LevelSelectObj>();

    public Button requireBtn;

    public List<SelectInfo> selectInfos = new List<SelectInfo>();

    private void Start()
    {
        requireBtn.onClick.AddListener(() =>
        {
            if (isLock)
            {
                requireBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "锁定";
                isLock = false;
            }
            else
            {
                requireBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "取消锁定";
                SetIndex(selectIndex, true);
            }
        });

        NetManager.AddMsgListener("MsgLevelSelectInfo", SetIndexFromServer);
    }

    private void Update()
    {
        if (PlayerManager.Instance.selfId != RoomManager.Instance.curRoom.hostId) return;
        if (selectInfos.Count == 0) return;
        bool checkSucc = true;
        int index = selectInfos[0].selectIndex;
        foreach (var item in selectInfos)
        {
            if (index != item.selectIndex || !item.isRequire)
            {
                checkSucc = false;
                return;
            }
        }

        if (checkSucc)
        {
            BattleManager.Instance.MapManager.SetRoom(index);
            TipManager.Instance.CloseTip(TipType.切换房间提示);
        }
    }
    public void Close()
    {
        selectInfos.Clear();
        selectList.Clear();
        isLock = false;
        requireBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "锁定";
    }

    public void Show()
    {
        for (int i = 0; i < insParent.childCount; i++)
        {
            Destroy(insParent.GetChild(i).gameObject);
        }
        var infos = BattleManager.Instance.MapManager.CurrentMap.GetLayerNodes(BattleManager.Instance.MapManager.curProgress.level);
        var obj = LoadManager.Instance.GetResourceByName<GameObject>("关卡选择槽");

        for (int i = 0; i < infos.Length; i++)
        {
            var item = Instantiate(obj, insParent);

            LevelSelectObj selectObj = item.GetComponent<LevelSelectObj>();
            selectList.Add(selectObj);
            if (infos[i].blueprintObj is EnemyLevelObj enemyObj)
            {
                selectObj.Init(this, i, infos[i].nodeType.ToString(), enemyObj.rewardType.ToString(), enemyObj.elementType.ToString());
            }
            else
               selectObj.Init(this, i, infos[i].nodeType.ToString());

        }
    }

    public void SetIndexFromServer(MsgBase msgBase)
    {
        MsgLevelSelectInfo msg = msgBase as MsgLevelSelectInfo;

        SelectInfo selectInfo = new SelectInfo()
        {
            selectIndex = msg.selectIndex,
            fromIp = msg.fromIp,
            isRequire = msg.isRequire,
        };

        var curselect = selectInfos.Find((a) =>
        {
            if (a.fromIp == msg.fromIp)
            {
                return true;
            }
            return false;
        });
        if (curselect == null)
        {
            selectInfos.Add(selectInfo);
        }
        else
        {
            curselect.selectIndex = msg.selectIndex;
            curselect.isRequire = msg.isRequire;
        }

        for (int i = 0; i < selectList.Count; i++)
        {
            selectList[i].selectObj.SetActive(false);
        }
        foreach (var item in selectInfos)
        {
            selectList[item.selectIndex].selectObj.SetActive(true);
        }
    }

    public void SetIndex(int index, bool isRequire = false)
    {
        if (isLock) return;
        selectIndex = index;
        isLock = isRequire;

        MsgLevelSelectInfo msg = new MsgLevelSelectInfo();

        msg.selectIndex = index;
        msg.isRequire = isRequire;

        NetManager.Send(msg);
    }
}
