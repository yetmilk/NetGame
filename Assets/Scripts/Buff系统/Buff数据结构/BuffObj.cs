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

    public BuffObj(AddBuffInfo buffInfo)
    {
        this.module = buffInfo.buffModel;
        this.curTime = 0;
        this.isForever = buffInfo.isForever;
        this.lifeTime = buffInfo.buffLifeTime;
        if (module.tickTime > 0)
            this.tickTimer = 0;
        else this.tickTimer = -1;

        this.paramDic = new Dictionary<string, object>();

        this.owner = buffInfo.fromer;
        this.target = buffInfo.carrier;
        module.onOccur += OnOccur;
        module.onRemove += OnRemove;
        module.onTimeTick += OnTimeTick;
        module.onHurt += OnHurt;
        module.onBeKillled += OnBeKillled;
        module.onKill += OnKill;

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
    #endregion
}
