using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInfo
{
    public float damageValue;

    public string targetNetId;

    public string fromerNetId;

    public Vector3 DamageDir;

    public DamageInfo(MsgDamageInfo msgDamageInfo)
    {
        damageValue = msgDamageInfo.damageValue;
        targetNetId = msgDamageInfo.targetNetId;
        fromerNetId = msgDamageInfo.fromerNetId;
        DamageDir = new Vector3(msgDamageInfo.DamageDirX, msgDamageInfo.DamageDirY, msgDamageInfo.DamageDirZ);
    }

    public DamageInfo() { }

}
