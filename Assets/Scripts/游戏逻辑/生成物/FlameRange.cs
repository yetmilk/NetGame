using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameRange : InstantiateObjBase
{
    private CollisionDetector detector;

    protected override void Start()
    {
        base.Start();
        detector.GetComponent<CollisionDetector>();
    }
    public override void Init(object owner, float lifeTime = -1)
    {
        base.Init(owner, lifeTime);

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
                AddBuffInfo addBuffInfo = new AddBuffInfo(BuffName.×ÆÉÕ.ToString(), item.target.GetComponent<CharacterController>(), ownerCBCtrl);

                item.target.GetComponent<CharacterController>().curCharaData.buffController.AddBuff(addBuffInfo);
            }
        }

    }
}
