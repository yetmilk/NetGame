using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ָ�����ݽṹ
public class ActionCommand
{
    public string actionId;       // ��ΪID
    public ActionTag commandType;//ָ������

    public float startPercent;//��һ����Ϊ��������İٷ�֮���ٿ�ʼ(Ĭ�ϴ�0��ʼ)

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
