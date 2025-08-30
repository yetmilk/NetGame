using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuffFactory
{
    public static BuffObj CreateBuff(AddBuffInfo buffInfo)
    {
        switch (buffInfo.buffModel.buffName)
        {
            case BuffName.灼烧:
                var buffModule = LoadManager.Instance.GetResourceByName<BuffModule>(buffInfo.buffModel.buffName.ToString());
                return new BurnBuff(buffInfo);

            default:
                return default;
        }
    }
}

public enum BuffName
{
    灼烧,
}
