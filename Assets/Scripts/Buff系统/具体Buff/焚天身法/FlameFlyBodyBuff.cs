using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameFlyBodyBuff : BuffObj
{
    public FlameFlyBodyBuff(AddBuffInfo buffInfo) : base(buffInfo)
    {
        buffInfo.target.selfActionCtrl.OnActionEnter += ParryEnter;
    }

    private void ParryEnter(ActionTag tag, ActionObj obj)
    {
        if(tag == ActionTag.Parry)
        {
            var go = LoadManager.Instance.NetInstantiate("ª—Ê’Û", target.transform, target.NetID);

            go.GetComponent<CollisionDetector>().Init(target.gameObject);
            go.GetComponent<InstantiateObjBase>().Init(target,2.5f);
        }
    }
}
