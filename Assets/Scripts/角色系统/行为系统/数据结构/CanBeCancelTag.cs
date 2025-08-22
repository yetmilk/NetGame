using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CanBeCancelTag
{
    public Vector2[] canBeCancelRange;

    public ActionTag canCancelTag;

    [Header("该行为打断的优先级")]
    public int proprity = 0;

    [Header("打断动作的起始百分比")]
    public float nextActionStartPercent = 0;


}
