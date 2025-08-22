using System.Collections.Generic;
using UnityEngine;

public class MsgGetPlayer : MsgBase
{
    public MsgGetPlayer() { protoName = "MsgGetPlayer"; }

    public string id;

    public bool isfind;
    public bool isOnline;
}
//���ͺ��������Э��
public class MsgAddFriendQuest : MsgBase
{
    public MsgAddFriendQuest() { protoName = "MsgAddFriendQuest"; }

    //�ͻ��˷�
    public string questid = "";//�������������˵�id
    public string addPlayerid = "";//��Ҫ��ӵ��˵�id

}

//ȷ�Ϻ��������Э��
public class MsgConfirmFriendQuset : MsgBase
{
    public MsgConfirmFriendQuset() { protoName = "MsgConfirmFriendQuset"; }

    public string confirmid;//��Ӧ������������id
    public string askPlayerid;//������Ӻ��ѵ����id

    public bool isConfirm;//�Ƿ�ͬ���������
}

//��ú����б��Э��
public class MsgGetFriendList : MsgBase
{
    public MsgGetFriendList() { protoName = "MsgGetFriendList"; }


    //���صĺ����б�
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

