/// <summary>
/// ��Ⱦ֡���ݽṹ - ������Ⱦ֡��������Ĺؼ���Ϣ
/// </summary>
public struct RenderFrameData
{
    /// <summary>
    /// ��ǰ��Ⱦ֡�ı��
    /// ����Ϸ������ʼ�ۼƵ���Ⱦ֡�������ڱ�ʶΨһ֡
    /// </summary>
    public long FrameNumber;

    /// <summary>
    /// ��ֵ���� - ��������֡�߼�����֮��ƽ����Ⱦ
    /// ֵ��Χ��0��1����ʾ������һ�߼�֡�Ľ��Ȱٷֱ�
    /// ���磺0.5��ʾλ����һ�߼�֡����һ�߼�֡���м��
    /// </summary>
    public float InterpolationFactor;
}