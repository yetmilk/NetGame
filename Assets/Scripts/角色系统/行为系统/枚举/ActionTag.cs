using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionTag
{
    None = 5,
    Idle = 0,
    Move = 1,
    NormalAttack = 2,
    Parry = 3,
    Block = 4,
    Hurt = 6,
    Dead = 7,
    Skill1 = 8,
    Skill2 = 9,
    Skill3 = 10,
    Skill1_Start = 11,
    Skill2_Start = 12,
    Skill3_Start = 13,
}

public enum Diraction
{
    [InspectorName("上")]
    Up,
    [InspectorName("下")]
    Down,
    [InspectorName("左")]
    Left,
    [InspectorName("右")]
    Right,
    [InspectorName("左上")]
    LeftUp,
    [InspectorName("右上")]
    RightUp,
    [InspectorName("左下")]
    LeftDown,
    [InspectorName("右下")]
    RightDown,
    [InspectorName("无")]
    None,
}
