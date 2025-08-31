using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkPlayer : CharacterController
{


    #region---------------重写技能一起势------------------
    private GameObject skill_1_IndectorObj;
    private GameObject skill_1_rangeObj;
    protected override void OnSkill_1_Start_Enter(ActionObj curActionObj)
    {
        if (IsLocal)
        {
            GameObject indectorPrefab = LoadManager.Instance.GetResourceByName<GameObject>("仙术师_技能1_指示器");
            GameObject indectorRangePrefab = LoadManager.Instance.GetResourceByName<GameObject>("仙术师_技能1_范围指示器");
            skill_1_IndectorObj = Instantiate(indectorPrefab);
            skill_1_rangeObj = Instantiate(indectorRangePrefab, transform);
            insVfxPos = Vector3.zero;
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
            if (Vector3.Distance(MousePos, transform.position) < 5f)
            {
                skill_1_IndectorObj.transform.position = new Vector3(MousePos.x, .5f, MousePos.z);
            }
        }
    }

    protected override void OnSkill_1_Start_Exit(ActionObj curActionObj, ActionObj nextActionObj)
    {
        if (nextActionObj.curActionInfo.tag == ActionTag.Skill1)
        {
            nextActionObj.direction = curActionObj.direction;
            if (IsLocal)
                insVfxPos = skill_1_IndectorObj.transform.position;
        }
        if (IsLocal)
        {
            if (skill_1_IndectorObj != null)
                Destroy(skill_1_IndectorObj);
            if (skill_1_rangeObj != null)
                Destroy(skill_1_rangeObj);
        }


    }

    #endregion
    #region---------------重写技能一------------------
    private Vector3 insVfxPos;
    private GameObject skill_1_Vfx;
    protected override void OnSkill_1_Enter(ActionObj curActionObj)
    {
        if (IsLocal)
        {
            skill_1_Vfx = LoadManager.Instance.NetInstantiate("VFX_仙术师_技能1_特效", transform, NetID);
            skill_1_Vfx.transform.parent = null;
            //skill_1_Vfx.GetComponent<GrowFlower>().Init(this);
            skill_1_Vfx.transform.position = insVfxPos;
        }


    }
    protected override void OnSkill_1_Update(ActionObj curActionObj)
    {
    }

    protected override void OnSkill_1_Exit(ActionObj curActionObj, ActionObj nextActionObj)
    {

    }
    #endregion

    #region---------------重写技能二起势------------------
    private GameObject skill_2_IndictorObj;
    protected override void OnSkill_2_Start_Enter(ActionObj curActionObj)
    {
        if (IsLocal)//判断是否是本端
        {
            var indictorPrefab = LoadManager.Instance.GetResourceByName<GameObject>("仙术师_技能2_范围指示器");
            skill_2_IndictorObj = Instantiate(indictorPrefab, transform);
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
    private float timer = 0f;
    protected override void OnSkill_2_Enter(ActionObj curActionObj)
    {

        timer = Time.time;
    }
    protected override void OnSkill_2_Update(ActionObj curActionObj)
    {

        if (IsLocal)
        {
            if (curActionObj.curLifeFrame == 20)
            {
                skill_2_Vfx = LoadManager.Instance.NetInstantiate("VFX_仙术师_技能2", transform, NetID);
                skill_2_Vfx.GetComponent<CollisionDetector>().Init(this.gameObject);
                var dectectList = skill_2_Vfx.GetComponent<CollisionDetector>().PerformDetection();

                Attack(dectectList, DamageFormulaType.通用, ActionTag.Skill2);
                curActionObj.GetLogicByFuncId(FunctionId.Move).MoveFunctionParam.moveDir = Vector3.zero;
            }
            if (Time.time - timer < 3f && curActionObj.curLifeFrame > 20)
            {
                bool checkSucc;
                Vector3 MousePos = PlayerInputManager.Instance.GetMouseDirection(out checkSucc);
                if (checkSucc)
                {
                    Vector3 dir = (MousePos - transform.position).normalized;
                    PlayerInputManager.Instance.HandleInput(new InputCommand(InputCommandType.技能2, dir), NetID);
                }

                Vector3 inputDir = PlayerInputManager.Instance.GetInputDiraction();

                curActionObj.GetLogicByFuncId(FunctionId.Move).MoveFunctionParam.moveDir = inputDir;
            }

            //获取碰撞检测的物体
            if (skill_2_Vfx != null)
            {
                List<CollisionDetector.DetectionInfo> detectList = skill_2_Vfx.GetComponent<CollisionDetector>().CurrentDetections;
                foreach (var item in detectList)
                {
                    if (item.target.GetComponent<IDataContainer>() != null
                        && !item.target.GetComponent<CampFlag>().CheckIsFriendly(campFlag.CampType))
                    {
                        DamageInfo damageInfo = new DamageInfo()
                        {
                            damageValue = DamageCaculateCollection.CaculateDamage(DamageFormulaType.通用, curCharaData.GetDataObj()
                            , item.target.GetComponent<CharacterDataController>().GetDataObj()),
                            fromerNetId = NetID,
                            targetNetId = item.target.GetComponent<CharacterController>().NetID,
                            DamageDir = (item.target.transform.position - transform.position).normalized,
                            attackTag = ActionTag.Skill2,
                        };

                        MsgDamageInfo msg = new MsgDamageInfo(damageInfo);

                        NetManager.Send(msg);
                    }
                }
            }
        }
        if (Time.time - timer > 3f)
        {
            PlayerInputManager.Instance.HandleInput(new InputCommand(InputCommandType.待机), NetID);

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

                var go = LoadManager.Instance.NetInstantiate("VFX_仙术师_技能3", transform, NetID);


                //TODO:增加buff，根据buff持续时间设定特效的持续时间

                go.GetComponent<InstantiateObjBase>().Init(this, 4f);
            }
        }

    }

    protected override void OnSkill_3_Start_Exit(ActionObj curActionObj, ActionObj nextActionObj)
    {
        base.OnSkill_3_Start_Exit(curActionObj, nextActionObj);

    }
    #endregion
}
