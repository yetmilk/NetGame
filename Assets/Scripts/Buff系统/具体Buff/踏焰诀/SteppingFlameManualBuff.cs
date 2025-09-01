using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteppingFlameManualBuff : BuffObj
{
    public SteppingFlameManualBuff(AddBuffInfo addBuffInfo) : base(addBuffInfo)
    {
        addBuffInfo.target.selfActionCtrl.OnActionExit += OnParryExit;
    }

    private void OnParryExit(ActionTag tag, ActionObj obj1, ActionObj obj2)
    {
        if (tag == ActionTag.Parry)
        {
            var go = LoadManager.Instance.NetInstantiate("»ðÇò", target.transform, target.NetID);

            go.GetComponent<CollisionDetector>().Init(target.gameObject);
            go.GetComponent<InstantiateObjBase>().Init(owner, 0.5f);
        }
    }
}
