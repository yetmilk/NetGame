

// 感知组件接口 - 用于更新世界状态
public interface ISensor
{
    void Initialize(Agent agent);
    void UpdateSensor();
}