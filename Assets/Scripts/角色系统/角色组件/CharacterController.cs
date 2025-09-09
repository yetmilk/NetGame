
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerManager;

[RequireComponent(typeof(ActionController))]
[RequireComponent(typeof(CharacterDataController))]
[RequireComponent(typeof(CampFlag))]
public class CharacterController : NetMonobehavior, IDealActionCommand, ICanInteract
{
    public ActionController selfActionCtrl;


    public CharacterDataController curCharaData;

    public CharacterClacify charaTag;

    public AudioSource audioSource;

    public Rigidbody rb;

    public CampFlag campFlag;



    public CharacterClacify Type => charaTag;

    protected virtual void Awake()
    {

        rb = GetComponent<Rigidbody>();

        curCharaData = GetComponent<CharacterDataController>();

        selfActionCtrl = GetComponent<ActionController>();

        audioSource = GetComponent<AudioSource>();

        campFlag = GetComponent<CampFlag>();
    }

    protected override void Start()
    {

        base.Start();
        //Init(charaTag);

        Init(charaTag);
        // StartCoroutine(InitCol());

        EventCenter.Subscribe(EventCenter.EventId.LogicFrameUpdate, LogicUpdate);

        NetManager.AddMsgListener("MsgDamageInfo", GetDamage);
    }
    IEnumerator InitCol()
    {
        PlayerManager.PlayerInfo playerInfo;
        while (true)
        {
            playerInfo = PlayerManager.Instance.GetPlayerInfoByNetId(NetID);
            if (playerInfo != null) break;
            yield return null;
        }


    }

    public void Init(CharacterClacify character)
    {

        charaTag = character;


        PlayerManager.PlayerInfo playerInfo = PlayerManager.Instance.GetPlayerInfoByName(PlayerManager.Instance.selfId);
        if (playerInfo != null && IsLocal && playerInfo.character == charaTag)
        {
            curCharaData.Init(ref playerInfo.characterData, this, false);
            BattleManager.Instance.uiManager.Initilize(ref playerInfo.characterData, curCharaData.uiParent);
        }
        else
        {
            CharacterDataSO data = LoadManager.Instance.GetResourceByName<CharacterDataSO>(charaTag.ToString());
            CharacterDataObj dataObj = new CharacterDataObj(data);
            curCharaData.Init(ref dataObj, this, true);
        }

        selfActionCtrl.InitializeAction(character, this);
        selfActionCtrl.OnActionEnter += DealActionEnter;
        selfActionCtrl.OnActionUpdate += DealActionUpdate;
        selfActionCtrl.OnActionExit += DealActionExit;
    }

    protected virtual void LogicUpdate(object param = null)
    {

    }

