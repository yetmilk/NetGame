using static EventCenter;
using UnityEngine;

/// <summary>
/// �̶�֡�ʹ����� - �ṩ�ȶ����߼�֡����Ⱦ֡�¼���֧��ȫ�ַ���
/// </summary>
public class FrameManager : Singleton<FrameManager>
{

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        // ��ʼ��֡���ʱ��
        logicFrameInterval = 1f / targetLogicFrameRate;
        renderFrameInterval = 1f / targetRenderFrameRate;

        // ���ó�ʼ����ʱ���
        nextLogicFrameTime = Time.time + logicFrameInterval;
        nextRenderFrameTime = Time.time + renderFrameInterval;
    }

    // ��̬���ԣ�����ʵ�����ɷ���
    public static long CurrentLogicFrame => Instance.currentLogicFrame;
    public static long CurrentRenderFrame => Instance.currentRenderFrame;
    public static float LogicFrameRate => Instance.targetLogicFrameRate;
    public static float RenderFrameRate => Instance.targetRenderFrameRate;
    public static float TimeScale => Instance.timeScale;
    public static bool IsPaused => Instance.isPaused;

    public static float LogicFrameInterval => Instance.logicFrameInterval;

    [Tooltip("Ŀ���߼�����֡�� (FPS) - Ӱ������ģ�����Ϸ�߼�ִ��Ƶ��")]
    [SerializeField] private float targetLogicFrameRate = 60f;

    [Tooltip("Ŀ����Ⱦ֡�� (FPS) - Ӱ�컭�����Ƶ�ʺͲ�ֵ����")]
    [SerializeField] private float targetRenderFrameRate = 60f;

    [Range(0f, 2f)]
    [Tooltip("ȫ��ʱ���������� - 0��ʾ��ͣ��1��ʾ�����ٶ�")]
    [SerializeField] private float timeScale = 1f;

    // �ڲ���ʱ������
    private float logicFrameInterval;     // �߼�֡���ʱ��(��)
    private float renderFrameInterval;    // ��Ⱦ֡���ʱ��(��)
    private float nextLogicFrameTime;     // ��һ�߼�֡����ʱ���
    private float nextRenderFrameTime;    // ��һ��Ⱦ֡����ʱ���
    private long currentLogicFrame = 0;   // ��ǰ�߼�֡����
    private long currentRenderFrame = 0;  // ��ǰ��Ⱦ֡����
    private bool isPaused = false;        // ��ͣ״̬��־

    // ������Ⱦ֡����
    private void Update()
    {
        if (isPaused) return;

        // ����ʵ����Ⱦ���ܴ�����Ⱦ֡
        if (Time.time >= nextRenderFrameTime)
        {
            currentRenderFrame++;
            nextRenderFrameTime += renderFrameInterval;

            // ������Ⱦ֡�����¼���Я����ֵ����
            Publish(EventId.RenderFrameUpdate, new RenderFrameData
            {
                FrameNumber = currentRenderFrame,
                InterpolationFactor = CalculateInterpolationFactor()
            });
        }
    }

    // ����̶�֡�ʵ��߼�����
    private void FixedUpdate()
    {
        if (isPaused) return;

        currentLogicFrame++;

        // �����߼�֡�����¼�
        Publish(EventId.LogicFrameUpdate, currentLogicFrame);
    }

    /// <summary>
    /// ���㵱ǰʱ��������߼�֮֡��Ĳ�ֵλ��
    /// </summary>
    /// <returns>��ֵ����(0-1)��0��ʾ��һ�߼�֡��1��ʾ��һ�߼�֡</returns>
    private float CalculateInterpolationFactor()
    {
        // ��ֹ��ֵ���ӳ�����Χ
        return Mathf.Clamp01((Time.unscaledTime - (nextLogicFrameTime - logicFrameInterval)) / logicFrameInterval);
    }

    /// <summary>
    /// ����ȫ��ʱ������
    /// </summary>
    /// <param name="newTimeScale">�µ�ʱ����������(0-2)</param>
    public void SetTimeScale(float newTimeScale)
    {
        // ȷ��ʱ���������Ӳ�С��0
        timeScale = Mathf.Max(0f, newTimeScale);
    }

    /// <summary>
    /// ��ͣ��ָ�֡����
    /// </summary>
    /// <param name="paused">true��ʾ��ͣ��false��ʾ�ָ�</param>
    public void SetPaused(bool paused)
    {
        isPaused = paused;

        // �ָ�ʱ���ü�ʱ��������ʱ����Ծ
        if (!paused)
        {
            nextLogicFrameTime = Time.unscaledTime + logicFrameInterval;
            nextRenderFrameTime = Time.unscaledTime + renderFrameInterval;
        }
    }
}