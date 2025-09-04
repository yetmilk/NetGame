public class MsgTransScene : MsgBase
{
    public MsgTransScene() { protoName = "MsgTransScene"; }


    public string sceneName;


}

public class MsgLevelSelectInfo:MsgBase
{
    public MsgLevelSelectInfo() { protoName = "MsgLevelSelectInfo"; }

    public string fromIp;
    public int selectIndex;
    public bool isRequire;
}