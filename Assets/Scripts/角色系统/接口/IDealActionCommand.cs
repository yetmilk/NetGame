using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///������Ϊָ��Ľӿڣ��̳иýӿڵ������������Ϊת��ָ���ְ��
/// </summary>
public interface IDealActionCommand
{
    public void HandleInputCommand(InputCommand command);
}
