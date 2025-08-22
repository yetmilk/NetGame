using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgMove : MsgBase
{
    public MsgMove() { protoName = "MsgMove"; }

    public int x = 0;
    public int y = 0;
    public int z = 0;
}

public class MsgAttack : MsgBase
{
    public string desc = "";
}

public class MsgPing : MsgBase
{
    public MsgPing() { protoName = "MsgPing"; }

}

public class MsgPong : MsgBase
{
    public MsgPong() { protoName = "MsgPong"; }
}