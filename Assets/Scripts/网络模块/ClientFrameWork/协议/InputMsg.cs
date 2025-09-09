using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MsgInputCommand : MsgBase
{
    public MsgInputCommand(int type, Vector3 direction)
    {
        protoName = "MsgInputCommand";
        this.type = type;

        directionX = direction.x;
        directionY = direction.y;
        directionZ = direction.z;

    }

    public MsgInputCommand() { protoName = "MsgInputCommand"; }

    public string NetId;

    public int type = -1;

    public string actionName;
    public int actionTag;

    public float directionX;
    public float directionY;
    public float directionZ;



}

public class MsgCmdCollection : MsgBase
{
    public MsgCmdCollection() { protoName = "MsgCmdCollection"; }

    public MsgInputCommand[] cmds;
}