using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSwordBuff : BuffObj
{
    public IceSwordBuff(AddBuffInfo buffInfo) : base(buffInfo)
    {

    }

    public override void OnAttack(CharacterController target, CharacterController owner, ref DamageInfo damageInfo)
    {
        base.OnAttack(target, owner, ref damageInfo);
        if(damageInfo.attackTag == ActionTag.NormalAttack)
        {
            float random = Random.Range(0, 1);

            if (random <= module.propModList[0].value)
            {
                AddBuffInfo addBuffInfo = new AddBuffInfo(BuffName.¼õËÙ.ToString(), target, owner);

                MsgAddBuffObj msg = new MsgAddBuffObj(addBuffInfo);

                NetManager.Send(msg);
            }
        }
    }
}
