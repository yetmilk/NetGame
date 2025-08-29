using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InstantiateType
{
    技能指示,
    交互物体,

}

public class InstantiateObjBase : NetMonobehavior, IInstantiateObj
{
    [Header("销毁时间")]
    public float lifeTime = .5f;

    protected CharacterController ownerCBCtrl;
    protected ActionController ownerActionCtrl;

    public virtual void Init(object owner, float lifeTime = -1)
    {
        ownerCBCtrl = owner as CharacterController;
        this.lifeTime = lifeTime;
        ownerActionCtrl = ownerCBCtrl.selfActionCtrl;
    }

    protected virtual void Update()
    {
        if (!IsLocal) return;
        if (lifeTime != -1 || lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime < 0)
            {
                lifeTime = 0;
                NetDestroy(NetID, gameObject);
            }
        }

    }
}
