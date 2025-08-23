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
            // ���Լ�����Ϸ����
            targetAssembly = Assembly.Load("Assembly-CSharp");
        }
        catch
        {
            // ���˵���ǰ����
            targetAssembly = Assembly.GetExecutingAssembly();
        }

        RefreshClassAndMethodLists();
    }

    private void RefreshClassAndMethodLists()
    {
        try
        {
            // ��ȡ���д��� PrimitiveTaskClassAttribute ����
            var types = targetAssembly.GetTypes()
               .Where(t => t.IsDefined(typeof(PrimitiveTaskClassAttribute), true))
               .ToList();

            // ��ȡ��������
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

            // ȷ����ʼ�����б���ȷ
            UpdateMethodNames();
        }
        catch (Exception ex)
        {
            Debug.LogError($"����������ͷ���ʱ����: {ex.Message}");
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

        // ����ֵ����Ƿ���ڸü�
        if (!classMethods.ContainsKey(selectedClassName))
        {
            Debug.LogError($"δ�ҵ����� '{selectedClassName}' �ķ����б�");
            methodNames = new string[0];
            selectedMethodIndex = 0;
            return;
        }

        methodNames = classMethods[selectedClassName].ToArray();
        PrimitiveTask primitiveTask = (PrimitiveTask)target;

        // ȷ�� selectedMethodIndex ��Ч
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

        // ��������Ƿ���سɹ�
        if (!isDataLoaded)
        {
            EditorGUILayout.HelpBox("�޷������������ݣ�������򼯺��������á�", MessageType.Error);
            return;
        }

        // ��ʾ����ѡ��
        if (classNames == null || classNames.Length == 0)
        {
            EditorGUILayout.HelpBox("�޿��õ�������", MessageType.Info);
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

        // ��ʾ����ѡ��
        if (methodNames == null || methodNames.Length == 0)
        {
            EditorGUILayout.HelpBox("�޿��õķ���", MessageType.Info);
        }
        else
        {
            selectedMethodIndex = EditorGUILayout.Popup("Action Name", selectedMethodIndex, methodNames);
            // ��������
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