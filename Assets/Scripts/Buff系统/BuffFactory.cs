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
                return new BurnBuff(buffInfo);
            case BuffName.焚阳剑诀:
                return new BurnSwordBuff(buffInfo);
            case BuffName.烈焰身法:
                return new FlameBodyBuff(buffInfo);
            case BuffName.凝水身法:
                return new WaterCondensingStance(buffInfo);
            case BuffName.水波纹:
                return new WaterWaveBuff(buffInfo);
            case BuffName.玄冰剑诀:
                return new IceSwordBuff(buffInfo);
            case BuffName.减速:
                return new ReduceSpeedBuff(buffInfo);
            case BuffName.净水诀:
                return new CleanWaterBuff(buffInfo);
            case BuffName.焚天身法:
                return new FlameFlyBodyBuff(buffInfo);
            case BuffName.清波功:
                return new ClearWaveSkillBuff(buffInfo);
            case BuffName.踏焰诀:
                return new SteppingFlameManualBuff(buffInfo);
            default:
                return new BuffObj(buffInfo);
        }
    }
}

public enum BuffName
{
    灼烧,
    焚阳剑诀,
    烈焰身法,
    凝水身法,
    玄冰剑诀,
    惊雷剑意,
    净水诀,
    焚天身法,
    清波功,
    踏焰诀,
    水波纹,
    减速
}
