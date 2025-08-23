using System;
using System.Reflection;
using UnityEngine;

// 序列化对方法的引用
[System.Serializable]
public class MethodReference
{
    [SerializeField] private string className;
    [SerializeField] private string methodName;
    [SerializeField] private string assemblyQualifiedName;

    public MethodReference() { }

    public MethodReference(Type type, string methodName)
    {
        this.className = type.FullName;
        this.methodName = methodName;
        this.assemblyQualifiedName = type.AssemblyQualifiedName;
    }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(className) && !string.IsNullOrEmpty(methodName);
    }

    public T GetMethod<T>() where T : class
    {
        if (!IsValid())
            return null;

        try
        {
            Type type = Type.GetType(assemblyQualifiedName);
            if (type == null)
                return null;

            MethodInfo method = type.GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            if (method == null)
                return null;

            // 如果是实例方法，需要创建类的实例
            object target = null;
            if (!method.IsStatic)
            {
                target = Activator.CreateInstance(type);
                if (target == null)
                    return null;
            }

            return Delegate.CreateDelegate(typeof(T), target, method) as T;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error getting method: {e.Message}");
            return null;
        }
    }

    public string GetDisplayName()
    {
        if (!IsValid())
            return "None";

        return $"{className}.{methodName}";
    }
}