using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnBuff : BuffObj
{

    public BurnBuff(AddBuffInfo buffInfo) : base(buffInfo)
    {

    }

    #region---------------ÖØÐ´------------
    NetMonobehavior vfx;
    public override void OnOccur(CharacterController target, CharacterController owner)
    {
        var go = LoadManager.Instance.NetInstantiate("VFX_»ðÑæ", target.transform, target.NetID, true);
        vfx = go.GetComponent<NetMonobehavior>();
    }

    public override void OnRemove(CharacterController target, CharacterController owner)
    {
        vfx.NetDestroy(vfx.NetID, vfx.gameObject);
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
