using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModleInput : InputBase
{

    public float sendInterval = .1f;
    private float timer = 0;
    public override void Init()
    {
        base.Init();
        PlayerInputManager.Instance.EnableInput();
        EventCenter.Subscribe(EventCenter.EventId.LogicFrameUpdate, LogicUpdate);
        timer = 0;
    }


    private void LogicUpdate(object param = null)
    {


        //float inputX = PlayerInputManager.Instance.GetHorizontalAxis();
        //float inputY = PlayerInputManager.Instance.GetVerticalAxis();
        //Vector2 inputDir = new Vector2(inputX, inputY);

        //if (inputX != 0 || inputY != 0)
        //{
        //    Vector3 moveDir = new Vector3(inputDir.x, 0, inputDir.y);
        //    InputCommand command = new InputCommand(InputCommandType.ÒÆ¶¯, moveDir);
        //    // Debug.Log("Run");
        //    commandCtrl.HandleInputCommand(command);

        //    MsgInputCommand msg = new MsgInputCommand((int)command.type, command.diraction.x, command.diraction.y, command.diraction.z); ;

        //    msg.playerId = PlayerManager.Instance.selfId;
        //    NetManager.Send(msg);
        //}
        //else
        //{
        //    InputCommand command = new InputCommand(InputCommandType.´ý»ú);
        //    commandCtrl.HandleInputCommand(command);

        //    MsgInputCommand msg = new MsgInputCommand((int)command.type, command.diraction.x, command.diraction.y, command.diraction.z);

        //    msg.playerId = PlayerManager.Instance.selfId;
        //    NetManager.Send(msg);
        //}

    }

}
