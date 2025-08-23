
using UnityEngine;

public class EnemyDistanceSensor : MonoBehaviour, ISensor
{
    public string enemyTag = "Player";
    public float detectionRange = 10f;
    private Agent agent;
    private GameObject enemy;

    public void Initialize(Agent agent)
    {
        this.agent = agent;
        enemy = GameObject.FindWithTag(enemyTag);
    }

    public void UpdateSensor()
    {
        if (enemy == null)
        {
            //agent.doMain.state.SetState("enemyInRange", false);
            agent.state.SetState("敌人距离", float.MaxValue);
            return;
        }

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
}