using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//ע��
public class MsgRegister : MsgBase
{
    public MsgRegister() { protoName = "MsgRegister"; }

    //�ͻ��˷�
    public string id = "";
    public string pw = "";

    //����˻�Ӧ��0-�ɹ���1-ʧ�ܣ�
    public int result = 0;
}

//��¼
public class MsgLogin : MsgBase
{
    public MsgLogin() { protoName = "MsgLogin"; }

    //�ͻ��˷�
    public string id = "";
    public string pw = "";

    //����˻�Ӧ��0-�ɹ���1-ʧ�ܣ�
    public int result = 0;
    public string netID = "";
}

//ǿ�����ߣ���������ͣ�
public class MsgKick : MsgBase
{
    public MsgKick() { protoName = "MsgKick"; }

    //ԭ��0-�����˵�¼ͬһ�˺ţ�
    public int result = 0;
}
