using UnityEngine;
using UnityEngine.UI;

public class AskFriendQuestBar : FriendBar
{
    public Button confirmAddFriendBtn;
    public Button refuseBtn;

    private void Start()
    {
        confirmAddFriendBtn.onClick.AddListener(OnConfirm);
        refuseBtn.onClick.AddListener(OnRefuse);
    }

    public void OnConfirm()
    {
        MsgConfirmFriendQuset msg = new MsgConfirmFriendQuset();
        msg.isConfirm = true;
        msg.askPlayerid = idText.text;
        msg.confirmid = PlayerManager.Instance.selfId;
        NetManager.Send(msg);

        Destroy(gameObject, .5f);
    }

    public void OnRefuse()
    {
        MsgConfirmFriendQuset msg = new MsgConfirmFriendQuset();
        msg.isConfirm = false;
        msg.askPlayerid = idText.text;
        msg.confirmid = PlayerManager.Instance.selfId;
        NetManager.Send(msg);
        Destroy(gameObject, .5f);
    }
}
