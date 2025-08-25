using System;
using System.Collections;
using UnityEngine;

[PrimitiveTaskClass("֩��")]
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
        Vector3 targetPos = agent.state.GetState<Vector3>("����λ��");


        float maxMoveTime = 10f; // ��������ƶ�ʱ��
        float startTime = Time.time;

        while (Vector3.Distance(agent.transform.position, agent.state.GetState<Vector3>("����λ��")) > 2f)
        {
            targetPos = agent.state.GetState<Vector3>("����λ��");
            // ��ʱ�˳�
            //if (Time.time - startTime > maxMoveTime)
            //{
            //    Debug.Log("Move timed out");
            //    //agent.Plan();
            //    break;
            //}

            Vector3 dir = (targetPos - agent.transform.position).normalized;
            Debug.Log(dir);
            //agent.GetComponent<IDealActionCommand>().HandleInputCommand(new InputCommand(InputCommandType.�ƶ�, dir));
            PlayerInputManager.Instance.HandleInput(new InputCommand(InputCommandType.�ƶ�, dir), agent.GetComponent<NetMonobehavior>().NetID);

            yield return null;
        }
        //agent.GetComponent<IDealActionCommand>().HandleInputCommand(new InputCommand(InputCommandType.�ƶ�));
        //PlayerInputManager.Instance.HandleInput(new InputCommand(InputCommandType.����), agent.GetComponent<NetMonobehavior>().NetID);
        Debug.Log("Arrived at enemy position");
        onComplete?.Invoke();
    }

    [SelectableMethod]
    public static void Attack(Agent agent, Action onComplete)
    {

        if (!agent.GetComponent<NetMonobehavior>().IsLocal) return;
        Vector3 attackDir = (agent.state.GetState<Vector3>("����λ��") - agent.transform.position).normalized;
        PlayerInputManager.Instance.HandleInput(new InputCommand(InputCommandType.��ͨ����, attackDir), agent.GetComponent<NetMonobehavior>().NetID);
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
        float maxRestTime = (float)agent.state.GetState<double>("��Ϣʱ��");
        float startTime = Time.time;

        // ֱ��ʹ�� maxRestTime ��Ϊ��ʱ�������Ƕ�̬����
        while (Time.time - startTime < maxRestTime)
        {
            // ���ڱ�Ҫʱ����״̬������ÿ0.5�����һ�Σ�
            if ((Time.time - startTime) % 0.5f < 0.1f)
            {
                agent.state.SetState("��ǰ��Ϣʱ��", Time.time - startTime);
            }

            Vector3 targetPos = agent.state.GetState<Vector3>("����λ��");
            Vector3 dir = (agent.transform.position - targetPos).normalized;

            PlayerInputManager.Instance.HandleInput(new InputCommand(InputCommandType.����, -dir), agent.GetComponent<NetMonobehavior>().NetID);
            // ���ھ���仯ʱ���¶����������ظ������ͬ���
            if (Vector3.Distance(agent.transform.position, targetPos) <= 5f)
            {
                PlayerInputManager.Instance.HandleInput("����", ActionTag.Move, -dir, agent.GetComponent<NetMonobehavior>().NetID);
            }


            yield return null;
        }

        // ��Ϣ����������״̬
        agent.state.SetState("��ǰ��Ϣʱ��", 0.0);
        Debug.Log("��Ϣ����������");
        onComplete?.Invoke();
    }
}
