using Map;

public class MsgTransScene : MsgBase
{
    public MsgTransScene() { protoName = "MsgTransScene"; }


    public string sceneName;


}

public class MsgLevelSelectInfo : MsgBase
{
    public MsgLevelSelectInfo() { protoName = "MsgLevelSelectInfo"; }

    public string fromIp;
    public int selectIndex;
    public bool isRequire;
}
public class MsgLevelOverInfo : MsgBase
{
    public MsgLevelOverInfo() { protoName = "MsgLevelOverInfo"; }

    public int[] nodetype;
    public int[] rewardType;
    public int[] elementType;

}

public class MsgMapInfo : MsgBase
{
    public MsgMapInfo() { protoName = "MsgMapInfo"; }

    public Map.Map map;
}