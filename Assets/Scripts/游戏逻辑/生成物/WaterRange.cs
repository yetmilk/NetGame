using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WaterRange : InstantiateObjBase
{
    public CollisionDetector detector;


    protected override void Update()
    {
        base.Update();
        var list = detector.PerformDetection();

        foreach (var item in list)
        {
            if (item.target.GetComponent<IDataContainer>() != null
             && !item.target.GetComponent<CampFlag>().CheckIsFriendly(ownerCBCtrl.campFlag.CampType))
            {
                AddBuffInfo addBuffInfo = new AddBuffInfo(BuffName.Ë®²¨ÎÆ.ToString(), item.target.GetComponent<CharacterController>(), ownerCBCtrl);

                item.target.GetComponent<CharacterController>().curCharaData.buffController.AddBuff(addBuffInfo);
            }
        }
    }
}
