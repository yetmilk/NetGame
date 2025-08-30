using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnBuff : BuffObj
{

    public BurnBuff(AddBuffInfo buffInfo) : base(buffInfo)
    {

    }

    #region---------------ÖØÐ´------------
    public override void OnOccur(CharacterController target, CharacterController owner)
    {


    }

    public override void OnRemove(CharacterController target, CharacterController owner)
    {

    }

    public override void OnTimeTick(CharacterController target, CharacterController owner)
    {
        DamageInfo damageInfo = new DamageInfo()
        {
            damageValue = 5f,
            fromerNetId = owner.NetID,
            targetNetId = target.NetID,

        };
        MsgDamageInfo msg = new MsgDamageInfo(damageInfo);
        NetManager.Send(msg);
    }
    #endregion
}
