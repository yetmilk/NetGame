using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCondensingStance : BuffObj
{
    private CollisionDetector detector;
    public WaterCondensingStance(AddBuffInfo addBuffInfo) : base(addBuffInfo)
    {

        addBuffInfo.target.selfActionCtrl.OnActionExit += ParryExit;

    }

    private void ParryExit(ActionTag tag, ActionObj obj1, ActionObj obj2)
    {
        if (tag == ActionTag.Parry && target.IsLocal)
        {

            var vfx = LoadManager.Instance.NetInstantiate("Ë®²¨ÎÆ", target.transform, target.NetID);

            vfx.GetComponent<InstantiateObjBase>().Init(target, 2.5f);

            //target.transform.parent = null;
        }
    }
}