    #region----------------------------状态逻辑------------------------
    private void DealActionEnter(ActionTag tag, ActionObj curActionObj)
    {
        //Debug.Log(11);
        switch (tag)
        {
            case ActionTag.Idle:
                OnIdleEnter(curActionObj);
                break;
            case ActionTag.Move:
                OnMoveEnter(curActionObj);
                break;
            case ActionTag.NormalAttack:
                OnAttackEnter(curActionObj);
                break;
            case ActionTag.Parry:
                OnParryEnter(curActionObj);
                break;
            case ActionTag.Block:
                break;
            case ActionTag.Hurt:
                OnHurtEnter(curActionObj);
                break;
            case ActionTag.Dead:
                OnDeadEnter(curActionObj);
                break;
            case ActionTag.Skill1:
                OnSkill_1_Enter(curActionObj);
                break;
            case ActionTag.Skill2:
                OnSkill_2_Enter(curActionObj);
                break;
            case ActionTag.Skill3:
                OnSkill_3_Enter(curActionObj);
                break;
            case ActionTag.Skill1_Start:
                OnSkill_1_Start_Enter(curActionObj);
                break;
            case ActionTag.Skill2_Start:
                OnSkill_2_Start_Enter(curActionObj);
                break;
            case ActionTag.Skill3_Start:
                OnSkill_3_Start_Enter(curActionObj);
                break;
            default:
                break;
        }
    }
    private void DealActionUpdate(ActionTag tag, ActionObj curActionObj)
    {
        //Debug.Log(tag);
        switch (tag)
        {
            case ActionTag.Idle:
                OnIdleUpdate(curActionObj);
                break;
            case ActionTag.Move:
                OnMoveUpdate(curActionObj);
                break;
            case ActionTag.NormalAttack:
                OnAttackUpdate(curActionObj);
                break;
            case ActionTag.Parry:
                OnParryUpdate(curActionObj);
                break;
            case ActionTag.Block:
                break;
            case ActionTag.Hurt:
                OnHurtUpdate(curActionObj);
                break;
            case ActionTag.Dead:
                OnDeadUpdate(curActionObj);
                break;
            case ActionTag.Skill1:
                OnSkill_1_Update(curActionObj);
                break;
            case ActionTag.Skill2:
                OnSkill_2_Update(curActionObj);
                break;
            case ActionTag.Skill3:
                OnSkill_3_Update(curActionObj);
                break;
            case ActionTag.Skill1_Start:
                OnSkill_1_Start_Update(curActionObj);
                break;
            case ActionTag.Skill2_Start:
                OnSkill_2_Start_Update(curActionObj);
                break;
            case ActionTag.Skill3_Start:
                OnSkill_3_Start_Update(curActionObj);
                break;
            default:
                break;
        }
    }
    private void DealActionExit(ActionTag tag, ActionObj curActionObj, ActionObj nextActionObj)
    {
        //Debug.Log(33);
        switch (tag)
        {
            case ActionTag.Idle:
                OnIdleExit(curActionObj, nextActionObj);
                break;
            case ActionTag.Move:
                OnMoveExit(curActionObj, nextActionObj);
                break;
            case ActionTag.NormalAttack:
                OnAttackExit(curActionObj, nextActionObj);
                break;
            case ActionTag.Parry:
                OnParryExit(curActionObj, nextActionObj);
                break;
            case ActionTag.Block:
                break;
            case ActionTag.Hurt:
                OnHurtExit(curActionObj, nextActionObj);
                break;
            case ActionTag.Dead:
                OnDeadExit(curActionObj, nextActionObj);
                break;
            case ActionTag.Skill1:
                OnSkill_1_Exit(curActionObj, nextActionObj);
                break;
            case ActionTag.Skill2:
                OnSkill_2_Exit(curActionObj, nextActionObj);
                break;
            case ActionTag.Skill3:
                OnSkill_3_Exit(curActionObj, nextActionObj);
                break;
            case ActionTag.Skill1_Start:
                OnSkill_1_Start_Exit(curActionObj, nextActionObj);
                break;
            case ActionTag.Skill2_Start:
                OnSkill_2_Start_Exit(curActionObj, nextActionObj);
                break;
            case ActionTag.Skill3_Start:
                OnSkill_3_Start_Exit(curActionObj, nextActionObj);
                break;
            default:
                break;
        }
    }
    #endregion

    #region-------------------子类重写-----------------
    #region Idle
    protected virtual void OnIdleEnter(ActionObj curActionObj)
    {

    }
    protected virtual void OnIdleUpdate(ActionObj curActionObj)
    {

    }

    protected virtual void OnIdleExit(ActionObj curActionObj, ActionObj nextActionObj)
    {

    }
    #endregion

    #region Move
    protected virtual void OnMoveEnter(ActionObj curActionObj)
    {

    }
    protected virtual void OnMoveUpdate(ActionObj curActionObj)
    {
        //if (IsLocal)
        //{
        //    Vector3 inputDir = PlayerInputManager.Instance.GetInputDiraction();
        //    if (inputDir != Vector3.zero)
        //    {
        //        PlayerInputManager.Instance.HandleInput(new InputCommand(InputCommandType.移动, inputDir),NetID);
        //    }
        //}
    }
    protected virtual void OnMoveExit(ActionObj curActionObj, ActionObj nextActionObj) { }
    #endregion

    #region Attack
    protected virtual void OnAttackEnter(ActionObj curActionObj) { }
    protected virtual void OnAttackUpdate(ActionObj curActionObj) { }
    protected virtual void OnAttackExit(ActionObj curActionObj, ActionObj nextActionObj) { }
    #endregion

    #region Parry
    float eneryResumeSpeed = 0;
    protected virtual void OnParryEnter(ActionObj curActionObj)
    {
        curCharaData.characterState.isInvincible = true;
        eneryResumeSpeed = curCharaData.GetDataObj().skillResumeSpeed;
        curCharaData.GetDataObj().skillResumeSpeed = 0;
        Physics.IgnoreLayerCollision(6, 8, true);
    }
    protected virtual void OnParryUpdate(ActionObj curActionObj) { }

