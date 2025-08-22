using System.Collections.Generic;
using UnityEngine;

public class MsgGetPlayer : MsgBase
{
    public MsgGetPlayer() { protoName = "MsgGetPlayer"; }

    public string id;

    public bool isfind;
    public bool isOnline;
}
//发送好友请求的协议
public class MsgAddFriendQuest : MsgBase
{
    public MsgAddFriendQuest() { protoName = "MsgAddFriendQuest"; }

    //客户端发
    public string questid = "";//发起好友请求的人的id
    public string addPlayerid = "";//想要添加的人的id

}

//确认好友请求的协议
public class MsgConfirmFriendQuset : MsgBase
{
    public MsgConfirmFriendQuset() { protoName = "MsgConfirmFriendQuset"; }

    public string confirmid;//回应好友请求的玩家id
    public string askPlayerid;//发起添加好友的玩家id

    public bool isConfirm;//是否同意好友申请
}

//获得好友列表的协议
public class MsgGetFriendList : MsgBase
{
    public MsgGetFriendList() { protoName = "MsgGetFriendList"; }


    //返回的好友列表
    public List<string> list = new List<string>();
}

public class MsgCheckPlayerOnline : MsgBase
{
    public MsgCheckPlayerOnline() { protoName = "MsgCheckPlayerOnline"; }

    public string checkId;
    public bool isOnline;
}
public class MsgDeleteFriend : MsgBase
{
    public MsgDeleteFriend() { protoName = "MsgDeleteFriend"; }

    public string deleteId;

}

