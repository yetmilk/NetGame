using Unity.AppUI.UI;
using UnityEngine;
/// <summary>
/// 玩家原始输入
/// </summary>
[System.Serializable]
public enum InputCommandType
{
    待机 = 0,
    移动 = 1,
    闪避 = 2,
    普通攻击 = 3,
    技能1_起势 = 9,
    技能1 = 4,
    技能2_起势 = 11,
    技能2 = 5,
    技能3_起势 = 10,
    技能3 = 6,
    交互 = 7,
    无 = 8,
}

[System.Serializable]
public class InputCommand
{
    public InputCommandType type;
    public ActionTag actionTag;
    public Vector3 direction;

    public InputCommand(InputCommandType type,Vector3 direction = default,ActionTag actionTag = ActionTag.None)
    {
        this.type = type;
        this.actionTag = actionTag;
        this.direction = direction;
    }
}