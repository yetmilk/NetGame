using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearWaveSkillBuff : BuffObj
{
    public ClearWaveSkillBuff(AddBuffInfo buffInfo):base(buffInfo)
    {

    }

    public override void OnOccur(CharacterController target, CharacterController owner)
    {
        base.OnOccur(target, owner);
        target.curCharaData.GetDataObj().moveSpeed = Mathf.RoundToInt(target.curCharaData.GetDataObj().moveSpeed / module.propModList[0].value);
    }

    public override void OnRemove(CharacterController target, CharacterController owner)
    {
        base.OnRemove(target, owner);
        target.curCharaData.GetDataObj().moveSpeed = Mathf.RoundToInt(target.curCharaData.GetDataObj().moveSpeed *module.propModList[0].value);
    }
}
