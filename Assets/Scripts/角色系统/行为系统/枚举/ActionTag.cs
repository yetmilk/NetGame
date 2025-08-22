using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionTag
{
    None = 5,
    Idle = 0,
    Move = 1,
    Attack = 2,
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
    [InspectorName("��")]
    Up,
    [InspectorName("��")]
    Down,
    [InspectorName("��")]
    Left,
    [InspectorName("��")]
    Right,
    [InspectorName("����")]
    LeftUp,
    [InspectorName("����")]
    RightUp,
    [InspectorName("����")]
    LeftDown,
    [InspectorName("����")]
    RightDown,
    [InspectorName("��")]
    None,
}
