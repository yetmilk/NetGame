using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 指令数据结构
public class ActionCommand
{
    public string actionId;       // 行为ID
    public ActionTag commandType;//指令类型

    public float startPercent;//下一个行为从其自身的百分之多少开始(默认从0开始)

    public Vector3 direction;


    public ActionCommand(string actionId, ActionTag type, Vector3 direction = default, float startPercent = 0f)
    {
        this.actionId = actionId;
        this.commandType = type;
        this.startPercent = startPercent;
        this.direction = direction;
    }
}

public enum CommandType
{

}
