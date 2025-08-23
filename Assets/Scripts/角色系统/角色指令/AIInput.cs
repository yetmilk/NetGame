using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class AIInput : MonoBehaviour
{
    IDealActionCommand commandCtrl;
    NetMonobehavior character;

    private void Start()
    {
        commandCtrl = GetComponent<IDealActionCommand>();
        NetManager.AddMsgListener("MsgInputCommand", ExcuteCommand);
        character = GetComponent<NetMonobehavior>();
    }
    public void ExcuteCommand(MsgBase msgBase)
    {
        MsgInputCommand msg = msgBase as MsgInputCommand;

       Debug.Log("��ǰ��ɫ��"+character.gameObject.name+"��ǰ��ɫnetid"+character.NetID+" ����netid"+msg.NetId);
        if (msg.NetId == character.NetID)
        {
            if (msg.type != -1)
            {
                Vector3 dir = new Vector3(msg.directionX, msg.directionY, msg.directionZ);
                InputCommand command = new InputCommand((InputCommandType)msg.type, dir);
                commandCtrl.HandleInputCommand(command);
            }
            else
            {
                Vector3 dir = new Vector3(msg.directionX, msg.directionY, msg.directionZ);
                commandCtrl.HandleInputCommand(msg.actionName, (ActionTag)msg.actionTag, dir);
            }

        }

    }

    private void OnDestroy()
    {
        NetManager.RemoveMsgListener("MsgInputCommand", ExcuteCommand);
    }
}

[System.Serializable]
public class AIActionDesign
{
    public string Name;
    public string param;
}
