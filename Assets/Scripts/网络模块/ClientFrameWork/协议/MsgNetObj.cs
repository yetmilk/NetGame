
using UnityEngine;

public class MsgInstantiateObj : MsgBase
{
    public MsgInstantiateObj()
    {
        protoName = "MsgInstantiateObj";

    }

    public string questIp;//���󷽵�ip
    public bool useParent;
    public string parentNetId;//����������и����壬����Id
    public string netId;//�������id��ʶ
    public string insObjName;//�����������


}

public class MsgDestroyObj : MsgBase
{
    public MsgDestroyObj() { protoName = "MsgDestroyObj"; }

    public string questIp;//���󷽵�ip
    public string netId;//ɾ�������id��ʶ
}
