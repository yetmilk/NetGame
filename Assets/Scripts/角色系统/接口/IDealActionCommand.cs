using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///处理行为指令的接口，继承该接口的类具有生成行为转换指令的职责
/// </summary>
public interface IDealActionCommand
{
    public void HandleInputCommand(InputCommand command);
}
