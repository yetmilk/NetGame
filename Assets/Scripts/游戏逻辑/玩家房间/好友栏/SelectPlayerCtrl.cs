using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SelectPlayerCtrl : MonoBehaviour
{
    public TMP_InputField id_InputField;
    public Button selectBtn;

    public Transform labelParent;

    public GameObject labelObj;
    public GameObject nullTextObj;

    private void Awake()
    {

        labelObj = LoadManager.Instance.GetResourceByName<GameObject>("好友条（搜索条）");
        nullTextObj = LoadManager.Instance.GetResourceByName<GameObject>("搜索玩家失败提示文字");
    }
    private void Start()
    {
        NetManager.AddMsgListener("MsgGetPlayer", GetPlayer);

    }


    public void GetPlayer(MsgBase msgBase)
    {
        MsgGetPlayer msg = msgBase as MsgGetPlayer;
        for (int i = 0; i < labelParent.childCount; i++)
        {
            Destroy(labelParent.GetChild(i).gameObject);
        }
        if (msg.isfind)
        {
            var obj = Instantiate(labelObj, labelParent);
            Color onlineColor = msg.isOnline ? Color.green : Color.grey;
            obj.GetComponent<FriendBar>().Init(onlineColor, msg.id);
        }
        else
        {
            Instantiate(nullTextObj, labelParent);
        }
    }

    public void SelectPlayer()
    {
        MsgGetPlayer msg = new MsgGetPlayer();
        msg.id = id_InputField.text;
        NetManager.Send(msg);
    }
    private void OnDestroy()
    {
        NetManager.RemoveMsgListener("MsgGetPlayer", GetPlayer);
    }
}
