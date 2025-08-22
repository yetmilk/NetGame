
public class MsgInstantiateObj : MsgBase
{
    public MsgInstantiateObj() { protoName = "MsgInstantiateObj"; }

    public string questIp;//请求方的ip
    public string netId;//生成物的id标识
    public string insObjName;//生成物的名称
}

public class MsgDestroyObj : MsgBase
{
    public MsgDestroyObj() { protoName = "MsgDestroyObj"; }

    public string questIp;//请求方的ip
    public string netId;//删除物体的id标识
}
