using Unity.AppUI.UI;
using UnityEngine;
/// <summary>
/// ���ԭʼ����
/// </summary>
[System.Serializable]
public enum InputCommandType
{
    ���� = 0,
    �ƶ� = 1,
    ���� = 2,
    ��ͨ���� = 3,
    ����1_���� = 9,
    ����1 = 4,
    ����2_���� = 11,
    ����2 = 5,
    ����3_���� = 10,
    ����3 = 6,
    ���� = 7,
    �� = 8,
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