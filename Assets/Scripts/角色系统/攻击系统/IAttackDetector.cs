using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackDetector
{
    public DamageFormulaType Type { get; }
}
