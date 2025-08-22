using System.Collections;

using System.Numerics;

public class MsgPosition : MsgBase
{
    public MsgPosition() { protoName = "MsgPosition"; }

    public string NetId;
    public string questIp;
    public float positionX;
    public float positionY;
    public float positionZ;
}

public class MsgRotation : MsgBase
{
    public MsgRotation() { protoName = "MsgRotation"; }

    public string NetId;
    public string questIp;
    public float rotationX;
    public float rotationY;
    public float rotationZ;
    public float rotationW;
}
