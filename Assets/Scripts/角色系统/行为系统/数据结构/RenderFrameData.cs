/// <summary>
/// 渲染帧数据结构 - 传递渲染帧更新所需的关键信息
/// </summary>
public struct RenderFrameData
{
    /// <summary>
    /// 当前渲染帧的编号
    /// 从游戏启动开始累计的渲染帧数，用于标识唯一帧
    /// </summary>
    public long FrameNumber;

    /// <summary>
    /// 插值因子 - 用于在两帧逻辑更新之间平滑渲染
    /// 值范围从0到1，表示距离上一逻辑帧的进度百分比
    /// 例如：0.5表示位于上一逻辑帧和下一逻辑帧的中间点
    /// </summary>
    public float InterpolationFactor;
}