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

        // ����һ���۵����򣬱���Ϊ "Effect"
        Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, "Effect", true);

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            position.y += EditorGUIUtility.singleLineHeight;
            position.height -= EditorGUIUtility.singleLineHeight;

            // ���Ʊ�����
            Rect backgroundRect = new Rect(position.x - 15, position.y, position.width + 15, EditorGUIUtility.singleLineHeight * 3);
            EditorGUI.DrawRect(backgroundRect, new Color(0.1f, 0.1f, 0.1f, 0.1f));

            // ��ȡ��ǰ���л�����
            var targetObject = property.serializedObject.targetObject;

            // ͨ�������ȡ WorldState ʵ��
            WorldState worldState = GetWorldStateFromTarget(targetObject);
            if (worldState == null)
            {
                EditorGUI.LabelField(position, "WorldState not found.");
                EditorGUI.EndProperty();
                return;
            }

            // ��ȡ���б����������͵��б�
            List<(string variableName, string variableType)> variablePairs = worldState.GetVariableNameAndTypePairs();
            string[] variableNamesWithTypes = new string[variablePairs.Count];
            for (int i = 0; i < variablePairs.Count; i++)
            {
                variableNamesWithTypes[i] = $"{variablePairs[i].variableName} ({variablePairs[i].variableType})";
            }

            // ��ȡ worldStateName ����
            SerializedProperty worldStateNameProperty = property.FindPropertyRelative("worldStateName");
            string currentWorldStateName = worldStateNameProperty.stringValue;

            // �ҵ���ǰѡ�������
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

            // ��ʾ�����˵�ѡ�� worldStateName
            Rect dropdownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            selectedIndex = EditorGUI.Popup(dropdownRect, "World State", selectedIndex, variableNamesWithTypes);

            // ���� worldStateName ����
            worldStateNameProperty.stringValue = variablePairs[selectedIndex].variableName;

            // ��ȡ changeValue ����
            SerializedProperty changeValueProperty = property.FindPropertyRelative("changeValue");

            // ����ѡ��ı���������ʾ��ͬ������ؼ�
            string selectedVariableType = variablePairs[selectedIndex].variableType;
            Rect valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(valueRect, "Change Value");
            valueRect.y += EditorGUIUtility.singleLineHeight;

            // ���ݱ���������ʾ��ͬ�������ֶΣ����������Ͳ�ƥ������
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
                    // Ĭ��ʹ���ַ�������
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
            return EditorGUIUtility.singleLineHeight * 4; // ����һ�������۵�����
        }
        return EditorGUIUtility.singleLineHeight;
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

        // ����Ҳ��� _state �ֶΣ����������ֶ�������Ϊ WorldState ���ֶ�
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