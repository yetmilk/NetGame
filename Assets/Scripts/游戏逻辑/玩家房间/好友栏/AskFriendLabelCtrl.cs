using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AskFriendLabelCtrl : MonoBehaviour
{
    public Transform labelParent;
    public GameObject labelObj;

    private void Awake()
    {
        labelObj = LoadManager.Instance.GetResourceByName<GameObject>("���������������룩");
    }
    private void Start()
    {
        NetManager.AddMsgListener("MsgAddFriendQuest", OnReceiveFriendQuest);
        NetManager.AddMsgListener("MsgConfirmFriendQuset", OnNoticeFriendQuest);
    }

    //�յ���������
    public void OnReceiveFriendQuest(MsgBase msgBase)
    {
        MsgAddFriendQuest msg = msgBase as MsgAddFriendQuest;

        //���º�������������
        var obj = Instantiate(labelObj, labelParent);
        obj.GetComponent<FriendBar>().Init(Color.green, msg.questid);

    }

    //�յ����պ�������
    public void OnNoticeFriendQuest(MsgBase msgBase)
    {
        MsgConfirmFriendQuset msg = msgBase as MsgConfirmFriendQuset;

        if (msg.isConfirm)
        {
            //TODO:���º����б�
        }
        else
        {
            //TODO:��Ϣϵͳ֪ͨ�Է��ܾ��˺�������
        }
    }
    private void OnDestroy()
    {
        NetManager.RemoveMsgListener("MsgAddFriendQuest", OnReceiveFriendQuest);
        NetManager.RemoveMsgListener("MsgConfirmFriendQuset", OnNoticeFriendQuest);
    }
}
