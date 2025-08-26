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
    /// ���䴴���ɹ��Ļص�
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

    #region------------------������������߼�--------------------

    /// <summary>
    /// �����˷��ʹ������������
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
    /// �������Է���˴�����������Ļص�
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
            Debug.LogError("������δ�ܳɹ��������䣬���䴴��ʧ�ܣ���");
        }

    }

    #endregion

    #region--------------���뷿������߼�-----------------
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
            TipManager.Instance.ShowTip(TipType.JoinRoomQuestTip, msg.hostId.ToString() + "��������뷿��");
        }
        else
            TipManager.Instance.ShowTip(TipType.JoinRoomQuestTip, msg.joinId.ToString() + "���������ķ���");

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
                TipManager.Instance.ShowTip(TipType.LogTip, "���ڼ��� " + msg.roomState.hostId.ToString() + " �ķ���", null, 2f);

                LoadToRoomScene();

            }
            else
            {
                curRoom = msg.roomState;
                OnRoomUpdate?.Invoke();
                TipManager.Instance.ShowTip(TipType.LogTip, "��� " + msg.addMemberId.ToString() + " ���뷿��", null, 2f);
            }

            //����UI
            uiCtrl.SetHostId(curRoom.hostId);
            uiCtrl.UpdateRoomUI();
        }
        else
        {
            Debug.Log("����ʧ��");
            TipManager.Instance.ShowTip(TipType.LogTip, msg.reason.ToString(), null, 1f);
        }


    }

    #endregion

    #region-----------�˳���������߼�-------------
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

                TipManager.Instance.ShowTip(TipType.LogTip, msg.membersId.ToString() + "�˳�����", null, 1f);
                curRoom.roomMembers.Remove(msg.membersId);
                PlayerManager.Instance.DeletePlayer(msg.membersId);

                uiCtrl.UpdateRoomUI();
            }
        }
        else
        {
            TipManager.Instance.ShowTip(TipType.LogTip, "�����˳�����", null, 1f);
            MsgGetRoom msgGetRoom = new MsgGetRoom();
            msgGetRoom.checkId = msg.membersId;
            NetManager.Send(msgGetRoom);
        }

    }
    #endregion

    #region---------�������-------------
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

    //�����Ա�б�
    public List<string> roomMembers = new List<string>();

    /// <summary>
    /// �����Ա��
    /// </summary>
    public int RoommateCount { get { return roomMembers.Count; } }

    //��������............
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
