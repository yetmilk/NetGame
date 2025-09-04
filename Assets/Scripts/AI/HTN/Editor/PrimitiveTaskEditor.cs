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

    // 保存当前选中的类名和方法名，避免切换资源时重置
    private string currentSelectedClass;
    private string currentSelectedMethod;

    private void OnEnable()
    {
        try
        {
            targetAssembly = Assembly.Load("Assembly-CSharp");
        }
        catch
        {
            targetAssembly = Assembly.GetExecutingAssembly();
        }

        RefreshClassAndMethodLists();

        // 加载目标对象已保存的值
        PrimitiveTask primitiveTask = (PrimitiveTask)target;
        currentSelectedClass = primitiveTask.className;
        currentSelectedMethod = primitiveTask.ActionName;

        // 恢复之前的选择状态
        RestoreSelectionState();
    }

    private void RefreshClassAndMethodLists()
    {
        try
        {
            var types = targetAssembly.GetTypes()
               .Where(t => t.IsDefined(typeof(PrimitiveTaskClassAttribute), true))
               .ToList();

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
            isDataLoaded = true;
            UpdateMethodNames();
        }
        catch (Exception ex)
        {
            Debug.LogError($"加载类和方法时出错: {ex.Message}");
            isDataLoaded = false;
        }
    }

    // 恢复之前保存的选择状态
    private void RestoreSelectionState()
    {
        if (!isDataLoaded || classNames == null || classNames.Length == 0) return;

        // 恢复类选择
        selectedClassIndex = Array.IndexOf(classNames, currentSelectedClass);
        if (selectedClassIndex == -1)
            selectedClassIndex = 0;

        UpdateMethodNames();

        // 恢复方法选择
        if (methodNames != null && methodNames.Length > 0)
        {
            selectedMethodIndex = Array.IndexOf(methodNames, currentSelectedMethod);
            if (selectedMethodIndex == -1)
                selectedMethodIndex = 0;
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

        if (!classMethods.ContainsKey(selectedClassName))
        {
            Debug.LogError($"未找到类 '{selectedClassName}' 的方法列表");
            methodNames = new string[0];
            selectedMethodIndex = 0;
            return;
        }

        methodNames = classMethods[selectedClassName].ToArray();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PrimitiveTask primitiveTask = (PrimitiveTask)target;

        if (!isDataLoaded)
        {
            EditorGUILayout.HelpBox("无法加载类和方法数据，请检查代码或重新编译。", MessageType.Error);
            return;
        }

        // 显示类选择
        if (classNames == null || classNames.Length == 0)
        {
            EditorGUILayout.HelpBox("无可用的类", MessageType.Info);
        }
        else
        {
            EditorGUI.BeginChangeCheck();
            selectedClassIndex = EditorGUILayout.Popup("Class Name", selectedClassIndex, classNames);

            if (EditorGUI.EndChangeCheck())
            {
                UpdateMethodNames();
                // 更新当前选择的类名
                currentSelectedClass = classNames[selectedClassIndex];
            }
        }

        // 显示方法选择
        if (methodNames == null || methodNames.Length == 0)
        {
            EditorGUILayout.HelpBox("无可用的方法", MessageType.Info);
        }
        else
        {
            EditorGUI.BeginChangeCheck();
            selectedMethodIndex = EditorGUILayout.Popup("Action Name", selectedMethodIndex, methodNames);
            if (EditorGUI.EndChangeCheck())
            {
                // 更新当前选择的方法名
                currentSelectedMethod = methodNames[selectedMethodIndex];
            }
        }

        // 添加赋值按钮
        if (GUILayout.Button("应用选择到字段"))
        {
            primitiveTask.ActionName = currentSelectedMethod;
            primitiveTask.className = currentSelectedClass;
            primitiveTask.InitializeExecuteAction();
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets(); // 立即保存更改
            Debug.Log($"已应用: {currentSelectedClass}.{currentSelectedMethod}");
        }
    }
}