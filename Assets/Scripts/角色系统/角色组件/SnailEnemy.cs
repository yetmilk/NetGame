using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[PrimitiveTaskClass("蜗牛")]
public class SnailEnemy : CharacterController
{
    #region ---------------AI---------------------
    [SelectableMethod]
    public static void MoveTo(Agent agent, Action onComplete)
    {
        if (!agent.GetComponent<NetMonobehavior>().IsLocal) return;
        Vector3 targetPos = agent.state.GetState<Vector3>("敌人位置");
        Vector3 dir = (targetPos - agent.transform.position).normalized;

        PlayerInputManager.Instance.HandleInput(new InputCommand(InputCommandType.移动, dir), agent.GetComponent<NetMonobehavior>().NetID);
        onComplete?.Invoke();
    }

    [SelectableMethod]
    public static void Attack(Agent agent, Action onComplete)
    {

        if (!agent.GetComponent<NetMonobehavior>().IsLocal) return;
        Vector3 targetPos = agent.state.GetState<Vector3>("敌人位置");
        PlayerInputManager.Instance.HandleInput(new InputCommand(InputCommandType.普通攻击, targetPos), agent.GetComponent<NetMonobehavior>().NetID);
        agent.GetComponent<CharacterController>().selfActionCtrl.OnActionExit += (a, b, c) =>
        {
            if (a == ActionTag.NormalAttack)
                onComplete?.Invoke();
        };

    }
    #endregion

    protected override void OnAttackUpdate(ActionObj curActionObj)
    {
        base.OnAttackUpdate(curActionObj);
    }
}
