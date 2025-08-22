using UnityEngine;
using UnityEngine.UI;

public class SelectPlayerBar : FriendBar
{
    public Button addFriendBtn;


    private void Start()
    {
        addFriendBtn.onClick.AddListener(AddFriendQuest);
    }

    public void AddFriendQuest()
    {
        MsgAddFriendQuest msg = new MsgAddFriendQuest();
        msg.questid = PlayerManager.Instance.selfId;
        msg.addPlayerid = idText.text;
        NetManager.Send(msg);
    }
}
