using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendStateBar : FriendBar
{
    public Button joinRoomBtn;
    public Button chatBtn;
    public Button deleteFriendBtn;
    public Button inviteBtn;

    private void Start()
    {
        NetManager.AddMsgListener("MsgCheckPlayerOnline", GetOnline);

        chatBtn.onClick.AddListener(Chat);
        deleteFriendBtn.onClick.AddListener(DeleteFriend);
        joinRoomBtn.onClick.AddListener(JoinRoom);
        inviteBtn.onClick.AddListener(InviteRoom);
        onlineImg = transform.GetChild(1).GetComponent<Image>();

        StartCoroutine(CheckOnlineLoop());


    }

    IEnumerator CheckOnlineLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            CheckOnline();
        }
    }

    public void JoinRoom()
    {
        RoomManager.Instance.JoinOthersRoom(idText.text.ToString());
    }

    public void InviteRoom()
    {
        RoomManager.Instance.InviteOtherToRoom(idText.text.ToString());
    }
    public void CheckOnline()
    {
        MsgCheckPlayerOnline msg = new MsgCheckPlayerOnline();
        msg.checkId = idText.text;
        NetManager.Send(msg);
    }

    public void GetOnline(MsgBase msgBase)
    {
        MsgCheckPlayerOnline msg = msgBase as MsgCheckPlayerOnline;
        Color color = msg.isOnline ? Color.green : Color.gray;
        onlineImg.color = color;
    }

    public void Chat()
    {
        string[] findid = { idText.text };
        foreach (var item in ChatManager.instance.logidToLogsDic.Values)
        {
            if (item.IsRoleInChatLogMembers(findid, item.chatLogMembers.ToArray()))
            {
                ChatManager.instance.ChangeLog(item.logID, idText.text);
                return;
            }
        }

        List<string> lis = new List<string>();
        lis.Add(PlayerManager.Instance.selfId);
        lis.Add(idText.text);
        ChatManager.instance.CreateNewLog(idText.text, lis);

    }

    public void DeleteFriend()
    {
        MsgDeleteFriend msg = new MsgDeleteFriend();
        msg.deleteId = idText.text;
        NetManager.Send(msg);
    }
    private void OnDestroy()
    {
        NetManager.RemoveMsgListener("MsgCheckPlayerOnline", GetOnline);
    }
}
