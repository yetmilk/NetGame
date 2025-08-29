using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PropMod
{
    public CharaDataEnum name;
    public bool isPercent;//按照百分比来记录数值
    [Header("使用百分比数值范围0-1，不使用时任意范围")]
    public float value;//数值
}
[System.Serializable]
public struct StateMod
{
    public string name;
    public bool state;
}

public delegate void OnOccur(CharacterController target, CharacterController owner);
public delegate void OnTimeTick(CharacterController target, CharacterController owner);
public delegate void OnBeHurt(CharacterController target, CharacterController owner);
public delegate void OnHurt(CharacterController target, CharacterController owner, ref DamageInfo damageInfo);
public delegate void OnBeKillled(CharacterController target, CharacterController owner);
public delegate void OnKill(CharacterController target, CharacterController owner);
public delegate void OnRemove(CharacterController target, CharacterController owner);


public enum BuffTag
{
    数值修改,
    状态修改,
    效果,
}


public struct AddBuffInfo
{
    public BuffModule buffModel;
    public int buffLifeTime;
    public bool isForever;

    public CharacterController fromer;
    public CharacterController carrier;

    public AddBuffInfo(string buffModelName, int buffLifeTime, bool isForever, CharacterController fromer, CharacterController carrier)
    {
        buffModel = LoadManager.Instance.GetResourceByName<BuffModule>(buffModelName);
        this.buffLifeTime = buffLifeTime;
        this.isForever = isForever;
        this.fromer = fromer;
        this.carrier = carrier;
    }
}