using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageCaculateCollection
{
    private static Dictionary<DamageFormulaType, Func<CharacterDataObj, CharacterDataObj, float>> DamageFormulaDic = new Dictionary<DamageFormulaType, Func<CharacterDataObj, CharacterDataObj, float>>()
    {
        {DamageFormulaType.通用,GeneralDamageFormula },

    };

    private static float GeneralDamageFormula(CharacterDataObj attacker, CharacterDataObj beAttacker)
    {
        return 5f;
    }

    public static float CaculateDamage(DamageFormulaType type, CharacterDataObj attacker, CharacterDataObj beAttacker)
    {
        if (DamageFormulaDic.ContainsKey(type))
            return DamageFormulaDic[type](attacker, beAttacker);

        Debug.LogError("未找到符合条件的伤害计算公式");
        return 0f;
    }

}

public enum DamageFormulaType
{
    通用,
    剑客_普攻,
    剑客_离尘一式,
    剑客_凝霜剑气,
    剑客_生生不息,
    仙术师_普攻,
    仙术师_天地造化,
    仙术师_离火诀,
    仙术师_业火焚心,
}