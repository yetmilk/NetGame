using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

[CustomPropertyDrawer(typeof(ConditionItem))]
public class ConditionItemDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 获取当前序列化对象
        var targetObject = property.serializedObject.targetObject;

        // 通过反射获取 _state 字段
        WorldState worldState = GetWorldStateFromTarget(targetObject);
        if (worldState == null)
        {
            EditorGUI.LabelField(position, "WorldState not found.");
            EditorGUI.EndProperty();
            return;
        }

        // 获取变量名和类型列表
        List<(string variableName, string variableType)> variablePairs = worldState.GetVariableNameAndTypePairs();
        string[] variableNamesWithTypes = new string[variablePairs.Count];
        for (int i = 0; i < variablePairs.Count; i++)
        {
            variableNamesWithTypes[i] = $"{variablePairs[i].variableName} ({variablePairs[i].variableType})";
        }

        // 获取 variableName 属性
        SerializedProperty variableNameProperty = property.FindPropertyRelative("variableName");
        string currentVariableName = variableNameProperty.stringValue;

        // 找到当前选择的变量
        int selectedIndex = -1;
        for (int i = 0; i < variablePairs.Count; i++)
        {
            if (variablePairs[i].variableName == currentVariableName)
            {
                selectedIndex = i;
                break;
            }
        }
        if (selectedIndex == -1)
        {
            selectedIndex = 0;
        }

        // 显示下拉框
        Rect dropdownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        selectedIndex = EditorGUI.Popup(dropdownRect, "Variable Name", selectedIndex, variableNamesWithTypes);

        // 更新 variableName 属性
        variableNameProperty.stringValue = variablePairs[selectedIndex].variableName;

        // 显示其他属性
        Rect otherPropertiesRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight * 2);
        EditorGUI.indentLevel++;
        EditorGUI.PropertyField(new Rect(otherPropertiesRect.x, otherPropertiesRect.y, otherPropertiesRect.width, EditorGUIUtility.singleLineHeight), property.FindPropertyRelative("compareValue"));
        EditorGUI.PropertyField(new Rect(otherPropertiesRect.x, otherPropertiesRect.y + EditorGUIUtility.singleLineHeight, otherPropertiesRect.width, EditorGUIUtility.singleLineHeight), property.FindPropertyRelative("comparisonOperator"));
        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 3;
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

        // 如果找不到 _state 字段，遍历所有字段查找 WorldState 类型的字段
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