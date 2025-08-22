using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public string questIp;

    public int type;

    public float directionX;
    public float directionY;
    public float directionZ;



}