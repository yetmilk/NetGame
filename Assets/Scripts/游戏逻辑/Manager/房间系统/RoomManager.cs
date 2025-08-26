using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class RoomManager : Singleton<RoomManager>
{

    public List<RoomState> allOnlineRoom;
    public RoomState curRoom;
    /// <summary>
    /// 房间创建成功的回调
    /// </summary>
    public Action OnCreateRoomComplete;
    public Action OnRoomUpdate;

    public RoomUIController uiCtrl;

    public SceneName roomScene;

    /// <summary>
    /// 
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        NetManager.AddMsgListener("MsgCreateRoomCallback", CreateRoomCallbackFromServer);
        NetManager.AddMsgListener("MsgJoinRoomCallback", JoinOthersRoomCallback);
        NetManager.AddMsgListener("MsgAskJoinRoom", JoinQuestFromOther);
        NetManager.AddMsgListener("MsgQuitRoomCallback", QuitRoomCallback);
        NetManager.AddMsgListener("MsgUpdateRoom", UpdateRoom);
    }

    #region------------------创建房间相关逻辑--------------------

    /// <summary>
    /// 向服务端发送创建房间的请求
    /// </summary>
    /// <param name="hostId"></param>
    /// <param name="membersId"></param>
    public void CreateRoom(string hostId, List<string> membersId)
    {
        MsgCreateRoom msg = new MsgCreateRoom();
        msg.hostId = hostId;
        msg.membersId = membersId;

        NetManager.Send(msg);
    }

    /// <summary>
    /// 接收来自服务端创建房间请求的回调
    /// </summary>
    /// <param name="msgBase"></param>
    public void CreateRoomCallbackFromServer(MsgBase msgBase)
    {
        MsgCreateRoomCallback msg = msgBase as MsgCreateRoomCallback;
        if (msg.succCreate)
        {
            RoomState room = new RoomState(msg.roomId, msg.hostId, msg.membersId, msg.roomData);
            curRoom = room;

            LoadToRoomScene();

            OnCreateRoomComplete?.Invoke();
            //UI
            uiCtrl.SetRoomPanelActive(true);
            uiCtrl.SetHostId(msg.hostId);
            uiCtrl.UpdateRoomUI();
        }
        else
        {
            Debug.LogError("服务器未能成功创建房间，房间创建失败！！");
        }

    }

    #endregion

    #region--------------加入房间相关逻辑-----------------
    public void JoinOthersRoom(string hostId)
    {
        MsgJoinRoom msg = new MsgJoinRoom();
        msg.hostId = hostId;
        msg.membersId = PlayerManager.Instance.selfId;
        NetManager.Send(msg);
    }

    public void InviteOtherToRoom(string inviteId)
    {
        MsgInviteOtherToRoom msg = new MsgInviteOtherToRoom();
        msg.hostId = PlayerManager.Instance.selfId;
        msg.membersId = inviteId;

        NetManager.Send(msg);
    }

    private void JoinQuestFromOther(MsgBase msgBase)
    {
        isChange = false;
        MsgAskJoinRoom msg = msgBase as MsgAskJoinRoom;

        if (msg.isInvite)
        {
            TipManager.Instance.ShowTip(TipType.JoinRoomQuestTip, msg.hostId.ToString() + "邀请你加入房间");
        }
        else
            TipManager.Instance.ShowTip(TipType.JoinRoomQuestTip, msg.joinId.ToString() + "请求加入你的房间");

        StartCoroutine(WaitForJoinQuest(msg));
    }
    IEnumerator WaitForJoinQuest(MsgAskJoinRoom msg)
    {
        while (!isChange)
        {
            yield return null;
        }
        msg.accapt = isAccapt;
        NetManager.Send(msg);
        isChange = false;
    }

    bool isAccapt;
    bool isChange;
    public void ReplayJoinQuest(bool accapt)
    {
        isAccapt = accapt;
        isChange = true;
    }

    public void JoinOthersRoomCallback(MsgBase msgBase)
    {
        MsgJoinRoomCallback msg = msgBase as MsgJoinRoomCallback;
        if (msg.succJoin)
        {
            if (msg.addMemberId == PlayerManager.Instance.selfId)
            {
                curRoom = msg.roomState;
                OnRoomUpdate?.Invoke();
                TipManager.Instance.ShowTip(TipType.LogTip, "正在加入 " + msg.roomState.hostId.ToString() + " 的房间", null, 2f);

                LoadToRoomScene();

            }
            else
            {
                curRoom = msg.roomState;
                OnRoomUpdate?.Invoke();
                TipManager.Instance.ShowTip(TipType.LogTip, "玩家 " + msg.addMemberId.ToString() + " 加入房间", null, 2f);
            }

            //更新UI
            uiCtrl.SetHostId(curRoom.hostId);
            uiCtrl.UpdateRoomUI();
        }
        else
        {
            Debug.Log("加入失败");
            TipManager.Instance.ShowTip(TipType.LogTip, msg.reason.ToString(), null, 1f);
        }


    }

    #endregion

    #region-----------退出房间相关逻辑-------------
    public void QuitRoom(string hostId)
    {
        MsgQuitRoom msg = new MsgQuitRoom();
        msg.hostId = hostId;
        msg.membersId = PlayerManager.Instance.selfId;

        NetManager.Send(msg);
    }

    public void QuitRoomCallback(MsgBase msgBase)
    {
        MsgQuitRoomCallback msg = msgBase as MsgQuitRoomCallback;

        if (PlayerManager.Instance.selfId != msg.membersId)
        {
            if ((PlayerManager.Instance.curActivePlayer.ContainsKey(msg.membersId)))
            {

                TipManager.Instance.ShowTip(TipType.LogTip, msg.membersId.ToString() + "退出房间", null, 1f);
                curRoom.roomMembers.Remove(msg.membersId);
                PlayerManager.Instance.DeletePlayer(msg.membersId);

                uiCtrl.UpdateRoomUI();
            }
        }
        else
        {
            TipManager.Instance.ShowTip(TipType.LogTip, "正在退出房间", null, 1f);
            MsgGetRoom msgGetRoom = new MsgGetRoom();
            msgGetRoom.checkId = msg.membersId;
            NetManager.Send(msgGetRoom);
        }

    }
    #endregion

    #region---------房间更新-------------
    public void UpdateRoom(MsgBase msgBase)
    {
        MsgUpdateRoom msg = msgBase as MsgUpdateRoom;

        curRoom = msg.roomState;
        OnRoomUpdate?.Invoke();
        uiCtrl.UpdateRoomUI();
    }
    #endregion

    public void LoadToRoomScene()
    {

        GameSceneManager.Instance.LoadSceneToServer(roomScene);
    }

    protected override void OnDestroy()
    {
        NetManager.RemoveMsgListener("MsgCreateRoomCallback", CreateRoomCallbackFromServer);
        NetManager.RemoveMsgListener("MsgJoinRoomCallback", JoinOthersRoomCallback);
        NetManager.RemoveMsgListener("MsgAskJoinRoom", JoinQuestFromOther);
        NetManager.RemoveMsgListener("MsgQuitRoomCallback", QuitRoomCallback);
        NetManager.RemoveMsgListener("MsgUpdateRoom", UpdateRoom);
    }

}
[System.Serializable]
public class RoomState
{
    public string roomId;

    public string hostId;

    //房间成员列表
    public List<string> roomMembers = new List<string>();

    /// <summary>
    /// 房间成员数
    /// </summary>
    public int RoommateCount { get { return roomMembers.Count; } }

    //更多数据............
    public RoomData data;

    public RoomState(string roomId, string host, List<string> members, RoomData data)
    {
        this.roomId = roomId;
        this.hostId = host;
        this.roomMembers = members;
        this.data = data;
    }

}
[System.Serializable]
public class RoomData
{
    string roomid = "00";
}
