// 原代码注释
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// 定义新的委托类型，包含一个回调方法
public delegate void ActionWithCallback(Agent agent, Action onComplete);

[CreateAssetMenu(fileName = "PrimitiveTask", menuName = "HTN/Task/PrimitiveTask")]
public class PrimitiveTask : Task
{
    public WorldState _state;
    public override bool IsPrimitive => true;
    public List<Effect> effects;
    public string ActionName;
    public string className;
    public ActionWithCallback executeAction;
    // 初始化执行动作，适应新的委托类型
    public void InitializeExecuteAction()
    {
        // 根据 className 查找对应的类型
        Type executeClassType = FindTypeByClassName(className);

        if (executeClassType == null)
        {
            Debug.LogError($"未找到标签为 {className} 的类");
            return;
        }

        // 根据 ActionName 获取对应的方法信息
        // 注意：这里假设方法签名为 (Agent, Action)，请根据实际情况调整
        MethodInfo methodInfo = executeClassType.GetMethod(
            ActionName,
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            new Type[] { typeof(Agent), typeof(Action) },
            null
        );

        if (methodInfo != null)
        {
            // 创建一个委托实例并赋值给 executeAction
            executeAction = (ActionWithCallback)Delegate.CreateDelegate(typeof(ActionWithCallback), methodInfo);
            Debug.Log($"成功绑定方法: {className}.{ActionName}");
        }
        else
        {
            Debug.LogError($"未找到名为 {ActionName} 的方法，或方法签名不匹配 (需要: Agent, Action)");
        }
    }

    // 根据 className 查找对应的类型
    private Type FindTypeByClassName(string className)
    {
        // 获取当前程序集
        Assembly assembly = Assembly.GetExecutingAssembly();


        Type[] types = assembly.GetTypes();
        foreach (Type type in types)
        {
            // 检查类是否有 PrimitiveTaskClassAttribute 特性
            var attribute = type.GetCustomAttribute<PrimitiveTaskClassAttribute>();
            if (attribute != null && attribute.ClassName == className)
            {
                return type;
            }
        }


        return null;
    }

    public ActionWithCallback GetExecuteAction()
    {
        if (executeAction == null)
        {
            InitializeExecuteAction();
        }
        return executeAction;
    }
}

// 用于标记任务库中可选的方法
[AttributeUsage(AttributeTargets.Method)]
public class SelectableMethodAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public class PrimitiveTaskClassAttribute : Attribute
{
    public string ClassName;

    public PrimitiveTaskClassAttribute(string className)
    {
        this.ClassName = className;
    }
}

