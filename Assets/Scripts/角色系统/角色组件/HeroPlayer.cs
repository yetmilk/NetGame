using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPlayer : CharacterController
{


    #region---------------重写技能一起势------------------
    private GameObject skill_1_IndectorObj;
    protected override void OnSkill_1_Start_Enter(ActionObj curActionObj)
    {
        if (IsLocal)
        {
            var obj = LoadManager.Instance.GetResourceByName<GameObject>("侠客_技能一_线条指示器");
            skill_1_IndectorObj = Instantiate(obj, transform);

            skill_1_IndectorObj.transform.localPosition = Vector3.up * .5f;
        }
    }
    protected override void OnSkill_1_Start_Update(ActionObj curActionObj)
    {

        if (IsLocal)
        {
            bool checkSucc;
            Vector3 MousePos = PlayerInputManager.Instance.GetMouseDirection(out checkSucc);
            if (checkSucc)
            {
                Vector3 dir = (MousePos - transform.position).normalized;
                PlayerInputManager.Instance.HandleInput(new InputCommand(InputCommandType.技能1_起势, dir), NetID);
            }

        }
    }

    protected override void OnSkill_1_Start_Exit(ActionObj curActionObj, ActionObj nextActionObj)
    {
        if (IsLocal)
        {
            if (skill_1_IndectorObj != null)
            {
                Destroy(skill_1_IndectorObj);
            }
        }

        if (nextActionObj.curActionInfo.tag == ActionTag.Skill1)
            nextActionObj.direction = curActionObj.direction;
    }

    #endregion
    #region---------------重写技能一------------------
    private GameObject skill_1_Vfx;
    protected override void OnSkill_1_Enter(ActionObj curActionObj)
    {
        if (IsLocal)
        {
            skill_1_Vfx = LoadManager.Instance.NetInstantiate("侠客技能1", transform);

            skill_1_Vfx.transform.position = transform.position + transform.forward * 1.5f;
        }
    }
    protected override void OnSkill_1_Update(ActionObj curActionObj)
    {
    }

    protected override void OnSkill_1_Exit(ActionObj curActionObj, ActionObj nextActionObj)
    {
        if ((IsLocal))
        {
            if (skill_1_Vfx != null)
                NetDestroy(skill_1_Vfx.GetComponent<NetMonobehavior>().NetID, skill_1_Vfx);
        }

    }
    #endregion

    #region---------------重写技能二起势------------------
    private GameObject skill_2_IndictorObj;
    protected override void OnSkill_2_Start_Enter(ActionObj curActionObj)
    {
        if (IsLocal)//判断是否是本端
        {
            var obj = LoadManager.Instance.GetResourceByName<GameObject>("侠客_技能二_圆形技能指示器");
            skill_2_IndictorObj = Instantiate(obj, transform);
            skill_2_IndictorObj.transform.localPosition = Vector3.up * .5f;
        }

    }

    protected override void OnSkill_2_Start_Update(ActionObj curActionObj)
    {
        if (IsLocal)
        {
            bool checkSucc;
            Vector3 MousePos = PlayerInputManager.Instance.GetMouseDirection(out checkSucc);
            if (checkSucc)
            {
                Vector3 dir = (MousePos - transform.position).normalized;
                PlayerInputManager.Instance.HandleInput(new InputCommand(InputCommandType.技能2_起势, dir), NetID);
            }

        }
    }

    protected override void OnSkill_2_Start_Exit(ActionObj curActionObj, ActionObj nextActionObj)
    {
        if (IsLocal)
        {
            if (skill_2_IndictorObj != null)
                Destroy(skill_2_IndictorObj);
        }

        if (nextActionObj.curActionInfo.tag == ActionTag.Skill2)
            nextActionObj.direction = curActionObj.direction;
    }
    #endregion

    #region-----------------重写技能二------------------------
    private GameObject skill_2_Vfx;
    protected override void OnSkill_2_Enter(ActionObj curActionObj)
    {



    }
    protected override void OnSkill_2_Update(ActionObj curActionObj)
    {
        if (IsLocal)
        {
            if (curActionObj.curLifeFrame == 40)
            {
                skill_2_Vfx = LoadManager.Instance.NetInstantiate("VFX_侠客技能2", transform);
                skill_2_Vfx.transform.parent = transform;
                skill_2_Vfx.GetComponent<IAttackDetector>().Init(this);
                skill_2_Vfx.transform.position = transform.position;
            }
        }

    }

    protected override void OnSkill_2_Exit(ActionObj curActionObj, ActionObj nextActionObj)
    {
        if (IsLocal)
        {
            if (skill_2_Vfx != null)
                NetDestroy(skill_2_Vfx.GetComponent<NetMonobehavior>().NetID, skill_2_Vfx);
        }

    }
    #endregion

    #region----------------重写技能三起势-----------
    protected override void OnSkill_3_Start_Enter(ActionObj curActionObj)
    {
        base.OnSkill_3_Start_Enter(curActionObj);
    }

    protected override void OnSkill_3_Start_Update(ActionObj curActionObj)
    {
        base.OnSkill_3_Start_Update(curActionObj);

        if (IsLocal)
        {
            if (curActionObj.curLifeFrame == 40)
            {

                var go = LoadManager.Instance.NetInstantiate("侠客_技能三_特效", transform);

                go.transform.parent = transform;

                go.transform.localPosition = Vector3.zero;
                //TODO:增加buff，根据buff持续时间设定特效的持续时间

                go.GetComponent<InstantiateObjBase>().Init(this, 30f);
            }
        }

    }

    protected override void OnSkill_3_Start_Exit(ActionObj curActionObj, ActionObj nextActionObj)
    {
        base.OnSkill_3_Start_Exit(curActionObj, nextActionObj);

    }
    #endregion

}
