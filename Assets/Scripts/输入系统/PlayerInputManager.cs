using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class PlayerInputManager : Singleton<PlayerInputManager>
{
    public InputBase activeInput;
    public PlayerInputAction action;
    public List<MsgInputCommand> commands = new List<MsgInputCommand>();
    public float syncInterval = .1f;

    // 新增：控制相同指令的发送频率（秒）
    public float sameCommandThreshold = 0.1f;

    // 新增：记录最近发送的指令及其时间戳
    private Dictionary<string, float> _lastCommandTimes = new Dictionary<string, float>();

    protected void Start()
    {
        action = new PlayerInputAction();
        action.Enable();
        StartCoroutine(SyncInputCoro());
    }

    IEnumerator SyncInputCoro()
    {
        while (true)
        {
            MsgCmdCollection msg = new MsgCmdCollection();
            msg.cmds = commands.ToArray();

            if (msg.cmds.Length != 0)
            {
                NetManager.Send(msg);
                commands.Clear();
            }

            yield return new WaitForSeconds(syncInterval);
        }
    }

    public void EnableInput()
    {
        activeInput.Enable();
    }

    public void DisableInput()
    {
        activeInput.Disable();
    }

    // 优化：添加频率控制
    public void HandleInput(InputCommand command, string netId)
    {
        // 创建唯一键标识相同类型的指令
        string commandKey = $"{netId}_{command.type}";

        // 检查是否需要发送该指令
        if (ShouldSendCommand(commandKey))
        {
            MsgInputCommand msg = new MsgInputCommand((int)command.type, command.direction);
            msg.NetId = netId;
            commands.Add(msg);

            // 更新最近发送时间
            UpdateLastCommandTime(commandKey);
        }
    }

    // 优化：添加频率控制
    public void HandleInput(string actionName, ActionTag actionTag, Vector3 dir, string netId)
    {
        // 创建唯一键标识相同类型的指令
        string commandKey = $"{netId}_{actionName}_{actionTag}";

        // 检查是否需要发送该指令
        if (ShouldSendCommand(commandKey))
        {
            MsgInputCommand msg = new MsgInputCommand(-1, dir);
            msg.NetId = netId;
            msg.actionName = actionName;
            msg.actionTag = (int)actionTag;
            commands.Add(msg);

            // 更新最近发送时间
            UpdateLastCommandTime(commandKey);
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

    // 新增：检查是否应该发送指令（基于时间阈值）
    private bool ShouldSendCommand(string commandKey)
    {
        // 如果是新指令，直接允许发送
        if (!_lastCommandTimes.ContainsKey(commandKey))
            return true;

        // 检查距离上次发送是否超过阈值
        return Time.time - _lastCommandTimes[commandKey] >= sameCommandThreshold;
    }

    // 新增：更新指令的最近发送时间
    private void UpdateLastCommandTime(string commandKey)
    {
        if (_lastCommandTimes.ContainsKey(commandKey))
            _lastCommandTimes[commandKey] = Time.time;
        else
            _lastCommandTimes.Add(commandKey, Time.time);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _lastCommandTimes.Clear();
    }
}