    protected virtual void OnParryExit(ActionObj curActionObj, ActionObj nextActionObj)
    {
        curCharaData.characterState.isInvincible = false;
        Physics.IgnoreLayerCollision(6, 8, false);
        curCharaData.GetDataObj().curEnergyValue -= curCharaData.GetDataObj().parrySpendValue;
        curCharaData.GetDataObj().skillResumeSpeed = eneryResumeSpeed;
    }
    #endregion

    #region Hurt
    protected virtual void OnHurtEnter(ActionObj curActionObj) { }
    protected virtual void OnHurtUpdate(ActionObj curActionObj) { }
    protected virtual void OnHurtExit(ActionObj curActionObj, ActionObj nextActionObj) { }
    #endregion

    #region Dead
    protected virtual void OnDeadEnter(ActionObj curActionObj) { }
    protected virtual void OnDeadUpdate(ActionObj curActionObj) { }
    protected virtual void OnDeadExit(ActionObj curActionObj, ActionObj nextActionObj) { }
    #endregion

    #region Skill_1
    protected virtual void OnSkill_1_Enter(ActionObj curActionObj) { }
    protected virtual void OnSkill_1_Update(ActionObj curActionObj) { }
    protected virtual void OnSkill_1_Exit(ActionObj curActionObj, ActionObj nextActionObj)
    {
        curCharaData.GetDataObj().skill1ResumeTimer = curCharaData.GetDataObj().skill1ResumeTime;
    }

    #endregion

    #region Skill_2
    protected virtual void OnSkill_2_Enter(ActionObj curActionObj) { }
    protected virtual void OnSkill_2_Update(ActionObj curActionObj) { }
    protected virtual void OnSkill_2_Exit(ActionObj curActionObj, ActionObj nextActionObj)
    {
        curCharaData.GetDataObj().skill2ResumeTimer = curCharaData.GetDataObj().skill2ResumeTime;
    }
    #endregion

    #region Skill_3
    protected virtual void OnSkill_3_Enter(ActionObj curActionObj) { }
    protected virtual void OnSkill_3_Update(ActionObj curActionObj) { }
    protected virtual void OnSkill_3_Exit(ActionObj curActionObj, ActionObj nextActionObj)
    {
        curCharaData.GetDataObj().skill3ResumeTimer = curCharaData.GetDataObj().skill3ResumeTime;
    }
    #endregion

    #region Skill_1_Start
    protected virtual void OnSkill_1_Start_Enter(ActionObj curActionObj) { }
    protected virtual void OnSkill_1_Start_Update(ActionObj curActionObj) { }

    protected virtual void OnSkill_1_Start_Exit(ActionObj curActionObj, ActionObj nextActionObj) { }
    #endregion
    #region Skill_2_Start
    protected virtual void OnSkill_2_Start_Enter(ActionObj curActionObj) { }
    protected virtual void OnSkill_2_Start_Update(ActionObj curActionObj) { }

    protected virtual void OnSkill_2_Start_Exit(ActionObj curActionObj, ActionObj nextActionObj) { }
    #endregion
    #region Skill_3_Start
    protected virtual void OnSkill_3_Start_Enter(ActionObj curActionObj) { }
    protected virtual void OnSkill_3_Start_Update(ActionObj curActionObj) { }

    protected virtual void OnSkill_3_Start_Exit(ActionObj curActionObj, ActionObj nextActionObj) { }
    #endregion
    #endregion

