using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEffectCollection
{
    private static Dictionary<string, OnOccur> buffIdToOnOccurDic = new Dictionary<string, OnOccur>()
    {
        {"眩晕",null },
        {"流血",null },
        {"沉默",null },
        {"中毒",null },
        {"冰冻",null },
        {"灼烧",null },
        {"缴械",null },

    };
    private static Dictionary<string, OnRemove> buffIdToOnRemoveDic = new Dictionary<string, OnRemove>()
    {
        {"眩晕",null },
        {"流血",null },
        {"沉默",null },
        {"中毒",null },
        {"冰冻",null },
        {"灼烧",null },
        {"缴械",null },
    };
    private static Dictionary<string, OnTimeTick> buffIdToOnTimeTickDic = new Dictionary<string, OnTimeTick>()
    {
        {"眩晕",null },
        {"流血",null },
        {"沉默",null },
        {"中毒",null },
        {"冰冻",null },
        {"灼烧",null },
        {"缴械",null },
    };
    private static Dictionary<string, OnHurt> buffIdToOnHurtDic = new Dictionary<string, OnHurt>()
    {
        {"眩晕",null },
        {"流血",null },
        {"沉默",null },
        {"中毒",null },
        {"冰冻",null },
        {"灼烧",null },
        {"缴械",null },
    };
    private static Dictionary<string, OnBeHurt> buffIdToOnBeHurtDic = new Dictionary<string, OnBeHurt>()
    {
        {"眩晕",null },
        {"流血",null },
        {"沉默",null },
        {"中毒",null },
        {"冰冻",null },
        {"灼烧",null },
        {"缴械",null },
    };
    private static Dictionary<string, OnKill> buffIdToOnKillDic = new Dictionary<string, OnKill>()
    {
        {"眩晕",null },
        {"流血",null },
        {"沉默",null },
        {"中毒",null },
        {"冰冻",null },
        {"灼烧",null },
        {"缴械",null },
    };
    private static Dictionary<string, OnBeKillled> buffIdToOnBeKilledDic = new Dictionary<string, OnBeKillled>()
    {
        {"眩晕",null },
        {"流血",null },
        {"沉默",null },
        {"中毒",null },
        {"冰冻",null },
        {"灼烧",null },
        {"缴械",null },
    };

    #region-------------------------------外界获取buff效果的接口-----------------------------
    public static OnOccur GetOnOccur(string key)
    {
        if (buffIdToOnOccurDic.ContainsKey(key))
            return buffIdToOnOccurDic[key];
        else return null;
    }
    public static OnRemove GetOnRemove(string key)
    {
        if (buffIdToOnRemoveDic.ContainsKey(key))
            return buffIdToOnRemoveDic[key];
        else return null;
    }
    public static OnTimeTick GetOnTimeTick(string key)
    {
        if (buffIdToOnTimeTickDic.ContainsKey(key))
            return buffIdToOnTimeTickDic[key];
        else return null;
    }
    public static OnBeHurt GetOnBeHurt(string key)
    {
        if (buffIdToOnBeHurtDic.ContainsKey(key))
            return buffIdToOnBeHurtDic[key];
        else return null;
    }
    public static OnHurt GetOnHurt(string key)
    {
        if (buffIdToOnHurtDic.ContainsKey(key))
            return buffIdToOnHurtDic[key];
        else return null;
    }
    public static OnBeKillled GetOnBeKilled(string key)
    {
        if (buffIdToOnBeKilledDic.ContainsKey(key))
            return buffIdToOnBeKilledDic[key];
        else return null;
    }
    public static OnKill GetOnKill(string key)
    {
        if (buffIdToOnKillDic.ContainsKey(key))
            return buffIdToOnKillDic[key];
        else return null;
    }
    #endregion
    #region ------------------------------------净化---------------------------------
    public static void OnClearStateBuffOccur(GameObject gameObject)
    {

    }

    public static void OnClearStateBuffRemove(GameObject gameObject)
    {
    }

    #endregion
}
