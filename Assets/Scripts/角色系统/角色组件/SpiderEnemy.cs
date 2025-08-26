using System;
using System.Collections;
using UnityEngine;

[PrimitiveTaskClass("蜘蛛")]
public class SpiderEnemy : CharacterBehaviourController
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

        while (Vector3.Distance(agent.transform.position, agent.state.GetState<Vector3>("敌人位置")) > 2f)
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
    [SelectableMethod]
    public static void Rest(Agent agent, Action onComplete)
    {
        if (!agent.GetComponent<NetMonobehavior>().IsLocal) return;
        agent.StartCoroutine(RestCoro(agent, onComplete));
    }
    static IEnumerator RestCoro(Agent agent, Action onComplete)
    {
        float maxRestTime = (float)agent.state.GetState<double>("休息时间");
        float startTime = Time.time;

        // 直接使用 maxRestTime 作为总时长，而非动态计算
        while (Time.time - startTime < maxRestTime)
        {
            // 仅在必要时更新状态（例如每0.5秒更新一次）
            if ((Time.time - startTime) % 0.5f < 0.1f)
            {
                agent.state.SetState("当前休息时间", Time.time - startTime);
            }

            Vector3 targetPos = agent.state.GetState<Vector3>("敌人位置");
            Vector3 dir = (agent.transform.position - targetPos).normalized;

            PlayerInputManager.Instance.HandleInput(new InputCommand(InputCommandType.待机, -dir), agent.GetComponent<NetMonobehavior>().NetID);
            // 仅在距离变化时更新动作（避免重复添加相同命令）
            if (Vector3.Distance(agent.transform.position, targetPos) <= 5f)
            {
                PlayerInputManager.Instance.HandleInput("后退", ActionTag.Move, -dir, agent.GetComponent<NetMonobehavior>().NetID);
            }


            yield return null;
        }

        // 休息结束，重置状态
        agent.state.SetState("当前休息时间", 0.0);
        Debug.Log("休息结束！！！");
        onComplete?.Invoke();
    }
}