    #region-------------------输入指令处理--------------------------
    public void HandleInputCommand(InputCommand command)
    {
        switch (command.actionTag)
        {
            case ActionTag.None:
                break;
            case ActionTag.Idle:
                selfActionCtrl.AddCommand("", ActionTag.Idle, selfActionCtrl.curActionObj.direction);
                return;
            case ActionTag.Move:
                if (curCharaData.characterState.cantMove) return;
                selfActionCtrl.AddCommand("", ActionTag.Move, command.direction);
                return;
            case ActionTag.NormalAttack:
                Vector3 attackDir = (command.direction - transform.position).normalized;
                selfActionCtrl.AddCommand("", ActionTag.NormalAttack, attackDir);
                return;
            case ActionTag.Parry:
                if (curCharaData.GetDataObj().curEnergyValue - curCharaData.GetDataObj().parrySpendValue <= 0)
                    return;

                Vector3 dir = command.direction == Vector3.zero ? selfActionCtrl.curActionObj.direction : command.direction;
                selfActionCtrl.AddCommand("", ActionTag.Parry, dir);
                return;
            case ActionTag.Block:
                break;
            case ActionTag.Hurt:
                selfActionCtrl.AddCommand("", ActionTag.Hurt, command.direction);
                break;
            case ActionTag.Dead:
                break;
            case ActionTag.Skill1:
                selfActionCtrl.AddCommand("", ActionTag.Skill1, command.direction);
                return;
            case ActionTag.Skill2:
                selfActionCtrl.AddCommand("", ActionTag.Skill2, command.direction);
                break;
            case ActionTag.Skill3:
                selfActionCtrl.AddCommand("", ActionTag.Skill3, command.direction);
                break;
            case ActionTag.Skill1_Start:
                if (curCharaData.GetDataObj().skill1ResumeTimer > 0)
                    return;

                selfActionCtrl.AddCommand("", ActionTag.Skill1_Start, command.direction);
                return;
            case ActionTag.Skill2_Start:
                if (curCharaData.GetDataObj().skill2ResumeTimer > 0)
                    return;

                selfActionCtrl.AddCommand("", ActionTag.Skill2_Start, command.direction);
                return;
            case ActionTag.Skill3_Start:
                if (curCharaData.GetDataObj().skill3ResumeTimer > 0)
                    return;

                selfActionCtrl.AddCommand("", ActionTag.Skill2, command.direction);
                return;
            default:
                break;
        }

        switch (command.type)
        {
            case InputCommandType.待机:

                selfActionCtrl.AddCommand("", ActionTag.Idle, selfActionCtrl.curActionObj.direction);
                break;
            case InputCommandType.移动:
                if (curCharaData.characterState.cantMove) return;
                selfActionCtrl.AddCommand("", ActionTag.Move, command.direction);
                break;
            case InputCommandType.闪避:
                if (curCharaData.GetDataObj().curEnergyValue - curCharaData.GetDataObj().parrySpendValue <= 0)
                    return;
                //curCharaData.GetDataObj().curEnergyValue -= curCharaData.GetDataObj().parrySpendValue;
                Vector3 dir = command.direction == Vector3.zero ? selfActionCtrl.curActionObj.direction : command.direction;
                selfActionCtrl.AddCommand("", ActionTag.Parry, dir);
                break;
            case InputCommandType.普通攻击:

                Vector3 attackDir = (command.direction - transform.position).normalized;
                selfActionCtrl.AddCommand("", ActionTag.NormalAttack, attackDir);
                break;

            case InputCommandType.交互:
                break;

            case InputCommandType.技能1_起势:
                if (curCharaData.GetDataObj().skill1ResumeTimer > 0)
                    return;
                //curCharaData.GetDataObj().skill1ResumeTimer = curCharaData.GetDataObj().skill1ResumeTime;
                selfActionCtrl.AddCommand("", ActionTag.Skill1_Start, command.direction);
                break;
            case InputCommandType.技能1:


                selfActionCtrl.AddCommand("", ActionTag.Skill1, command.direction);
                break;
            case InputCommandType.技能2_起势:
                if (curCharaData.GetDataObj().skill2ResumeTimer > 0)
                    return;
                //curCharaData.GetDataObj().skill2ResumeTimer = curCharaData.GetDataObj().skill2ResumeTime;

                selfActionCtrl.AddCommand("", ActionTag.Skill2_Start, command.direction);
                break;
            case InputCommandType.技能2:

                selfActionCtrl.AddCommand("", ActionTag.Skill2, command.direction);
                break;
            case InputCommandType.技能3_起势:
                if (curCharaData.GetDataObj().skill3ResumeTimer > 0)
                    return;
                //curCharaData.GetDataObj().skill3ResumeTimer = curCharaData.GetDataObj().skill3ResumeTime;
                selfActionCtrl.AddCommand("", ActionTag.Skill3_Start, command.direction);
                break;
            default:
                break;
        }
    }
    public void HandleInputCommand(string actionName, ActionTag actionTag, Vector3 dir)
    {
        if (actionTag == ActionTag.Parry)
        {
            if (curCharaData.GetDataObj().curEnergyValue - curCharaData.GetDataObj().parrySpendValue <= 0)
                return;
            //curCharaData.GetDataObj().curEnergyValue -= curCharaData.GetDataObj().parrySpendValue;
        }
        else if (actionTag == ActionTag.Skill1_Start)
        {
            if (curCharaData.GetDataObj().skill1ResumeTimer > 0)
                return;
            //curCharaData.GetDataObj().skill1ResumeTimer = curCharaData.GetDataObj().skill1ResumeTime;
        }
        else if (actionTag == ActionTag.Skill2_Start)
        {
            if (curCharaData.GetDataObj().skill2ResumeTimer > 0)
                return;
            //curCharaData.GetDataObj().skill2ResumeTimer = curCharaData.GetDataObj().skill2ResumeTime;
        }
        else if (actionTag == ActionTag.Skill3_Start)
        {
            if (curCharaData.GetDataObj().skill3ResumeTimer > 0)
                return;
            //curCharaData.GetDataObj().skill3ResumeTimer = curCharaData.GetDataObj().skill3ResumeTime;
        }
        selfActionCtrl.AddCommand(actionName, actionTag, dir);
    }

