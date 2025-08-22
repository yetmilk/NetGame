using static EventCenter;
using UnityEngine;

/// <summary>
/// 固定帧率管理器 - 提供稳定的逻辑帧和渲染帧事件，支持全局访问
/// </summary>
public class FrameManager : Singleton<FrameManager>
{

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        // 初始化帧间隔时间
        logicFrameInterval = 1f / targetLogicFrameRate;
        renderFrameInterval = 1f / targetRenderFrameRate;

        // 设置初始触发时间点
        nextLogicFrameTime = Time.time + logicFrameInterval;
        nextRenderFrameTime = Time.time + renderFrameInterval;
    }

    // 静态属性，无需实例即可访问
    public static long CurrentLogicFrame => Instance.currentLogicFrame;
    public static long CurrentRenderFrame => Instance.currentRenderFrame;
    public static float LogicFrameRate => Instance.targetLogicFrameRate;
    public static float RenderFrameRate => Instance.targetRenderFrameRate;
    public static float TimeScale => Instance.timeScale;
    public static bool IsPaused => Instance.isPaused;

    public static float LogicFrameInterval => Instance.logicFrameInterval;

    [Tooltip("目标逻辑更新帧率 (FPS) - 影响物理模拟和游戏逻辑执行频率")]
    [SerializeField] private float targetLogicFrameRate = 60f;

    [Tooltip("目标渲染帧率 (FPS) - 影响画面更新频率和插值计算")]
    [SerializeField] private float targetRenderFrameRate = 60f;

    [Range(0f, 2f)]
    [Tooltip("全局时间缩放因子 - 0表示暂停，1表示正常速度")]
    [SerializeField] private float timeScale = 1f;

    // 内部计时器变量
    private float logicFrameInterval;     // 逻辑帧间隔时间(秒)
    private float renderFrameInterval;    // 渲染帧间隔时间(秒)
    private float nextLogicFrameTime;     // 下一逻辑帧触发时间点
    private float nextRenderFrameTime;    // 下一渲染帧触发时间点
    private long currentLogicFrame = 0;   // 当前逻辑帧计数
    private long currentRenderFrame = 0;  // 当前渲染帧计数
    private bool isPaused = false;        // 暂停状态标志

    // 处理渲染帧更新
    private void Update()
    {
        if (isPaused) return;

        // 基于实际渲染性能触发渲染帧
        if (Time.time >= nextRenderFrameTime)
        {
            currentRenderFrame++;
            nextRenderFrameTime += renderFrameInterval;

            // 发布渲染帧更新事件，携带插值因子
            Publish(EventId.RenderFrameUpdate, new RenderFrameData
            {
                FrameNumber = currentRenderFrame,
                InterpolationFactor = CalculateInterpolationFactor()
            });
        }
    }

    // 处理固定帧率的逻辑更新
    private void FixedUpdate()
    {
        if (isPaused) return;

        currentLogicFrame++;

        // 发布逻辑帧更新事件
        Publish(EventId.LogicFrameUpdate, currentLogicFrame);
    }

    /// <summary>
    /// 计算当前时间点在两逻辑帧之间的插值位置
    /// </summary>
    /// <returns>插值因子(0-1)，0表示上一逻辑帧，1表示下一逻辑帧</returns>
    private float CalculateInterpolationFactor()
    {
        // 防止插值因子超出范围
        return Mathf.Clamp01((Time.unscaledTime - (nextLogicFrameTime - logicFrameInterval)) / logicFrameInterval);
    }

    /// <summary>
    /// 设置全局时间缩放
    /// </summary>
    /// <param name="newTimeScale">新的时间缩放因子(0-2)</param>
    public void SetTimeScale(float newTimeScale)
    {
        // 确保时间缩放因子不小于0
        timeScale = Mathf.Max(0f, newTimeScale);
    }

    /// <summary>
    /// 暂停或恢复帧更新
    /// </summary>
    /// <param name="paused">true表示暂停，false表示恢复</param>
    public void SetPaused(bool paused)
    {
        isPaused = paused;

        // 恢复时重置计时器，避免时间跳跃
        if (!paused)
        {
            nextLogicFrameTime = Time.unscaledTime + logicFrameInterval;
            nextRenderFrameTime = Time.unscaledTime + renderFrameInterval;
        }
    }
}