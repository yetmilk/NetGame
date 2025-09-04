using PixPlays.ElementalVFX;
using System;
using System.Collections;
using UnityEngine;

[PrimitiveTaskClass("蜘蛛")]
public class SpiderEnemy : CharacterController
{
    public Transform attackTransform;
    #region ---------------AI---------------------
    [SelectableMethod]
    public static void MoveTo(Agent agent, Action onComplete)
    {
        if (!agent.GetComponent<NetMonobehavior>().IsLocal) return;
        Vector3 targetPos = agent.state.GetState<Vector3>("敌人位置");
        Vector3 dir = (targetPos - agent.transform.position).normalized;
        Debug.Log(dir);

        PlayerInputManager.Instance.HandleInput(new InputCommand(InputCommandType.移动, dir), agent.GetComponent<NetMonobehavior>().NetID);
        onComplete?.Invoke();
    }

    [SelectableMethod]
    public static void Attack(Agent agent, Action onComplete)
    {

        if (!agent.GetComponent<NetMonobehavior>().IsLocal) return;
        Vector3 targetPos = agent.state.GetState<Vector3>("敌人位置");
        PlayerInputManager.Instance.HandleInput(new InputCommand(InputCommandType.普通攻击, targetPos), agent.GetComponent<NetMonobehavior>().NetID);
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

    #endregion


    protected override void OnAttackEnter(ActionObj curActionObj)
    {
        base.OnAttackEnter(curActionObj);

        


    }

    protected override void OnAttackUpdate(ActionObj curActionObj)
    {
        base.OnAttackUpdate(curActionObj);

        if(curActionObj.curLifeFrame ==10&&IsLocal)
        {
            var go = LoadManager.Instance.NetInstantiate("VFX_蜘蛛_攻击");

            DamageInfo damageInfo = new DamageInfo()
            {
                damageFormulaType = DamageFormulaType.通用,
                attackTag = ActionTag.NormalAttack,
            };
            go.GetComponent<InstantiateObjBase>().Init(this);

            VfxData newData = new VfxData(attackTransform, curActionObj.direction, 3f, .5f, damageInfo);
            go.GetComponent<BaseVfx>().Play(newData);
        }
    }

    protected override void OnAttackExit(ActionObj curActionObj, ActionObj nextActionObj)
    {
        base.OnAttackExit(curActionObj, nextActionObj);
    }
}
