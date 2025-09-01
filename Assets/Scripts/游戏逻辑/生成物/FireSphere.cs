using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSphere : InstantiateObjBase
{
    public CollisionDetector detector;
    protected override void Start()
    {
        base.Start();
        detector = GetComponent<CollisionDetector>();
    }

    protected override void Update()
    {
        base.Update();
        var list = detector.PerformDetection();

        foreach (var item in list)
        {
            if (item.target.GetComponent<IDataContainer>() != null
             && !item.target.GetComponent<CampFlag>().CheckIsFriendly(ownerCBCtrl.campFlag.CampType))
            {
                DamageInfo damageInfo = new DamageInfo()
                {
                    damageValue = DamageCaculateCollection.CaculateDamage(DamageFormulaType.Õ®”√,
                    ownerCBCtrl.curCharaData.GetDataObj()
                    , item.target.GetComponent<CharacterDataController>().GetDataObj()),
                    DamageDir = (item.target.transform.position - ownerCBCtrl.transform.position).normalized,
                    attackTag = ActionTag.None,
                    targetNetId = item.target.GetComponent<CharacterController>().NetID,
                    fromerNetId = ownerCBCtrl.NetID
                };


                MsgDamageInfo msg = new MsgDamageInfo(damageInfo);

                NetManager.Send(msg);
            }
        }
    }
}
