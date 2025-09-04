using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISkillCoolDownCtrl : UIElement
{
    public UiColldown skill1;
    public UiColldown skill2;
    public UiColldown skill3;


    private void Update()
    {
        skill1.UpdateSkill(dataObj.skill1ResumeTimer);
        skill2.UpdateSkill(dataObj.skill2ResumeTimer);
        skill3.UpdateSkill(dataObj.skill3ResumeTimer);
    }
}
