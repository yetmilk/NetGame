using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanWaterBuff : BuffObj
{

    int timer;
    public CleanWaterBuff(AddBuffInfo addBuffInfo) : base(addBuffInfo)
    {
        addBuffInfo.target.selfActionCtrl.OnActionEnter += SkillEnter;
    }


    private void SkillEnter(ActionTag tag, ActionObj obj)
    {
        if (tag == ActionTag.Skill1 ||
           tag == ActionTag.Skill2 ||
           tag == ActionTag.Skill3)
        {
            timer++;
        }
        if (timer == 3)
        {
            owner.curCharaData.GetDataObj().curHealth += Mathf.RoundToInt(owner.curCharaData.GetDataObj().curHealth * module.propModList[0].value);
        }
    }

    public override void Disable()
    {
        base.Disable();
        target.selfActionCtrl.OnActionEnter -= SkillEnter;
    }
}
