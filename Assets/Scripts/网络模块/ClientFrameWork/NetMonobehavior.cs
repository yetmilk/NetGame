using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetMonobehavior : MonoBehaviour
{
    [SerializeField] private string netId;

    public string NetID => netId;

    [SerializeField] private string netFlag;

    protected virtual void Start()
    {
        NetManager.AddMsgListener("MsgDestroyObj", OnDestroyFromServer);
      
    }

    /// <summary>
    /// 是否是本地
    /// </summary>
    public bool IsLocal => netFlag == "Local";


    public void SetId(string netId)
    {
        this.netId = netId;
    }
    public void SetFlag(string netFlag)
    {
        this.netFlag = netFlag;
    }

    public void ClientInit(string netId, string flag)
    {
        this.netId = netId;
        this.netFlag = flag;
        GameNetManager.Instance.SubbmitNetObject(netId, this);
    }



    /// <summary>
    /// 只有本地端可以调用，注意！！
    /// </summary>
    /// <param name="netId"></param>
    public void NetDestroy(string netId, GameObject localObj)
    {
        MsgDestroyObj msg = new MsgDestroyObj();
        msg.netId = netId;
        msg.questIp = NetManager.LocalEndPoint;
        NetManager.Send(msg);
        GameNetManager.Instance.UnSubmitNetObj(NetID);
        Destroy(localObj);
    }

    private void OnDestroyFromServer(MsgBase msgBase)
    {
        MsgDestroyObj msg = msgBase as MsgDestroyObj;

        if (msg.netId == netId && !IsLocal)
        {
            GameNetManager.Instance.UnSubmitNetObj(NetID);
            Destroy(gameObject);
        }

    }

    private void OnDestroy()
    {
        NetManager.RemoveMsgListener("MsgDestroyObj", OnDestroyFromServer);

        GameNetManager.Instance.UnSubmitNetObj(NetID);
    }
}
