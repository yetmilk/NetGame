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

    // ���浱ǰѡ�е������ͷ������������л���Դʱ����
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

        // ����Ŀ������ѱ����ֵ
        PrimitiveTask primitiveTask = (PrimitiveTask)target;
        currentSelectedClass = primitiveTask.className;
        currentSelectedMethod = primitiveTask.ActionName;

        // �ָ�֮ǰ��ѡ��״̬
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
            Debug.LogError($"������ͷ���ʱ����: {ex.Message}");
            isDataLoaded = false;
        }
    }

    // �ָ�֮ǰ�����ѡ��״̬
    private void RestoreSelectionState()
    {
        if (!isDataLoaded || classNames == null || classNames.Length == 0) return;

        // �ָ���ѡ��
        selectedClassIndex = Array.IndexOf(classNames, currentSelectedClass);
        if (selectedClassIndex == -1)
            selectedClassIndex = 0;

        UpdateMethodNames();

        // �ָ�����ѡ��
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
            Debug.LogError($"δ�ҵ��� '{selectedClassName}' �ķ����б�");
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
            EditorGUILayout.HelpBox("�޷�������ͷ������ݣ������������±��롣", MessageType.Error);
            return;
        }

        // ��ʾ��ѡ��
        if (classNames == null || classNames.Length == 0)
        {
            EditorGUILayout.HelpBox("�޿��õ���", MessageType.Info);
        }
        else
        {
            EditorGUI.BeginChangeCheck();
            selectedClassIndex = EditorGUILayout.Popup("Class Name", selectedClassIndex, classNames);

            if (EditorGUI.EndChangeCheck())
            {
                UpdateMethodNames();
                // ���µ�ǰѡ�������
                currentSelectedClass = classNames[selectedClassIndex];
            }
        }

        // ��ʾ����ѡ��
        if (methodNames == null || methodNames.Length == 0)
        {
            EditorGUILayout.HelpBox("�޿��õķ���", MessageType.Info);
        }
        else
        {
            EditorGUI.BeginChangeCheck();
            selectedMethodIndex = EditorGUILayout.Popup("Action Name", selectedMethodIndex, methodNames);
            if (EditorGUI.EndChangeCheck())
            {
                // ���µ�ǰѡ��ķ�����
                currentSelectedMethod = methodNames[selectedMethodIndex];
            }
        }

        // ��Ӹ�ֵ��ť
        if (GUILayout.Button("Ӧ��ѡ���ֶ�"))
        {
            primitiveTask.ActionName = currentSelectedMethod;
            primitiveTask.className = currentSelectedClass;
            primitiveTask.InitializeExecuteAction();
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets(); // �����������
            Debug.Log($"��Ӧ��: {currentSelectedClass}.{currentSelectedMethod}");
        }
    }
}