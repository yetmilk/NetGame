using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlameBodyBuff : BuffObj
{

    private CollisionDetector detector;
    public FlameBodyBuff(AddBuffInfo addBuffInfo) : base(addBuffInfo)
    {
        addBuffInfo.target.selfActionCtrl.OnActionEnter += StartDetect;
        addBuffInfo.target.selfActionCtrl.OnActionUpdate += AddFlame;
        addBuffInfo.target.selfActionCtrl.OnActionExit += ParryExit;

    }

    private void StartDetect(ActionTag actionTag, ActionObj actionObj)
    {
        if (actionTag == ActionTag.Parry)
        {
            detector = target.AddComponent<CollisionDetector>();
            detector.Init(target.gameObject);
        }
    }
    private void AddFlame(ActionTag actionTag, ActionObj actionObj)
    {
        if (actionTag == ActionTag.Parry && target.IsLocal)
        {

            var list = detector.PerformDetection();

            foreach (var item in list)
            {
                if (item.target.GetComponent<IDataContainer>() != null
              && !item.target.GetComponent<CampFlag>().CheckIsFriendly(target.campFlag.CampType))
                {
                    AddBuffInfo addBuffInfo = new AddBuffInfo(BuffName.×ÆÉÕ.ToString(), item.target.GetComponent<CharacterController>(), owner);

                    MsgAddBuffObj msg = new MsgAddBuffObj(addBuffInfo);

                    NetManager.Send(msg);
                }
            }
        }
    }

    private void ParryExit(ActionTag actionTag, ActionObj actionObj,ActionObj nextActionObj)
    {
        if (detector != null && actionTag == ActionTag.Parry)
            Object.Destroy(detector);
    }

    public override void Disable()
    {
        base.Disable();
        target.selfActionCtrl.OnActionEnter -= StartDetect;
        target.selfActionCtrl.OnActionUpdate -= AddFlame;   
        target.selfActionCtrl.OnActionExit -= ParryExit;
    }
}
