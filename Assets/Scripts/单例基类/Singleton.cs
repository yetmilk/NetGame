using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    // 单例实例
    private static T instance;

    // 线程安全锁
    private static readonly object _lock = new object();

    // 标记是否已初始化
    private bool _isInitialized = false;

    /// <summary>
    /// 全局访问点，获取单例实例
    /// </summary>
    public static T Instance
    {
        get
        {
            // 检查是否在编辑模式且未运行
            if (!Application.isPlaying)
            {
                Debug.LogWarning($"在编辑模式下访问 {typeof(T).Name} 单例实例");
                return null;
            }

            lock (_lock)
            {
                if (instance == null)
                {
                    // 尝试在场景中查找已有实例
                    instance = FindObjectOfType<T>();

                    // 如果没找到，则创建新实例
                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject($"[Singleton] {typeof(T).Name}");
                        instance = singletonObject.AddComponent<T>();

                        // 设为跨场景不销毁
                        DontDestroyOnLoad(singletonObject);

                        Debug.Log($"创建单例实例: {typeof(T).Name}");
                    }
                    else
                    {
                        Debug.Log($"找到已存在的单例实例: {typeof(T).Name}");
                    }
                }

                return instance;
            }
        }
    }

    /// <summary>
    /// 确保实例唯一性
    /// </summary>
    protected virtual void Awake()
    {
        if (instance == null)
        {
            // 若当前实例是第一个，则设为单例
            instance = this as T;
            DontDestroyOnLoad(gameObject);
            Initialize();
            _isInitialized = true;
        }
        else if (instance != this)
        {
            // 若存在其他实例，则销毁当前实例
            Debug.LogWarning($"检测到重复的 {typeof(T).Name} 实例，已自动销毁");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 初始化方法，子类可重写实现自定义初始化逻辑
    /// </summary>
    protected virtual void Initialize()
    {

    }

    /// <summary>
    /// 检查单例是否已初始化
    /// </summary>
    public bool IsInitialized => _isInitialized;

    /// <summary>
    /// 销毁时清理
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
    /// 清理方法，子类可重写实现资源释放逻辑
    /// </summary>
    protected virtual void Cleanup()
    {

    }

    /// <summary>
    /// 防止在编辑模式下被实例化
    /// </summary>
    protected virtual void OnValidate()
    {
        if (Application.isPlaying && instance != null && instance != this)
        {
            Debug.LogWarning($"场景中存在多个 {typeof(T).Name} 实例，运行时将自动销毁多余实例");
        }
    }
}
