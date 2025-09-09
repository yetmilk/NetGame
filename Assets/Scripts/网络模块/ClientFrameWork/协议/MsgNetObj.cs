
using UnityEngine;

public class MsgInstantiateObj : MsgBase
{
    public MsgInstantiateObj()
    {
        protoName = "MsgInstantiateObj";

    }

    public string questIp;//请求方的ip
    public bool useParent;
    public string parentNetId;//如果生成物有父物体，传此Id
    public string netId;//生成物的id标识
    public string insObjName;//生成物的名称


}

public class MsgDestroyObj : MsgBase
{
    public MsgDestroyObj() { protoName = "MsgDestroyObj"; }

    public string questIp;//请求方的ip
    public string netId;//删除物体的id标识
}
