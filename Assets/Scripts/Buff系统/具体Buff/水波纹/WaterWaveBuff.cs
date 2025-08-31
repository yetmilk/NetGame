using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWaveBuff : BuffObj
{
    public WaterWaveBuff(AddBuffInfo addBuffInfo) : base(addBuffInfo)
    {

    }

    public override void OnOccur(CharacterController target, CharacterController owner)
    {
        base.OnOccur(target, owner);
        target.curCharaData.GetDataObj().moveSpeed = Mathf.RoundToInt(target.curCharaData.GetDataObj().moveSpeed * module.propModList[0].value);

        DamageInfo damageInfo = new DamageInfo()
        {
            damageValue = DamageCaculateCollection.CaculateDamage(DamageFormulaType.Õ®”√, owner.curCharaData.GetDataObj(), target.curCharaData.GetDataObj()),
            DamageDir = (target.transform.position - owner.transform.position).normalized,
            fromerNetId = owner.NetID,
            targetNetId = target.NetID,
        };

        MsgDamageInfo msgDamageInfo = new MsgDamageInfo(damageInfo);

        NetManager.Send(msgDamageInfo);

    }

    public override void OnRemove(CharacterController target, CharacterController owner)
    {
        base.OnRemove(target, owner);
        target.curCharaData.GetDataObj().moveSpeed = Mathf.RoundToInt(target.curCharaData.GetDataObj().moveSpeed / module.propModList[0].value);
    }
}
