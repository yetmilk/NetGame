
public class MsgInstantiateObj : MsgBase
{
    public MsgInstantiateObj() { protoName = "MsgInstantiateObj"; }

    public string questIp;//���󷽵�ip
    public string netId;//�������id��ʶ
    public string insObjName;//�����������
}

public class MsgDestroyObj : MsgBase
{
    public MsgDestroyObj() { protoName = "MsgDestroyObj"; }

    public string questIp;//���󷽵�ip
    public string netId;//ɾ�������id��ʶ
}
