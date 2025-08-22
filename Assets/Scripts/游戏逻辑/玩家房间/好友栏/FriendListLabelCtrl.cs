using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class FriendListLabelCtrl : MonoBehaviour
{
    public Transform labelParent;
    public GameObject labelObj;

    private void Awake()
    {
        labelObj = LoadManager.Instance.GetResourceByName<GameObject>("好友条（好友列表）");

    }
    private void Start()
    {
        NetManager.AddMsgListener("MsgGetFriendList", OnUpdateFriendList);
    }

    private void OnEnable()
    {
        //更新列表信息
        MsgGetFriendList msg = new MsgGetFriendList();
        NetManager.Send(msg);
    }

    public void OnUpdateFriendList(MsgBase msgBase)
    {
        MsgGetFriendList msg = (MsgGetFriendList)msgBase;

        for (int i = labelParent.childCount - 1; i >= 0; i--)
        {
            Destroy(labelParent.GetChild(i).gameObject);
        }
        foreach (var friend in msg.list)
        {
            var obj = Instantiate(labelObj, labelParent);

            obj.GetComponent<FriendBar>().Init(Color.gray, friend);
        }
    }

    private void OnDestroy()
    {
        NetManager.RemoveMsgListener("MsgGetFriendList", OnUpdateFriendList);
    }
}
