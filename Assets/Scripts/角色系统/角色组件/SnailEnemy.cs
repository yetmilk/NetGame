using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[PrimitiveTaskClass("蜗牛")]
public class SnailEnemy : CharacterController
{
  [SelectableMethod]
    public static void MoveTo(Agent agent, Action onComplete)
    {
        if (!agent.GetComponent<NetMonobehavior>().IsLocal) return;
        agent.StartCoroutine(MoveCoro(agent, onComplete));
        //Debug.LogWarning(111);
    }

    static IEnumerator MoveCoro(Agent agent, Action onComplete)
    {
        Vector3 targetPos = agent.state.GetState<Vector3>("敌人位置");


        float maxMoveTime = 10f; // 设置最大移动时间
        float startTime = Time.time;

        while (Vector3.Distance(agent.transform.position, agent.state.GetState<Vector3>("敌人位置")) > 3f)
        {
            targetPos = agent.state.GetState<Vector3>("敌人位置");
            // 超时退出
            //if (Time.time - startTime > maxMoveTime)
            //{
            //    Debug.Log("Move timed out");
            //    //agent.Plan();
            //    break;
            //}

            Vector3 dir = (targetPos - agent.transform.position).normalized;
            Debug.Log(dir);
            //agent.GetComponent<IDealActionCommand>().HandleInputCommand(new InputCommand(InputCommandType.移动, dir));
            PlayerInputManager.Instance.HandleInput(new InputCommand(InputCommandType.移动, dir), agent.GetComponent<NetMonobehavior>().NetID);

            yield return null;
        }
        //agent.GetComponent<IDealActionCommand>().HandleInputCommand(new InputCommand(InputCommandType.移动));
        //PlayerInputManager.Instance.HandleInput(new InputCommand(InputCommandType.待机), agent.GetComponent<NetMonobehavior>().NetID);
        Debug.Log("Arrived at enemy position");
        onComplete?.Invoke();
    }
 [SelectableMethod]
    public static void Attack(Agent agent, Action onComplete)
    {

        if (!agent.GetComponent<NetMonobehavior>().IsLocal) return;
        Vector3 attackDir = (agent.state.GetState<Vector3>("敌人位置") - agent.transform.position).normalized;
        PlayerInputManager.Instance.HandleInput(new InputCommand(InputCommandType.普通攻击, attackDir), agent.GetComponent<NetMonobehavior>().NetID);
        onComplete?.Invoke();
    }
}
