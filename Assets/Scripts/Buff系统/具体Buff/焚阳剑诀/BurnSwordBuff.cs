using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnSwordBuff : BuffObj
{
    public BurnSwordBuff(AddBuffInfo buffInfo) : base(buffInfo)
    {

    }

    public override void OnAttack(CharacterController target, CharacterController owner, ref DamageInfo damageInfo)
    {
        base.OnAttack(target, owner, ref damageInfo);

        AddBuffInfo addBuffInfo = new AddBuffInfo(BuffName.×ÆÉÕ.ToString(), target, owner);

        MsgAddBuffObj msg = new MsgAddBuffObj(addBuffInfo);

        NetManager.Send(msg);
    }
}