    #endregion

    #region--------------------------特殊状态处理--------------------------------------

    public void Attack(List<CollisionDetector.DetectionInfo> detectList, DamageFormulaType formulaType, ActionTag actionTag)
    {

        foreach (var item in detectList)
        {
            if (item.target.GetComponent<IDataContainer>() != null
                && !item.target.GetComponent<CampFlag>().CheckIsFriendly(campFlag.CampType))
            {
                DamageInfo damageInfo = new DamageInfo()
                {
                    damageValue = DamageCaculateCollection.CaculateDamage(formulaType, curCharaData.GetDataObj()
                    , item.target.GetComponent<CharacterDataController>().GetDataObj()),
                    fromerNetId = NetID,
                    targetNetId = item.target.GetComponent<CharacterController>().NetID,
                    DamageDir = (item.target.transform.position - transform.position).normalized,
                    attackTag = actionTag,
                };

                curCharaData.buffController.UpdateBuffsOnAttack(item.target.GetComponent<CharacterController>(), this, ref damageInfo);

                MsgDamageInfo msg = new MsgDamageInfo(damageInfo);

                NetManager.Send(msg);
            }
        }
    }
    public void Attack(List<CollisionDetector.DetectionInfo> detectList, DamageInfo damageInfo)
    {

        foreach (var item in detectList)
        {
            if (item.target.GetComponent<IDataContainer>() != null
                && !item.target.GetComponent<CampFlag>().CheckIsFriendly(campFlag.CampType))
            {

                curCharaData.buffController.UpdateBuffsOnAttack(item.target.GetComponent<CharacterController>(), this, ref damageInfo);

                MsgDamageInfo msg = new MsgDamageInfo(damageInfo);

                NetManager.Send(msg);
            }
        }
    }
    public virtual void GetDamage(MsgBase msgBase)
    {
        MsgDamageInfo msg = msgBase as MsgDamageInfo;

        DamageInfo damageInfo = new DamageInfo(msg);
        if (curCharaData.characterState.isInvincible) return;
        if (damageInfo.targetNetId != NetID) return;
        HandleInputCommand("", ActionTag.Hurt, -damageInfo.DamageDir);
        //TODO:数据同步
        curCharaData.CaculateDamage(damageInfo);
        if (curCharaData.GetDataObj().curHealth <= 0f)
            HandleInputCommand("", ActionTag.Dead, Vector3.zero);
    }

    public virtual bool CanBeAttack(CampFlag attackercamp)
    {
        return curCharaData.GetDataObj().curHealth > 0f && !GetComponent<CampFlag>().CheckIsFriendly(attackercamp.CampType);
    }

    public virtual bool CanBeHealth(CampFlag camp)
    {
        return curCharaData.GetDataObj().curHealth < curCharaData.GetDataObj().maxhealth && !GetComponent<CampFlag>().CheckIsFriendly(camp.CampType);
    }

    public void GetHealth(float health)
    {
        curCharaData.GetDataObj().curHealth += health;
        curCharaData.GetDataObj().curHealth = curCharaData.GetDataObj().curHealth > curCharaData.GetDataObj().maxhealth
            ? curCharaData.GetDataObj().maxhealth : curCharaData.GetDataObj().curHealth;


    }
    #endregion

    private void OnDestroy()
    {
        EventCenter.Unsubscribe(EventCenter.EventId.LogicFrameUpdate, LogicUpdate);
        NetManager.RemoveMsgListener("MsgDamageInfo", GetDamage);
    }


}
