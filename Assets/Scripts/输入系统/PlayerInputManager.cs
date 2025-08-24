using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class PlayerInputManager : Singleton<PlayerInputManager>
{
    public InputBase activeInput;

    public PlayerInputAction action;
    protected void Start()
    {

        action = new PlayerInputAction();
        action.Enable();
    }

    public void EnableInput()
    {
        activeInput.Enable();
    }

    public void DisableInput()
    {
        activeInput.Disable();
    }
    public void HandleInput(InputCommand command, string netId)
    {

        MsgInputCommand msg = new MsgInputCommand((int)command.type, command.direction);
        msg.NetId = netId;
        NetManager.Send(msg);
    }
    public void HandleInput(string actionName, ActionTag actionTag, Vector3 dir, string netId)
    {
        MsgInputCommand msg = new MsgInputCommand(-1, dir);

        msg.NetId = netId;

        msg.actionName = actionName;
        msg.actionTag = (int)actionTag;

        NetManager.Send(msg);
    }
    public Vector3 GetInputDiraction()
    {
        Vector2 inputDir = action.GamePlay.Move.ReadValue<Vector2>();

        return new Vector3(inputDir.x, 0, inputDir.y);
    }

    public Vector3 GetMouseDirection(out bool checkSucc)
    {
        Vector2 inputDir = action.GamePlay.MousePosition.ReadValue<Vector2>();
        return MouseUtility.GetClickedPosition(inputDir, "Ground", out checkSucc);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
