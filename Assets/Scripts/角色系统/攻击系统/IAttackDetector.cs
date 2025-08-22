using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackDetector:IInstantiateObj
{
    public DamageFormulaType Type { get; }
}
