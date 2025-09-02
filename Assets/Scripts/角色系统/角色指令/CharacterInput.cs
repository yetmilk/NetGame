using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

public class CharacterInput : MonoBehaviour
{
    public PlayerInputAction action;

    bool isMove;
    bool startSkill1;
    bool startSkill2;
    bool startSkill3;
    NetMonobehavior character;
    IDealActionCommand commandCtrl;
    private void Start()
    {
        commandCtrl = GetComponent<IDealActionCommand>();
        action = PlayerInputManager.Instance.action;
        character = GetComponent<NetMonobehavior>();
        action.GamePlay.Move.started += Move_started;
        action.GamePlay.Move.canceled += Move_canceled;
        action.GamePlay.Parry.started += Parry_started;
        action.GamePlay.NormalAttack.started += NormalAttack_started;
        action.GamePlay.Parry.canceled += Parry_canceled;
        action.GamePlay.Skill1.started += Skill1_started;
        action.GamePlay.Skill1.canceled += Skill1_canceled;
        action.GamePlay.Skill2.started += Skill2_started;
        action.GamePlay.Skill2.canceled += Skill2_canceled;
        action.GamePlay.Skill3.started += Skill3_started;
        action.GamePlay.Skill3.canceled += Skill3_canceled;
        EventCenter.Subscribe(EventCenter.EventId.LogicFrameUpdate, LogicUpdate);
        NetManager.AddMsgListener("MsgInputCommand", ExcuteCommand);
    }





    #region-------------交互处理--------------------
    private void Skill3_started(InputAction.CallbackContext obj)
    {
        Debug.Log("jineng3");
        HandleInput(new InputCommand(InputCommandType.技能3_起势));
    }
    private void Skill3_canceled(InputAction.CallbackContext obj)
    {
        HandleInput(new InputCommand(InputCommandType.技能3));
    }
    private void Skill2_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        startSkill2 = false;
        HandleInput(new InputCommand(InputCommandType.技能2));
    }

    private void Skill2_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        HandleInput(new InputCommand(InputCommandType.技能2_起势));
    }
    private void Skill1_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        HandleInput(new InputCommand(InputCommandType.技能1_起势));
    }
    private void Skill1_canceled(InputAction.CallbackContext context)
    {
        HandleInput(new InputCommand(InputCommandType.技能1));
    }
    private void Parry_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        HandleInput(new InputCommand(InputCommandType.待机));
    }

    /// <summary>
    /// 处理武器1按键交互
    /// </summary>
    /// <param name="obj"></param>
    private void NormalAttack_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {


        bool checkSucc;
        Vector3 dir = GetMouseDirection(out checkSucc);

        if (checkSucc)
            HandleInput(new InputCommand(InputCommandType.普通攻击, dir));
        //Debug.Log(Utility.MouseUtility.GetClickedPosition(mousePos));
    }


    private void Parry_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Vector3 dir = GetInputDiraction();
        HandleInput(new InputCommand(InputCommandType.闪避, dir));
    }

    private void LogicUpdate(object param = null)
    {
        if (isMove)
        {
            Vector3 dir = GetInputDiraction();
            HandleInput(new InputCommand(InputCommandType.移动, dir));
        }
    }

    private void Move_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        isMove = false;
        HandleInput(new InputCommand(InputCommandType.待机));
    }

    private void Move_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Vector3 dir = GetInputDiraction();
        HandleInput(new InputCommand(InputCommandType.移动, dir));
        isMove = true;

    }
    #endregion


    private void HandleInput(InputCommand command)
    {
        if (GetComponent<NetMonobehavior>().IsLocal)
            PlayerInputManager.Instance.HandleInput(command, character.NetID);
    }

    public void ExcuteCommand(MsgBase msgBase)
    {
        MsgInputCommand msg = msgBase as MsgInputCommand;

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

    private void OnDestroy()
    {
        EventCenter.Unsubscribe(EventCenter.EventId.LogicFrameUpdate, LogicUpdate);
        action.GamePlay.Move.started -= Move_started;
        action.GamePlay.Move.canceled -= Move_canceled;
        action.GamePlay.Parry.started -= Parry_started;
        action.GamePlay.NormalAttack.started -= NormalAttack_started;
        action.GamePlay.Parry.canceled -= Parry_canceled;
        NetManager.RemoveMsgListener("MsgInputCommand", ExcuteCommand);
    }

}

