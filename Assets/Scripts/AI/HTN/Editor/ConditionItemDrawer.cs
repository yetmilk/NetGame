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

        // ��ȡ��ǰ���л�����
        var targetObject = property.serializedObject.targetObject;

        // ͨ�������ȡ _state �ֶ�
        WorldState worldState = GetWorldStateFromTarget(targetObject);
        if (worldState == null)
        {
            EditorGUI.LabelField(position, "WorldState not found.");
            EditorGUI.EndProperty();
            return;
        }

        // ��ȡ�������������б�
        List<(string variableName, string variableType)> variablePairs = worldState.GetVariableNameAndTypePairs();
        string[] variableNamesWithTypes = new string[variablePairs.Count];
        for (int i = 0; i < variablePairs.Count; i++)
        {
            variableNamesWithTypes[i] = $"{variablePairs[i].variableName} ({variablePairs[i].variableType})";
        }

        // ��ȡ variableName ����
        SerializedProperty variableNameProperty = property.FindPropertyRelative("variableName");
        string currentVariableName = variableNameProperty.stringValue;

        // �ҵ���ǰѡ��ı���
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

        // ��ʾ������
        Rect dropdownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        selectedIndex = EditorGUI.Popup(dropdownRect, "Variable Name", selectedIndex, variableNamesWithTypes);

        // ���� variableName ����
        variableNameProperty.stringValue = variablePairs[selectedIndex].variableName;

        // ��ʾ��������
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
        // ���Ի�ȡ _state �ֶ�
        FieldInfo stateField = targetObject.GetType().GetField("_state",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (stateField != null && stateField.FieldType == typeof(WorldState))
        {
            return stateField.GetValue(targetObject) as WorldState;
        }

        // ����Ҳ��� _state �ֶΣ����������ֶβ��� WorldState ���͵��ֶ�
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