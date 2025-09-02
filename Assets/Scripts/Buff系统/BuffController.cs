using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BuffController
{
    CharacterController buffCarrier;

    IBuffView buffUIView;

    public List<BuffObj> curBuffsList = new List<BuffObj>();

    public BuffController(CharacterController buffContainer)
    {
        this.buffCarrier = buffContainer;
        EventCenter.Subscribe(EventCenter.EventId.LogicFrameUpdate, OnCheckBuff);

    }
    public void OnDisable()
    {
        EventCenter.Unsubscribe(EventCenter.EventId.LogicFrameUpdate, OnCheckBuff);

    }

    public void UpdateBuffsOnAttack(CharacterController target, CharacterController fromer, ref DamageInfo damageInfo)
    {
        foreach (var item in curBuffsList)
        {
            item.onAttack?.Invoke(target, fromer, ref damageInfo);
            Debug.Log(item.onAttack.GetInvocationList().Length);
        }
    }
    //检查buff状态
    public void OnCheckBuff(object param)
    {
        Debug.Log(111);
        if (curBuffsList.Count == 0) return;
        //更新buff生命周期，有结束的移除
        for (int i = curBuffsList.Count - 1; i >= 0; i--)
        {
            if (curBuffsList[i].curTime >= curBuffsList[i].lifeTime * FrameManager.LogicFrameRate && !curBuffsList[i].isForever)
            {
                //当buff被移除时的事件
                curBuffsList[i].onRemove?.Invoke(curBuffsList[i].target, curBuffsList[i].owner);
                //更新buffUI
                //buffUIView.UpdateView();

                curBuffsList[i].Disable();
                curBuffsList.RemoveAt(i);
            }
            else
            {
                curBuffsList[i].curTime++;
            }
        }

        //更新buff生效周期计时器，有达到条件的执行
        for (int i = 0; i < curBuffsList.Count; i++)
        {
            if (curBuffsList[i].tickTimer == -1) continue;

            if (curBuffsList[i].tickTimer >= curBuffsList[i].module.tickTime)
            {
                curBuffsList[i].tickTimer = 0;
                curBuffsList[i].onTimeTick?.Invoke(curBuffsList[i].target, curBuffsList[i].owner);
            }
            else
            {
                curBuffsList[i].tickTimer++;
            }
        }

        //更新buff属性
        // UpdateBuffCarrierProp();

    }

    public void AddBuff(AddBuffInfo addBuffInfo)
    {

        BuffModule newBuffModel = addBuffInfo.buffModel;
        BuffObj buff = BuffFactory.CreateBuff(addBuffInfo);

        if (curBuffsList.Exists((a) => a.module.buffName == addBuffInfo.buffModel.buffName)) return;

        curBuffsList.Add(buff);
        curBuffsList.Sort((a, b) => b.module.priority.CompareTo(a.module.priority));
        //执行添加buff时的事件
        buff.onOccur?.Invoke(buff.target, buff.owner);

    }

}
public interface IBuffView
{
    public void UpdateView();
}
