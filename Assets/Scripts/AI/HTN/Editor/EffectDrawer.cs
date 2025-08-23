using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

[CustomPropertyDrawer(typeof(Effect))]
public class EffectDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 创建一个折叠区域，标题为 "Effect"
        Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, "Effect", true);

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            position.y += EditorGUIUtility.singleLineHeight;
            position.height -= EditorGUIUtility.singleLineHeight;

            // 绘制背景框
            Rect backgroundRect = new Rect(position.x - 15, position.y, position.width + 15, EditorGUIUtility.singleLineHeight * 3);
            EditorGUI.DrawRect(backgroundRect, new Color(0.1f, 0.1f, 0.1f, 0.1f));

            // 获取当前序列化对象
            var targetObject = property.serializedObject.targetObject;

            // 通过反射获取 WorldState 实例
            WorldState worldState = GetWorldStateFromTarget(targetObject);
            if (worldState == null)
            {
                EditorGUI.LabelField(position, "WorldState not found.");
                EditorGUI.EndProperty();
                return;
            }

            // 获取所有变量名和类型的列表
            List<(string variableName, string variableType)> variablePairs = worldState.GetVariableNameAndTypePairs();
            string[] variableNamesWithTypes = new string[variablePairs.Count];
            for (int i = 0; i < variablePairs.Count; i++)
            {
                variableNamesWithTypes[i] = $"{variablePairs[i].variableName} ({variablePairs[i].variableType})";
            }

            // 获取 worldStateName 属性
            SerializedProperty worldStateNameProperty = property.FindPropertyRelative("worldStateName");
            string currentWorldStateName = worldStateNameProperty.stringValue;

            // 找到当前选择的索引
            int selectedIndex = -1;
            for (int i = 0; i < variablePairs.Count; i++)
            {
                if (variablePairs[i].variableName == currentWorldStateName)
                {
                    selectedIndex = i;
                    break;
                }
            }
            if (selectedIndex == -1)
            {
                selectedIndex = 0;
            }

            // 显示下拉菜单选择 worldStateName
            Rect dropdownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            selectedIndex = EditorGUI.Popup(dropdownRect, "World State", selectedIndex, variableNamesWithTypes);

            // 更新 worldStateName 属性
            worldStateNameProperty.stringValue = variablePairs[selectedIndex].variableName;

            // 获取 changeValue 属性
            SerializedProperty changeValueProperty = property.FindPropertyRelative("changeValue");

            // 根据选择的变量类型显示不同的输入控件
            string selectedVariableType = variablePairs[selectedIndex].variableType;
            Rect valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(valueRect, "Change Value");
            valueRect.y += EditorGUIUtility.singleLineHeight;

            // 根据变量类型显示不同的输入字段，并处理类型不匹配的情况
            switch (selectedVariableType)
            {
                case "Int32":
                    int intValue = 0;
                    if (!string.IsNullOrEmpty(changeValueProperty.stringValue))
                    {
                        if (!int.TryParse(changeValueProperty.stringValue, out intValue))
                        {
                            intValue = 0;
                            changeValueProperty.stringValue = "0";
                        }
                    }
                    intValue = EditorGUI.IntField(valueRect, "", intValue);
                    changeValueProperty.stringValue = intValue.ToString();
                    break;

                case "Single":
                    float floatValue = 0f;
                    if (!string.IsNullOrEmpty(changeValueProperty.stringValue))
                    {
                        if (!float.TryParse(changeValueProperty.stringValue, out floatValue))
                        {
                            floatValue = 0f;
                            changeValueProperty.stringValue = "0";
                        }
                    }
                    floatValue = EditorGUI.FloatField(valueRect, "", floatValue);
                    changeValueProperty.stringValue = floatValue.ToString();
                    break;

                case "Boolean":
                    bool boolValue = false;
                    if (!string.IsNullOrEmpty(changeValueProperty.stringValue))
                    {
                        if (!bool.TryParse(changeValueProperty.stringValue, out boolValue))
                        {
                            boolValue = false;
                            changeValueProperty.stringValue = "false";
                        }
                    }
                    boolValue = EditorGUI.Toggle(valueRect, "", boolValue);
                    changeValueProperty.stringValue = boolValue.ToString();
                    break;

                default:
                    // 默认使用字符串输入
                    changeValueProperty.stringValue = EditorGUI.TextField(valueRect, "", changeValueProperty.stringValue);
                    break;
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
        {
            return EditorGUIUtility.singleLineHeight * 4; // 增加一行用于折叠标题
        }
        return EditorGUIUtility.singleLineHeight;
    }

    private WorldState GetWorldStateFromTarget(Object targetObject)
    {
        // 尝试获取 _state 字段
        FieldInfo stateField = targetObject.GetType().GetField("_state",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (stateField != null && stateField.FieldType == typeof(WorldState))
        {
            return stateField.GetValue(targetObject) as WorldState;
        }

        // 如果找不到 _state 字段，查找所有字段中类型为 WorldState 的字段
        foreach (FieldInfo field in targetObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (field.FieldType == typeof(WorldState))
            {
                return field.GetValue(targetObject) as WorldState;
            }
        }

        return null;
    }
}