using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//注册
public class MsgRegister : MsgBase
{
    public MsgRegister() { protoName = "MsgRegister"; }

    //客户端发
    public string id = "";
    public string pw = "";

    //服务端回应（0-成功，1-失败）
    public int result = 0;
}

//登录
public class MsgLogin : MsgBase
{
    public MsgLogin() { protoName = "MsgLogin"; }

    //客户端发
    public string id = "";
    public string pw = "";

    //服务端回应（0-成功，1-失败）
    public int result = 0;
    public string netID = "";
}

//强制下线（服务端推送）
public class MsgKick : MsgBase
{
    public MsgKick() { protoName = "MsgKick"; }

    //原因（0-其他人登录同一账号）
    public int result = 0;
}
