using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AskFriendLabelCtrl : MonoBehaviour
{
    public Transform labelParent;
    public GameObject labelObj;

    private void Awake()
    {
        labelObj = LoadManager.Instance.GetResourceByName<GameObject>("好友条（好友申请）");
    }
    private void Start()
    {
        NetManager.AddMsgListener("MsgAddFriendQuest", OnReceiveFriendQuest);
        NetManager.AddMsgListener("MsgConfirmFriendQuset", OnNoticeFriendQuest);
    }

    //收到好友请求
    public void OnReceiveFriendQuest(MsgBase msgBase)
    {
        MsgAddFriendQuest msg = msgBase as MsgAddFriendQuest;

        //更新好友申请栏内容
        var obj = Instantiate(labelObj, labelParent);
        obj.GetComponent<FriendBar>().Init(Color.green, msg.questid);

    }

    //收到接收好友请求
    public void OnNoticeFriendQuest(MsgBase msgBase)
    {
        MsgConfirmFriendQuset msg = msgBase as MsgConfirmFriendQuset;

        if (msg.isConfirm)
        {
            //TODO:更新好友列表
        }
        else
        {
            //TODO:消息系统通知对方拒绝了好友申请
        }
    }
    private void OnDestroy()
    {
        NetManager.RemoveMsgListener("MsgAddFriendQuest", OnReceiveFriendQuest);
        NetManager.RemoveMsgListener("MsgConfirmFriendQuset", OnNoticeFriendQuest);
    }
}
