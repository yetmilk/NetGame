using DTT.Utils.Extensions;

public class MsgGetPlayerInfo : MsgBase
{
    public MsgGetPlayerInfo() { protoName = "MsgGetPlayerInfo"; }

    public string name;
    public string netID;
    public string character;

}

public class MsgUpdatePlayerClacify : MsgBase
{
    public MsgUpdatePlayerClacify() { protoName = "MsgUpdatePlayerClacify"; }

    public string name;
    public string character;
    public string questIp;
}
public class MsgDamageInfo : MsgBase
{
    public float damageValue;

    public string targetNetId;

    public string fromerNetId;

    public float DamageDirX;
    public float DamageDirY;
    public float DamageDirZ;

    public MsgDamageInfo(DamageInfo damageInfo)
    {
        protoName = "MsgDamageInfo";
        fromerNetId = damageInfo.fromerNetId;
        targetNetId = damageInfo.targetNetId;
        damageValue = damageInfo.damageValue;
        DamageDirX = damageInfo.DamageDir.x;
        DamageDirY = damageInfo.DamageDir.y;
        DamageDirZ = damageInfo.DamageDir.z;
    }

}

public class MsgUpdateDataObj : MsgBase
{
    public string netId;
    public MsgUpdateDataObj() { protoName = "MsgUpdateDataObj"; }
    public CharacterDataObj characterDataObj;
}
