using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Collections.LowLevel.Unsafe;

[CustomEditor(typeof(WorldState))]
public class WorldStateEditor : Editor
{
    private string newKey;
    #region---------------�����������ͼ�------------------
    private VariableType newVariableType = VariableType.Float;
    private float newFloatValue;
    private double newDoubleValue;
    private int newIntValue;
    private bool newBoolValue;
    private string newStringValue;
    #endregion
    private Vector3 newVector3Value;
    private Vector3 currentVector3Value; // 新增：用于存储当前 Vector3 值
    private Vector3 scrollPosition;
    private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        WorldState worldState = (WorldState)target;

        // 显示变量列表
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("World State Variables", EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

        if (worldState.runTimeValues == null || worldState.runTimeValues.Count == 0)
        {
            EditorGUILayout.LabelField("No variables defined.", EditorStyles.helpBox);
        }
        else
        {
            // 使用临时列表避免运行时修改字典
            var keys = new List<string>(worldState.runTimeValues.Keys);
            foreach (var key in keys)
            {
                if (!worldState.runTimeValues.TryGetValue(key, out var value))
                    continue;

                if (!foldoutStates.ContainsKey(key))
                    foldoutStates[key] = false;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();

                // 折叠按钮
                foldoutStates[key] = EditorGUILayout.Foldout(foldoutStates[key], key, true);

                // 删除按钮
                if (GUILayout.Button("X", GUILayout.Width(24)))
                {
                    if (EditorUtility.DisplayDialog("Confirm Delete",
                        $"Are you sure you want to delete variable '{key}'?", "Yes", "No"))
                    {
                        worldState.runTimeValues.Remove(key);
                        EditorUtility.SetDirty(worldState);
                        continue;
                    }
                }

                EditorGUILayout.EndHorizontal();

                if (foldoutStates[key])
                {
                    EditorGUILayout.Space();

                    // 显示变量类型
                    if (value.value != null)
                    {
                        EditorGUILayout.LabelField("Type:", value.value.GetType().Name);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Type:", "Null");
                    }

                    // 显示并编辑变量值
                    if (value.value is float floatValue)
                    {
                        float newValue = EditorGUILayout.Slider("Value:", floatValue, 0f, 100f);
                        if (newValue != floatValue)
                        {
                            value.value = newValue;
                            EditorUtility.SetDirty(worldState);
                        }
                    }
                    else if (value.value is int intValue)
                    {
                        int newValue = EditorGUILayout.IntField("Value:", intValue);
                        if (newValue != intValue)
                        {
                            value.value = newValue;
                            EditorUtility.SetDirty(worldState);
                        }
                    }
                    else if (value.value is bool boolValue)
                    {
                        bool newValue = EditorGUILayout.Toggle("Value:", boolValue);
                        if (newValue != boolValue)
                        {
                            value.value = newValue;
                            EditorUtility.SetDirty(worldState);
                        }
                    }
                    else if (value.value is string stringValue)
                    {
                        string newValue = EditorGUILayout.TextField("Value:", stringValue);
                        if (newValue != stringValue)
                        {
                            value.value = newValue;
                            EditorUtility.SetDirty(worldState);
                        }
                    }
                    else if (value.value is Vector3 vector3Value)
                    {
                        // 新增：实现 Vector3 类型变量值的修改功能
                        currentVector3Value = vector3Value;
                        Vector3 newVector3 = EditorGUILayout.Vector3Field("Value:", currentVector3Value);
                        if (newVector3 != currentVector3Value)
                        {
                            value.value = newVector3;
                            EditorUtility.SetDirty(worldState);
                        }
                    }
                    else if(value.value is double doubleValue )
                    {
                        double newDouble = EditorGUILayout.DoubleField("Value:", doubleValue);
                        if(newDouble !=doubleValue)
                        {
                            value.value = newDouble;
                            EditorUtility.SetDirty(worldState);
                        }
                    }
                    else if (value.value != null)
                    {
                        EditorGUILayout.LabelField("Value:", value.value.ToString());
                        EditorGUILayout.HelpBox("Editing complex types not supported.", MessageType.Info);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Value:", "Null");
                        if (GUILayout.Button("Set Default Value"))
                        {
                            // 设置变量默认值
                            Type valueType = value.value?.GetType();
                            if (valueType != null)
                            {
                                switch (Type.GetTypeCode(valueType))
                                {
                                    case TypeCode.Single:
                                        value.value = 0f;
                                        break;
                                    case TypeCode.Int32:
                                        value.value = 0;
                                        break;
                                    case TypeCode.Boolean:
                                        value.value = false;
                                        break;
                                    case TypeCode.String:
                                        value.value = "";
                                        break;
                                }
                            }
                            EditorUtility.SetDirty(worldState);
                        }
                    }

                    // 显示是否触发重新规划
                    bool triggerReplan = value.changeTriggerReplan;
                    bool newTriggerReplan = EditorGUILayout.Toggle("Trigger Replan:", triggerReplan);
                    if (newTriggerReplan != triggerReplan)
                    {
                        value.changeTriggerReplan = newTriggerReplan;
                        EditorUtility.SetDirty(worldState);
                    }

                    EditorGUILayout.Space();
                }

                EditorGUILayout.EndVertical();
            }
        }

        EditorGUILayout.EndScrollView();

        // 添加新变量区域
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Add New Variable", EditorStyles.boldLabel);

        newKey = EditorGUILayout.TextField("Key:", newKey);
        newVariableType = (VariableType)EditorGUILayout.EnumPopup("Type:", newVariableType);

        // 根据选择的类型显示不同的输入框
        switch (newVariableType)
        {
            case VariableType.Float:
                newFloatValue = EditorGUILayout.FloatField("Value:", newFloatValue);
                break;
            case VariableType.Int:
                newIntValue = EditorGUILayout.IntField("Value:", newIntValue);
                break;
            case VariableType.Bool:
                newBoolValue = EditorGUILayout.Toggle("Value:", newBoolValue);
                break;
            case VariableType.String:
                newStringValue = EditorGUILayout.TextField("Value:", newStringValue);
                break;
            case VariableType.Vector3:
                newVector3Value = EditorGUILayout.Vector3Field("Value", newVector3Value);
                break;
        }

        // 添加按钮
        EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(newKey));
        if (GUILayout.Button("Add Variable", GUILayout.Height(30)))
        {
            // 创建新的 Value 对象
            WorldState.Value newValue = new WorldState.Value();

            // 根据选择的类型设置值
            switch (newVariableType)
            {
                case VariableType.Float:
                    newValue.value = newFloatValue;
                    break;
                case VariableType.Int:
                    newValue.value = newIntValue;
                    break;
                case VariableType.Bool:
                    newValue.value = newBoolValue;
                    break;
                case VariableType.String:
                    newValue.value = newStringValue;
                    break;
                case VariableType.Vector3:
                    newValue.value = newVector3Value;
                    break;
            }

            // 设置是否触发重新规划
            newValue.changeTriggerReplan = true;

            if (worldState.runTimeValues.ContainsKey(newKey))
            {
                if (EditorUtility.DisplayDialog("Replace Variable",
                    $"Variable '{newKey}' already exists. Do you want to replace it?", "Yes", "No"))
                {
                    worldState.runTimeValues[newKey] = newValue;
                    EditorUtility.SetDirty(worldState);
                    newKey = "";
                    ResetNewVariableValues();
                }
            }
            else
            {
                worldState.runTimeValues.Add(newKey, newValue);
                EditorUtility.SetDirty(worldState);
                newKey = "";
                ResetNewVariableValues();
            }
        }
        EditorGUI.EndDisabledGroup();

        // 添加将当前值设为模板值的按钮
        if (GUILayout.Button("Set Current Values as Template", GUILayout.Height(30)))
        {
            worldState.SerializeToJson();
            EditorUtility.SetDirty(worldState);
        }

        serializedObject.ApplyModifiedProperties();
    }

    // 重置新变量的值
    private void ResetNewVariableValues()
    {
        newFloatValue = 0f;
        newIntValue = 0;
        newBoolValue = false;
        newStringValue = "";
    }
}