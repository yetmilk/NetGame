using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ͻ���������������������Э��
/// </summary>
public class MsgCreateRoom : MsgBase
{
    public MsgCreateRoom() { protoName = "MsgCreateRoom"; }

    public string hostId;

    public List<string> membersId;

}
/// <summary>
/// �������ͻ��˷��͵Ļص�
/// </summary>
public class MsgCreateRoomCallback : MsgBase
{
    public MsgCreateRoomCallback(bool succCreate, string roomId = "", string hostId = "")
    {
        protoName = "MsgCreateRoomCallback";
        this.succCreate = succCreate;
        this.roomId = roomId;
        this.hostId = hostId;
    }

    public bool succCreate;
    public string roomId;
    public string hostId;
    public List<string> membersId;
    public RoomData roomData;

}
/// <summary>
/// �ͻ����������ĳ�����Э��
/// </summary>
public class MsgJoinRoom : MsgBase
{
    public MsgJoinRoom() { protoName = "MsgJoinRoom"; }

    public string hostId;
    public string membersId;
}
/// <summary>
/// �ͻ������������ͻ��˵ļ��뷿��Э��
/// </summary>
public class MsgInviteOtherToRoom : MsgBase
{
    public MsgInviteOtherToRoom() { protoName = "MsgInviteOtherToRoom"; }

    public string hostId;
    public string membersId;
}
/// <summary>
/// �������ͻ��˷��͵Ļص�
/// </summary>
public class MsgJoinRoomCallback : MsgBase
{
    public bool succJoin;
    public string addMemberId;
    public RoomState roomState;
    public string reason;
    public MsgJoinRoomCallback()
    {
        protoName = "MsgJoinRoomCallback";
    }
}
public class MsgAskJoinRoom : MsgBase
{
    public MsgAskJoinRoom() { protoName = "MsgAskJoinRoom"; }
    public string hostId;
    public string joinId;
    public bool isInvite;//�Ƿ�������

    //
    public bool accapt;
}
/// <summary>
/// �ͻ��������˳������Э��
/// </summary>
public class MsgQuitRoom : MsgBase
{
    public MsgQuitRoom() { protoName = "MsgQuitRoom"; }

    public string hostId;
    public string membersId;

    public bool isKick;
}
public class MsgQuitRoomCallback : MsgBase
{
    public string hostId;
    public string membersId;
    public bool isKick;

    public MsgQuitRoomCallback(string hostId, string membersId)
    {
        protoName = "MsgQuitRoomCallback";
        this.hostId = hostId;
        this.membersId = membersId;
    }


}
public class MsgUpdateRoom : MsgBase
{
    public MsgUpdateRoom() { protoName = "MsgUpdateRoom"; }

    public RoomState roomState;
}

public class MsgGetRoom : MsgBase
{
    public MsgGetRoom() { protoName = "MsgGetRoom"; }

    public string checkId;
}
