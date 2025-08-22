using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    // ����ʵ��
    private static T instance;

    // �̰߳�ȫ��
    private static readonly object _lock = new object();

    // ����Ƿ��ѳ�ʼ��
    private bool _isInitialized = false;

    /// <summary>
    /// ȫ�ַ��ʵ㣬��ȡ����ʵ��
    /// </summary>
    public static T Instance
    {
        get
        {
            // ����Ƿ��ڱ༭ģʽ��δ����
            if (!Application.isPlaying)
            {
                Debug.LogWarning($"�ڱ༭ģʽ�·��� {typeof(T).Name} ����ʵ��");
                return null;
            }

            lock (_lock)
            {
                if (instance == null)
                {
                    // �����ڳ����в�������ʵ��
                    instance = FindObjectOfType<T>();

                    // ���û�ҵ����򴴽���ʵ��
                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject($"[Singleton] {typeof(T).Name}");
                        instance = singletonObject.AddComponent<T>();

                        // ��Ϊ�糡��������
                        DontDestroyOnLoad(singletonObject);

                        Debug.Log($"��������ʵ��: {typeof(T).Name}");
                    }
                    else
                    {
                        Debug.Log($"�ҵ��Ѵ��ڵĵ���ʵ��: {typeof(T).Name}");
                    }
                }

                return instance;
            }
        }
    }

    /// <summary>
    /// ȷ��ʵ��Ψһ��
    /// </summary>
    protected virtual void Awake()
    {
        if (instance == null)
        {
            // ����ǰʵ���ǵ�һ��������Ϊ����
            instance = this as T;
            DontDestroyOnLoad(gameObject);
            Initialize();
            _isInitialized = true;
        }
        else if (instance != this)
        {
            // ����������ʵ���������ٵ�ǰʵ��
            Debug.LogWarning($"��⵽�ظ��� {typeof(T).Name} ʵ�������Զ�����");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ��ʼ���������������дʵ���Զ����ʼ���߼�
    /// </summary>
    protected virtual void Initialize()
    {

    }

    /// <summary>
    /// ��鵥���Ƿ��ѳ�ʼ��
    /// </summary>
    public bool IsInitialized => _isInitialized;

    /// <summary>
    /// ����ʱ����
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            Cleanup();
        }
    }

    /// <summary>
    /// ���������������дʵ����Դ�ͷ��߼�
    /// </summary>
    protected virtual void Cleanup()
    {

    }

    /// <summary>
    /// ��ֹ�ڱ༭ģʽ�±�ʵ����
    /// </summary>
    protected virtual void OnValidate()
    {
        if (Application.isPlaying && instance != null && instance != this)
        {
            Debug.LogWarning($"�����д��ڶ�� {typeof(T).Name} ʵ��������ʱ���Զ����ٶ���ʵ��");
        }
    }
}
