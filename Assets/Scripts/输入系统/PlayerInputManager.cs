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

    // ������������ָͬ��ķ���Ƶ�ʣ��룩
    public float sameCommandThreshold = 0.1f;

    // ��������¼������͵�ָ���ʱ���
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

    // �Ż������Ƶ�ʿ���
    public void HandleInput(InputCommand command, string netId)
    {
        // ����Ψһ����ʶ��ͬ���͵�ָ��
        string commandKey = $"{netId}_{command.type}";

        // ����Ƿ���Ҫ���͸�ָ��
        if (ShouldSendCommand(commandKey))
        {
            MsgInputCommand msg = new MsgInputCommand((int)command.type, command.direction);
            msg.NetId = netId;
            commands.Add(msg);

            // �����������ʱ��
            UpdateLastCommandTime(commandKey);
        }
    }

    // �Ż������Ƶ�ʿ���
    public void HandleInput(string actionName, ActionTag actionTag, Vector3 dir, string netId)
    {
        // ����Ψһ����ʶ��ͬ���͵�ָ��
        string commandKey = $"{netId}_{actionName}_{actionTag}";

        // ����Ƿ���Ҫ���͸�ָ��
        if (ShouldSendCommand(commandKey))
        {
            MsgInputCommand msg = new MsgInputCommand(-1, dir);
            msg.NetId = netId;
            msg.actionName = actionName;
            msg.actionTag = (int)actionTag;
            commands.Add(msg);

            // �����������ʱ��
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

    // ����������Ƿ�Ӧ�÷���ָ�����ʱ����ֵ��
    private bool ShouldSendCommand(string commandKey)
    {
        // �������ָ�ֱ��������
        if (!_lastCommandTimes.ContainsKey(commandKey))
            return true;

        // �������ϴη����Ƿ񳬹���ֵ
        return Time.time - _lastCommandTimes[commandKey] >= sameCommandThreshold;
    }

    // ����������ָ����������ʱ��
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
