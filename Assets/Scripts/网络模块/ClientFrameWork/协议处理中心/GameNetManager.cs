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

    #region-------------------连接至服务器-------------------------
    public void ConnectToServer()
    {
        TipManager.Instance.ShowTip(TipType.LogTip, "正在连接服务器");
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
        TipManager.Instance.ShowTip(TipType.LogTip, "服务器连接成功", null, 1f);
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
            TipManager.Instance.ShowTip(TipType.LogTip, "注册成功，可以返回登录页进行登录", null, 2f);
        }
        else
        {
            TipManager.Instance.ShowTip(TipType.LogTip, "注册失败", null, 2f);
            Debug.Log("注册失败");
        }
    }
    public void OnLogin(MsgBase msgBase)
    {
        MsgLogin msg = msgBase as MsgLogin;
        if (msg.result == 0)
        {
            TipManager.Instance.ShowTip(TipType.LogTip, "登录成功", null, 2f);
            logInPanel.SetActive(false);
            Debug.Log("登录成功");
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
            TipManager.Instance.ShowTip(TipType.LogTip, "登录失败，请检查用户名或密码", null, 2f);
        }
    }
    public void OnKick(MsgBase msgBase)
    {
        MsgKick msg = msgBase as MsgKick;
        if (msg.result == 0)
        {
            Debug.Log("异地登录，强迫下线");
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
