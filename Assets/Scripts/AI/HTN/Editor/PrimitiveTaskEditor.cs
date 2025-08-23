using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrimitiveTask))]
public class PrimitiveTaskEditor : Editor
{
    private string[] classNames;
    private int selectedClassIndex;
    private string[] methodNames;
    private int selectedMethodIndex;
    private Dictionary<string, List<string>> classMethods;
    private Assembly targetAssembly;
    private bool isDataLoaded = false;

    private void OnEnable()
    {
        try
        {
            // 尝试加载游戏程序集
            targetAssembly = Assembly.Load("Assembly-CSharp");
        }
        catch
        {
            // 回退到当前程序集
            targetAssembly = Assembly.GetExecutingAssembly();
        }

        RefreshClassAndMethodLists();
    }

    private void RefreshClassAndMethodLists()
    {
        try
        {
            // 获取所有带有 PrimitiveTaskClassAttribute 的类
            var types = targetAssembly.GetTypes()
               .Where(t => t.IsDefined(typeof(PrimitiveTaskClassAttribute), true))
               .ToList();

            // 获取所有类名
            List<string> classNamesList = new List<string>();
            classMethods = new Dictionary<string, List<string>>();

            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<PrimitiveTaskClassAttribute>();
                string className = attribute.ClassName;

                if (!classNamesList.Contains(className))
                {
                    classNamesList.Add(className);
                }

                MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                List<string> selectableMethods = new List<string>();

                foreach (MethodInfo method in methods)
                {
                    if (method.GetCustomAttribute<SelectableMethodAttribute>() != null)
                    {
                        selectableMethods.Add(method.Name);
                    }
                }

                if (!classMethods.ContainsKey(className))
                {
                    classMethods[className] = selectableMethods;
                }
                else
                {
                    classMethods[className].AddRange(selectableMethods);
                }
            }

            classNames = classNamesList.ToArray();
            selectedClassIndex = 0;
            isDataLoaded = true;

            // 确保初始方法列表正确
            UpdateMethodNames();
        }
        catch (Exception ex)
        {
            Debug.LogError($"加载任务类和方法时出错: {ex.Message}");
            isDataLoaded = false;
        }
    }

    private void UpdateMethodNames()
    {
        if (!isDataLoaded || classNames.Length == 0)
        {
            methodNames = new string[0];
            return;
        }

        string selectedClassName = classNames[selectedClassIndex];

        // 检查字典中是否存在该键
        if (!classMethods.ContainsKey(selectedClassName))
        {
            Debug.LogError($"未找到类名 '{selectedClassName}' 的方法列表");
            methodNames = new string[0];
            selectedMethodIndex = 0;
            return;
        }

        methodNames = classMethods[selectedClassName].ToArray();
        PrimitiveTask primitiveTask = (PrimitiveTask)target;

        // 确保 selectedMethodIndex 有效
        selectedMethodIndex = Array.IndexOf(methodNames, primitiveTask.ActionName);
        if (selectedMethodIndex == -1)
        {
            selectedMethodIndex = 0;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PrimitiveTask primitiveTask = (PrimitiveTask)target;

        // 检查数据是否加载成功
        if (!isDataLoaded)
        {
            EditorGUILayout.HelpBox("无法加载任务数据，请检查程序集和特性配置。", MessageType.Error);
            return;
        }

        // 显示类名选择
        if (classNames == null || classNames.Length == 0)
        {
            EditorGUILayout.HelpBox("无可用的任务类", MessageType.Info);
        }
        else
        {
            EditorGUI.BeginChangeCheck();
            selectedClassIndex = EditorGUILayout.Popup("Class Name", selectedClassIndex, classNames);

            if (EditorGUI.EndChangeCheck())
            {
                UpdateMethodNames();
            }
        }

        // 显示方法选择
        if (methodNames == null || methodNames.Length == 0)
        {
            EditorGUILayout.HelpBox("无可用的方法", MessageType.Info);
        }
        else
        {
            selectedMethodIndex = EditorGUILayout.Popup("Action Name", selectedMethodIndex, methodNames);
            // 更新数据
            if (GUI.changed)
            {
                primitiveTask.ActionName = methodNames[selectedMethodIndex];
                primitiveTask.className = classNames[selectedClassIndex];
                primitiveTask.InitializeExecuteAction();
                EditorUtility.SetDirty(target);
            }
        }
    }
}