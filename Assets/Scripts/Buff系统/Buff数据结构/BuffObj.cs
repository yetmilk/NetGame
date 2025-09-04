using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuffObj
{
    public BuffModule module;

    //public int stack;

    public int curTime;

    public bool isForever;

    public int lifeTime;

    public int tickTimer;

    public CharacterController owner;

    public CharacterController target;

    public Dictionary<string, object> paramDic;

    public OnOccur onOccur = null;

    public OnRemove onRemove = null;

    public OnTimeTick onTimeTick = null;

    public OnBeHurt onBeHurt = null;

    public OnHurt onHurt = null;

    public OnBeKillled onBeKillled = null;

    public OnKill onKill = null;

    public OnAttack onAttack = null;

    public BuffObj(AddBuffInfo buffInfo)
    {
        this.module = buffInfo.buffModel;
        this.curTime = 0;
        this.isForever = module.isForever;
        this.lifeTime = module.lifeTime;
        if (module.tickTime > 0)
            this.tickTimer = 0;
        else this.tickTimer = -1;

        this.paramDic = new Dictionary<string, object>();

        this.owner = buffInfo.owner;
        this.target = buffInfo.target;
        OnEnable();

    }

    #region-----------------Buff»Øµ÷µã-----------------------
    public virtual void OnOccur(CharacterController target, CharacterController owner)
    {

    }

    public virtual void OnRemove(CharacterController target, CharacterController owner)
    {

    }

    public virtual void OnTimeTick(CharacterController target, CharacterController owner)
    {
    }

    public virtual void OnHurt(CharacterController target, CharacterController owner, ref DamageInfo damageInfo)
    {

    }

    public virtual void OnBeKillled(CharacterController target, CharacterController owner)
    {

    }

    public virtual void OnKill(CharacterController target, CharacterController owner)
    {

    }
    public virtual void OnAttack(CharacterController target, CharacterController owner, ref DamageInfo damageInfo)
    {

    }

    #endregion

    public virtual void OnEnable()
    {
        onOccur += OnOccur;
        onRemove += OnRemove;
        onTimeTick += OnTimeTick;
        onHurt += OnHurt;
        onBeKillled += OnBeKillled;
        onKill += OnKill;
        onAttack += OnAttack;
    }
    public virtual void Disable()
    {
        onOccur -= OnOccur;
        onRemove -= OnRemove;
        onTimeTick -= OnTimeTick;
        onHurt -= OnHurt;
        onBeKillled -= OnBeKillled;
        onKill -= OnKill;
        onAttack -= OnAttack;
    }

}
