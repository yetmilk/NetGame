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
        NetManager.AddMsgListener("MsgCmdCollection", ExcuteCommand);
        character = GetComponent<NetMonobehavior>();
    }
    public void ExcuteCommand(MsgBase msgBase)
    {
        MsgCmdCollection msg = msgBase as MsgCmdCollection;

        foreach (var cmd in msg.cmds)
        {
            if (cmd.NetId == character.NetID)
            {
                if (cmd.type != -1)
                {
                    Vector3 dir = new Vector3(cmd.directionX, cmd.directionY, cmd.directionZ);
                    InputCommand command = new InputCommand((InputCommandType)cmd.type, dir);
                    commandCtrl.HandleInputCommand(command);
                }
                else
                {
                    Vector3 dir = new Vector3(cmd.directionX, cmd.directionY, cmd.directionZ);
                    commandCtrl.HandleInputCommand(cmd.actionName, (ActionTag)cmd.actionTag, dir);
                }

            }
        }


    }

    private void OnDestroy()
    {
        NetManager.RemoveMsgListener("MsgCmdCollection", ExcuteCommand);
    }
}

[System.Serializable]
public class AIActionDesign
{
    public string Name;
    public string param;
}
