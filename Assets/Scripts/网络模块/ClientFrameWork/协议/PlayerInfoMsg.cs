public class MsgGetPlayerInfo : MsgBase
{
    public MsgGetPlayerInfo() { protoName = "MsgGetPlayerInfo"; }

    public string name;
    public string netID;
    public string character;

}

public class MsgUpdatePlayerClacify : MsgBase
{
    public MsgUpdatePlayerClacify() { protoName = "MsgUpdatePlayerClacify"; }

    public string name;
    public string character;
    public string questIp;
}