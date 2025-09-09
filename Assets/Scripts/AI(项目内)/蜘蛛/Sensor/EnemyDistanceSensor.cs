
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDistanceSensor : MonoBehaviour, ISensor
{
    public string enemyTag = "Player";
    public float detectionRange = 10f;
    private Agent agent;
    private GameObject enemy;

    public LayerMask layer;

    public void Initialize(Agent agent)
    {
        this.agent = agent;
        //enemy = GameObject.FindWithTag(enemyTag);
    }


    public void UpdateSensor()
    {

        var cols = Physics.OverlapSphere(transform.position, detectionRange, layer);
        float minDis = float.MaxValue;
        GameObject enemy = null;
        foreach (var item in cols)
        {
            float curDis = Vector3.Distance(item.transform.position, transform.position);
            if (curDis < minDis && item.CompareTag("Player"))
            {
                minDis = curDis;
                enemy = item.gameObject;

            }
        }
        if (enemy != null)
        {
            agent.state.SetState("有敌人", true);
            this.enemy = enemy;
        }
        else
        {
            agent.state.SetState("敌人距离", float.MaxValue);
            agent.state.SetState("有敌人", false);
            return;
        }

        if (enemy == null) return;

        float distance = Vector3.Distance(transform.position, enemy.transform.position);
        bool inRange = distance <= detectionRange;

        //agent.doMain.state.SetState("enemyInRange", inRange);
        agent.state.SetState("敌人距离", distance);
        agent.state.SetState("敌人位置", enemy.transform.position);

        // 如果有敌人健康组件，也更新敌人健康状态
        //var enemyHealthComponent = enemy.GetComponent<EnemyHealth>();
        //if (enemyHealthComponent != null)
        //{
        //    agent.worldState.SetState("enemyHealth", enemyHealthComponent.currentHealth);
        //}
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, detectionRange);
    }
}