using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class GameNetManager : Singleton<GameNetManager>
{
    private bool connectState = false;

    public GameObject logInPanel;
    public string ConnectIp = "127.0.0.1";


    public Dictionary<string, NetMonobehavior> netIdToObject = new Dictionary<string, NetMonobehavior>();
    private void Start()
    {
        DontDestroyOnLoad(this);
        NetManager.AddMsgListener("MsgRegister", OnRegister);
        NetManager.AddMsgListener("MsgLogin", OnLogin);
        NetManager.AddMsgListener("MsgKick", OnKick);
        NetManager.AddEventListener(NetEvent.ConnectSucc, OnConnectCallback);
        NetManager.AddEventListener(NetEvent.ConnectFail, OnConnectCallback);


        //47.105.122.120
        ConnectToServer();


    }

    #region-------------------������������-------------------------
    public void ConnectToServer()
    {
        TipManager.Instance.ShowTip(TipType.LogTip, "�������ӷ�����");
        StartCoroutine(WaitForCallback());

    }
    IEnumerator WaitForCallback()
    {
        yield return new WaitForSeconds(1f);

        while (!connectState)
        {
            NetManager.Connect(ConnectIp, 8888);
            yield return null;
        }
        TipManager.Instance.ShowTip(TipType.LogTip, "���������ӳɹ�", null, 1f);
        logInPanel.SetActive(true);
    }
    public void OnConnectCallback(string err)
    {
        if (err == "ConnectFail")
        {
            NetManager.Connect(ConnectIp, 8888);
        }
        else
        {
            connectState = true;


        }
    }

    #endregion


    public NetMonobehavior GetNetObject(string netId)
    {
        if(netIdToObject.ContainsKey(netId))
        {
            return netIdToObject[netId];
        }
        return null;
    }

    public void SubbmitNetObject(string netId,NetMonobehavior netObj)
    {
        if(!netIdToObject.ContainsKey(netId))
        {
            netIdToObject.Add(netId, netObj);
        }
        else
        {
            netIdToObject[netId] = netObj;
        }
    }

    public void UnSubmitNetObj(string netId)
    {
        if(netIdToObject.ContainsKey(netId))
        {
            netIdToObject.Remove(netId);
        }
    }


    public void OnRegister(MsgBase msgBase)
    {
        MsgRegister msg = msgBase as MsgRegister;
        if (msg.result == 0)
        {
            TipManager.Instance.ShowTip(TipType.LogTip, "ע��ɹ������Է��ص�¼ҳ���е�¼", null, 2f);
        }
        else
        {
            TipManager.Instance.ShowTip(TipType.LogTip, "ע��ʧ��", null, 2f);
            Debug.Log("ע��ʧ��");
        }
    }
    public void OnLogin(MsgBase msgBase)
    {
        MsgLogin msg = msgBase as MsgLogin;
        if (msg.result == 0)
        {
            TipManager.Instance.ShowTip(TipType.LogTip, "��¼�ɹ�", null, 2f);
            logInPanel.SetActive(false);
            Debug.Log("��¼�ɹ�");
            PlayerManager.Instance.selfId = msg.id;
            PlayerManager.Instance.InitPlayer(msg.id, msg.netID);

            //Action onCompleteFunc = null;
            //onCompleteFunc = () =>
            //{
            //    TipManager.Instance.CloseTip(TipType.LogTip);
            //    RoomManager.Instance.OnCreateRoomComplete -= onCompleteFunc;
            //};
            //RoomManager.Instance.CreateRoom(msg.id, new List<string>() { msg.id });
            //RoomManager.Instance.OnCreateRoomComplete += onCompleteFunc;
        }
        else
        {
            TipManager.Instance.ShowTip(TipType.LogTip, "��¼ʧ�ܣ������û���������", null, 2f);
        }
    }
    public void OnKick(MsgBase msgBase)
    {
        MsgKick msg = msgBase as MsgKick;
        if (msg.result == 0)
        {
            Debug.Log("��ص�¼��ǿ������");
        }

    }

    public static void SendMsg(MsgBase msg)
    {
        NetManager.Send(msg);
    }


    private void Update()
    {
        NetManager.Update();
    }
}
